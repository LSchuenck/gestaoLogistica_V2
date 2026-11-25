using System.Security.Claims;
using System.Text;
using GestaoLogistica.API.Middlewares;
using GestaoLogistica.Application;
using GestaoLogistica.Application.Comum;
using GestaoLogistica.Application.Interfaces;
using GestaoLogistica.Infrastructure;
using GestaoLogistica.Infrastructure.Dados;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var configuracao = builder.Configuration;

// A chave do JWT fica fora do código (appsettings.Development.json ou variável de ambiente Jwt__Chave)
var chaveJwt = configuracao["Jwt:Chave"];
if (string.IsNullOrWhiteSpace(chaveJwt) || chaveJwt.Length < 32)
    throw new InvalidOperationException("Configure a chave 'Jwt:Chave' com pelo menos 32 caracteres (veja o README).");

// ---------- Camadas (injeção de dependência) ----------
builder.Services.AdicionarCamadaAplicacao();
builder.Services.AdicionarCamadaInfraestrutura(configuracao);

// ---------- Controllers + erros de validação no padrão ResultWrapper ----------
builder.Services.AddControllers(opcoes =>
{
    opcoes.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;

    // Mensagens padrão em português
    var mensagens = opcoes.ModelBindingMessageProvider;
    mensagens.SetMissingRequestBodyRequiredValueAccessor(() => "Envie os dados no corpo da requisição.");
    mensagens.SetNonPropertyAttemptedValueIsInvalidAccessor(valor => $"O valor '{valor}' não é válido.");
    mensagens.SetAttemptedValueIsInvalidAccessor((valor, campo) => $"O valor '{valor}' não é válido para {campo}.");
})
.AddJsonOptions(opcoes => opcoes.AllowInputFormatterExceptionMessages = false)
.ConfigureApiBehaviorOptions(opcoes =>
{
    // Erros como 415 ficam sem corpo aqui e são preenchidos pelo UseStatusCodePages
    opcoes.SuppressMapClientErrors = true;

    opcoes.InvalidModelStateResponseFactory = contexto =>
    {
        var erros = contexto.ModelState.Values
            .SelectMany(valor => valor.Errors)
            .Select(erro => string.IsNullOrEmpty(erro.ErrorMessage) ? "JSON inválido ou campo com formato incorreto." : erro.ErrorMessage)
            .ToList();

        return new BadRequestObjectResult(ResultWrapper<object>.Falha("Dados inválidos.", erros));
    };
});

// ---------- Autenticação JWT ----------
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opcoes =>
    {
        // Regras usadas para validar o token recebido em cada requisição protegida
        opcoes.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = configuracao["Jwt:Emissor"],
            ValidateAudience = true,
            ValidAudience = configuracao["Jwt:Publico"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveJwt)),
            ClockSkew = TimeSpan.Zero
        };

        // Respostas 401 e 403 também no padrão ResultWrapper
        opcoes.Events = new JwtBearerEvents
        {
            // Recusa o token de quem foi desativado, excluído ou trocou de perfil depois do login
            OnTokenValidated = async contexto =>
            {
                var servico = contexto.HttpContext.RequestServices.GetRequiredService<IServicoAutenticacao>();
                var idUsuario = int.Parse(contexto.Principal!.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var perfil = contexto.Principal!.FindFirstValue(ClaimTypes.Role)!;

                if (!await servico.TokenAindaEhValido(idUsuario, perfil))
                    contexto.Fail("Usuário sem acesso.");
            },
            OnChallenge = async contexto =>
            {
                contexto.HandleResponse();
                contexto.Response.StatusCode = 401;
                await contexto.Response.WriteAsJsonAsync(ResultWrapper<object>.Falha(
                    "Acesso não autorizado.", new List<string> { "Token ausente, inválido ou expirado. Faça login novamente." }, 401));
            },
            OnForbidden = async contexto =>
            {
                contexto.Response.StatusCode = 403;
                await contexto.Response.WriteAsJsonAsync(ResultWrapper<object>.Falha(
                    "Acesso negado.", new List<string> { "Seu perfil não tem permissão para esta operação." }, 403));
            }
        };
    });
builder.Services.AddAuthorization();

// ---------- CORS: libera o front-end Vue para chamar a API ----------
builder.Services.AddCors(opcoes =>
{
    opcoes.AddPolicy("Frontend", politica => politica
        .WithOrigins(configuracao["Cors:OrigemFrontend"]!)
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

// ---------- Cria o banco e os dados de exemplo na primeira execução ----------
using (var escopo = app.Services.CreateScope())
{
    var contexto = escopo.ServiceProvider.GetRequiredService<ContextoBanco>();
    var criptografia = escopo.ServiceProvider.GetRequiredService<ICriptografiaSenha>();
    DadosIniciais.CriarBancoEPopular(contexto, criptografia, configuracao["AdministradorInicial:Senha"]);
}

// ---------- Pipeline das requisições ----------
app.UseMiddleware<TratamentoErrosMiddleware>();
app.UseStatusCodePages(async contexto =>
{
    // Respostas que ficariam sem corpo (rota inexistente, método errado...) também no padrão ResultWrapper
    var resposta = contexto.HttpContext.Response;
    var mensagem = resposta.StatusCode switch
    {
        404 => "Endereço não encontrado na API.",
        405 => "Método HTTP não permitido neste endereço.",
        415 => "Formato não suportado. Envie os dados em JSON.",
        _ => "Não foi possível realizar a operação."
    };
    await resposta.WriteAsJsonAsync(ResultWrapper<object>.Falha(mensagem, null, resposta.StatusCode));
});
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

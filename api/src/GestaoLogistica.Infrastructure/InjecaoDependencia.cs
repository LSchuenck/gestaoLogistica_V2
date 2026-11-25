using GestaoLogistica.Application.Interfaces;
using GestaoLogistica.Domain.Interfaces;
using GestaoLogistica.Infrastructure.Dados;
using GestaoLogistica.Infrastructure.Email;
using GestaoLogistica.Infrastructure.Repositorios;
using GestaoLogistica.Infrastructure.Seguranca;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GestaoLogistica.Infrastructure;

// Registra banco de dados, repositórios e serviços de segurança na injeção de dependência
public static class InjecaoDependencia
{
    public static IServiceCollection AdicionarCamadaInfraestrutura(this IServiceCollection servicos, IConfiguration configuracao)
    {
        var conexao = configuracao.GetConnectionString("BancoGestaoLogistica");
        // AutoDetect descobre a versão do servidor (funciona com MySQL e com o MariaDB do XAMPP)
        servicos.AddDbContext<ContextoBanco>(opcoes =>
            opcoes.UseMySql(conexao, ServerVersion.AutoDetect(conexao)));

        servicos.AddScoped<IRepositorioUsuario, RepositorioUsuario>();
        servicos.AddScoped<IRepositorioTransportadora, RepositorioTransportadora>();
        servicos.AddScoped<IRepositorioMotorista, RepositorioMotorista>();
        servicos.AddScoped<IRepositorioVeiculo, RepositorioVeiculo>();
        servicos.AddScoped<IRepositorioCliente, RepositorioCliente>();
        servicos.AddScoped<IRepositorioArmazem, RepositorioArmazem>();
        servicos.AddScoped<IRepositorioProduto, RepositorioProduto>();
        servicos.AddScoped<IRepositorioEstoque, RepositorioEstoque>();
        servicos.AddScoped<IRepositorioViagem, RepositorioViagem>();

        servicos.AddScoped<ICriptografiaSenha, CriptografiaSenhaBCrypt>();
        servicos.AddScoped<IGeradorToken, GeradorTokenJwt>();
        servicos.AddScoped<IServicoEmail, ServicoEmailPowerAutomate>();

        return servicos;
    }
}

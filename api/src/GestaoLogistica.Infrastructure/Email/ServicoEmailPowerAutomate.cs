using System.Net.Http.Json;
using GestaoLogistica.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GestaoLogistica.Infrastructure.Email;

// Envia os e-mails por meio de um fluxo do Power Automate
public class ServicoEmailPowerAutomate : IServicoEmail
{
    private static readonly HttpClient _http = new() { Timeout = TimeSpan.FromSeconds(15) };

    private readonly IConfiguration _configuracao;
    private readonly ILogger<ServicoEmailPowerAutomate> _logger;

    public ServicoEmailPowerAutomate(IConfiguration configuracao, ILogger<ServicoEmailPowerAutomate> logger)
    {
        _configuracao = configuracao;
        _logger = logger;
    }

    public async Task<bool> EnviarBoasVindas(string email, string nome, string senhaTemporaria)
    {
        var url = _configuracao["Email:UrlPowerAutomate"];
        if (string.IsNullOrWhiteSpace(url))
        {
            _logger.LogWarning("E-mail não enviado: 'Email:UrlPowerAutomate' não está configurado.");
            return false;
        }

        var mensagem = new
        {
            email,
            titulo = "Gestão Logística — Seu acesso foi criado",
            corpo = $"Olá, {nome}!\n\n" +
                    "Seu acesso ao sistema de Gestão Logística foi criado.\n\n" +
                    $"Sua senha temporária é: {senhaTemporaria}\n\n" +
                    "No primeiro acesso você será obrigado a criar uma nova senha.\n\n" +
                    $"O link do site é: {_configuracao["Cors:OrigemFrontend"]}\n\n" +
                    "Atenção: não compartilhe essa senha com ninguém."
        };

        try
        {
            var resposta = await _http.PostAsJsonAsync(url, mensagem);
            if (!resposta.IsSuccessStatusCode)
                _logger.LogWarning("E-mail não enviado: o Power Automate respondeu {Codigo}.", (int)resposta.StatusCode);

            return resposta.IsSuccessStatusCode;
        }
        catch (Exception erro)
        {
            // O cadastro não pode falhar só porque o e-mail falhou
            _logger.LogWarning(erro, "E-mail não enviado: erro ao chamar o Power Automate.");
            return false;
        }
    }
}

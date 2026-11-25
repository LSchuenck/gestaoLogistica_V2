namespace GestaoLogistica.Application.Interfaces;

// Envia os e-mails do sistema (implementado na Infrastructure com o Power Automate)
public interface IServicoEmail
{
    // Devolve false se o e-mail não pôde ser enviado
    Task<bool> EnviarBoasVindas(string email, string nome, string senhaTemporaria);
}

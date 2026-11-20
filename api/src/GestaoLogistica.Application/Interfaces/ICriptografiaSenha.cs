namespace GestaoLogistica.Application.Interfaces;

// Gera e confere o hash das senhas (implementado na Infrastructure com BCrypt)
public interface ICriptografiaSenha
{
    string GerarHash(string senha);
    bool SenhaConfere(string senha, string hash);
}

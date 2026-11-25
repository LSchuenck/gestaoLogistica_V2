using GestaoLogistica.Application.Comum;
using GestaoLogistica.Application.DTOs;

namespace GestaoLogistica.Application.Interfaces;

public interface IServicoAutenticacao
{
    Task<ResultWrapper<LoginRespostaDto>> Entrar(LoginDto dados);

    // Confere se o dono do token ainda existe, está ativo e tem o mesmo perfil
    Task<bool> TokenAindaEhValido(int idUsuario, string perfilDoToken);

    // Troca a senha temporária no primeiro acesso e devolve um novo token
    Task<ResultWrapper<LoginRespostaDto>> TrocarSenha(int idUsuario, TrocarSenhaDto dados);
}

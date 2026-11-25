using GestaoLogistica.Application.Comum;
using GestaoLogistica.Application.DTOs;
using GestaoLogistica.Application.Interfaces;
using GestaoLogistica.Domain.Interfaces;

namespace GestaoLogistica.Application.Servicos;

public class ServicoAutenticacao : IServicoAutenticacao
{
    private readonly IRepositorioUsuario _repositorioUsuario;
    private readonly ICriptografiaSenha _criptografiaSenha;
    private readonly IGeradorToken _geradorToken;

    public ServicoAutenticacao(IRepositorioUsuario repositorioUsuario,
                               ICriptografiaSenha criptografiaSenha,
                               IGeradorToken geradorToken)
    {
        _repositorioUsuario = repositorioUsuario;
        _criptografiaSenha = criptografiaSenha;
        _geradorToken = geradorToken;
    }

    public async Task<ResultWrapper<LoginRespostaDto>> Entrar(LoginDto dados)
    {
        var usuario = await _repositorioUsuario.BuscarPorEmail(dados.Email.Trim());

        // Mensagem genérica de propósito: não revela se o e-mail existe
        if (usuario == null || !_criptografiaSenha.SenhaConfere(dados.Senha, usuario.SenhaHash))
            return ResultWrapper<LoginRespostaDto>.Falha("E-mail ou senha inválidos.", null, 401);

        if (!usuario.EstaAtivo())
            return ResultWrapper<LoginRespostaDto>.Falha("Usuário inativo. Contate o administrador.", null, 403);

        var resposta = new LoginRespostaDto
        {
            Token = _geradorToken.GerarToken(usuario),
            Usuario = UsuarioDto.Converter(usuario)
        };

        return ResultWrapper<LoginRespostaDto>.Ok(resposta, "Login realizado com sucesso.");
    }

    public async Task<bool> TokenAindaEhValido(int idUsuario, string perfilDoToken)
    {
        var usuario = await _repositorioUsuario.BuscarPorId(idUsuario);
        return usuario != null && usuario.EstaAtivo() && usuario.Perfil == perfilDoToken;
    }
}

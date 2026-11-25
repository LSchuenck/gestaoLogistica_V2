using GestaoLogistica.Application.Comum;
using GestaoLogistica.Application.DTOs;
using GestaoLogistica.Application.Interfaces;
using GestaoLogistica.Domain.Entidades;
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

        return ResultWrapper<LoginRespostaDto>.Ok(MontarResposta(usuario), "Login realizado com sucesso.");
    }

    public async Task<ResultWrapper<LoginRespostaDto>> TrocarSenha(int idUsuario, TrocarSenhaDto dados)
    {
        var usuario = await _repositorioUsuario.BuscarPorId(idUsuario);
        if (usuario == null)
            return ResultWrapper<LoginRespostaDto>.NaoEncontrado("Usuário não encontrado.");

        if (!usuario.TrocarSenha)
            return ResultWrapper<LoginRespostaDto>.Falha("A senha já foi trocada no primeiro acesso.");

        usuario.SenhaHash = _criptografiaSenha.GerarHash(dados.NovaSenha);
        usuario.TrocarSenha = false;
        await _repositorioUsuario.Atualizar(usuario);

        // O token antigo diz que a senha precisa ser trocada, então é preciso gerar outro
        return ResultWrapper<LoginRespostaDto>.Ok(MontarResposta(usuario), "Senha alterada com sucesso.");
    }

    public async Task<bool> TokenAindaEhValido(int idUsuario, string perfilDoToken)
    {
        var usuario = await _repositorioUsuario.BuscarPorId(idUsuario);
        return usuario != null && usuario.EstaAtivo() && usuario.Perfil == perfilDoToken;
    }

    private LoginRespostaDto MontarResposta(Usuario usuario)
    {
        return new LoginRespostaDto
        {
            Token = _geradorToken.GerarToken(usuario),
            Usuario = UsuarioDto.Converter(usuario)
        };
    }
}

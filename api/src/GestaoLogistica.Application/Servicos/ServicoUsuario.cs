using System.Security.Cryptography;
using GestaoLogistica.Application.Comum;
using GestaoLogistica.Application.DTOs;
using GestaoLogistica.Application.Interfaces;
using GestaoLogistica.Domain.Constantes;
using GestaoLogistica.Domain.Entidades;
using GestaoLogistica.Domain.Interfaces;

namespace GestaoLogistica.Application.Servicos;

public class ServicoUsuario : IServicoUsuario
{
    private const int TamanhoSenhaTemporaria = 10;
    private const string CaracteresSenha = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private const int TamanhoMinimoSenha = 6;

    private readonly IRepositorioUsuario _repositorio;
    private readonly ICriptografiaSenha _criptografiaSenha;
    private readonly IServicoEmail _servicoEmail;

    public ServicoUsuario(IRepositorioUsuario repositorio, ICriptografiaSenha criptografiaSenha, IServicoEmail servicoEmail)
    {
        _repositorio = repositorio;
        _criptografiaSenha = criptografiaSenha;
        _servicoEmail = servicoEmail;
    }

    public async Task<ResultWrapper<List<UsuarioDto>>> Listar()
    {
        var usuarios = await _repositorio.ListarTodos();
        var lista = usuarios.Select(UsuarioDto.Converter).ToList();
        return ResultWrapper<List<UsuarioDto>>.Ok(lista, "Usuários listados com sucesso.");
    }

    public async Task<ResultWrapper<UsuarioDto>> BuscarPorId(int id)
    {
        var usuario = await _repositorio.BuscarPorId(id);
        if (usuario == null)
            return ResultWrapper<UsuarioDto>.NaoEncontrado("Usuário não encontrado.");

        return ResultWrapper<UsuarioDto>.Ok(UsuarioDto.Converter(usuario), "Usuário encontrado.");
    }

    public async Task<ResultWrapper<UsuarioDto>> Cadastrar(SalvarUsuarioDto dados)
    {
        var erro = await ValidarDados(dados, 0);
        if (erro != null)
            return ResultWrapper<UsuarioDto>.Falha(erro);

        var senhaTemporaria = GerarSenhaTemporaria();
        var usuario = new Usuario
        {
            Nome = dados.Nome.Trim(),
            Email = dados.Email.Trim(),
            Perfil = dados.Perfil,
            SenhaHash = _criptografiaSenha.GerarHash(senhaTemporaria),
            TrocarSenha = true
        };

        await _repositorio.Adicionar(usuario);
        var resposta = UsuarioDto.Converter(usuario);

        if (await _servicoEmail.EnviarBoasVindas(usuario.Email, usuario.Nome, senhaTemporaria))
            return ResultWrapper<UsuarioDto>.Criado(resposta, "Usuário cadastrado. Senha enviada por e-mail.");

        // Se o e-mail falhar, o administrador vê a senha para repassar ao usuário
        resposta.SenhaTemporaria = senhaTemporaria;
        return ResultWrapper<UsuarioDto>.Criado(resposta, $"Usuário cadastrado, mas o e-mail não foi enviado. Senha temporária: {senhaTemporaria}");
    }

    public async Task<ResultWrapper<UsuarioDto>> Atualizar(int id, SalvarUsuarioDto dados)
    {
        var usuario = await _repositorio.BuscarPorId(id);
        if (usuario == null)
            return ResultWrapper<UsuarioDto>.NaoEncontrado("Usuário não encontrado.");

        var erro = await ValidarDados(dados, id);
        if (erro != null)
            return ResultWrapper<UsuarioDto>.Falha(erro);

        if (usuario.EhAdministradorMaster() && dados.Perfil != PerfisUsuario.Administrador)
            return ResultWrapper<UsuarioDto>.Falha("O perfil do administrador master não pode ser alterado.");

        usuario.Nome = dados.Nome.Trim();
        usuario.Email = dados.Email.Trim();
        usuario.Perfil = dados.Perfil;

        // Senha só é alterada quando uma nova for informada
        if (!string.IsNullOrWhiteSpace(dados.Senha))
        {
            if (dados.Senha.Length < TamanhoMinimoSenha)
                return ResultWrapper<UsuarioDto>.Falha($"A senha deve ter no mínimo {TamanhoMinimoSenha} caracteres.");

            usuario.SenhaHash = _criptografiaSenha.GerarHash(dados.Senha);
        }

        await _repositorio.Atualizar(usuario);
        return ResultWrapper<UsuarioDto>.Ok(UsuarioDto.Converter(usuario), "Usuário atualizado com sucesso.");
    }

    public async Task<ResultWrapper<UsuarioDto>> AlternarStatus(int id, int idUsuarioLogado)
    {
        var usuario = await _repositorio.BuscarPorId(id);
        if (usuario == null)
            return ResultWrapper<UsuarioDto>.NaoEncontrado("Usuário não encontrado.");

        if (usuario.EhAdministradorMaster())
            return ResultWrapper<UsuarioDto>.Falha("Essa conta não pode ser desativada!");

        if (usuario.Id == idUsuarioLogado)
            return ResultWrapper<UsuarioDto>.Falha("Você não pode desativar sua própria conta.");

        usuario.AlternarStatus();
        await _repositorio.Atualizar(usuario);
        return ResultWrapper<UsuarioDto>.Ok(UsuarioDto.Converter(usuario), $"Usuário agora está {usuario.Status}.");
    }

    public async Task<ResultWrapper<object>> Excluir(int id, int idUsuarioLogado)
    {
        var usuario = await _repositorio.BuscarPorId(id);
        if (usuario == null)
            return ResultWrapper<object>.NaoEncontrado("Usuário não encontrado.");

        if (usuario.EhAdministradorMaster())
            return ResultWrapper<object>.Falha("Essa conta não pode ser excluída!");

        if (usuario.Id == idUsuarioLogado)
            return ResultWrapper<object>.Falha("Você não pode excluir sua própria conta.");

        if (await _repositorio.PossuiMovimentacoes(id))
            return ResultWrapper<object>.Falha("Não é possível excluir: o usuário possui movimentações de estoque. Desative-o se necessário.");

        await _repositorio.Remover(usuario);
        return ResultWrapper<object>.Ok(null, "Usuário excluído com sucesso.");
    }

    // Regras comuns ao cadastro e à alteração. Retorna a mensagem de erro ou null se estiver tudo certo.
    private async Task<string?> ValidarDados(SalvarUsuarioDto dados, int idUsuario)
    {
        if (!PerfisUsuario.Todos.Contains(dados.Perfil))
            return "Perfil inválido. Use ADMIN, GERENTE ou OPERADOR.";

        if (await _repositorio.EmailJaCadastrado(dados.Email.Trim(), idUsuario))
            return "E-mail já cadastrado.";

        return null;
    }

    private static string GerarSenhaTemporaria()
    {
        return RandomNumberGenerator.GetString(CaracteresSenha, TamanhoSenhaTemporaria);
    }
}

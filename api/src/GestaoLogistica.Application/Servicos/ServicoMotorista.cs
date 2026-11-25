using GestaoLogistica.Application.Comum;
using GestaoLogistica.Application.DTOs;
using GestaoLogistica.Application.Interfaces;
using GestaoLogistica.Domain.Entidades;
using GestaoLogistica.Domain.Interfaces;

namespace GestaoLogistica.Application.Servicos;

public class ServicoMotorista : IServicoMotorista
{
    private readonly IRepositorioMotorista _repositorio;
    private readonly IRepositorioTransportadora _repositorioTransportadora;
    private readonly IRepositorioViagem _repositorioViagem;

    public ServicoMotorista(IRepositorioMotorista repositorio, IRepositorioTransportadora repositorioTransportadora, IRepositorioViagem repositorioViagem)
    {
        _repositorio = repositorio;
        _repositorioTransportadora = repositorioTransportadora;
        _repositorioViagem = repositorioViagem;
    }

    public async Task<ResultWrapper<List<MotoristaDto>>> Listar()
    {
        var motoristas = await _repositorio.ListarTodos();
        var lista = motoristas.Select(MotoristaDto.Converter).ToList();
        return ResultWrapper<List<MotoristaDto>>.Ok(lista, "Motoristas listados com sucesso.");
    }

    public async Task<ResultWrapper<MotoristaDto>> BuscarPorId(int id)
    {
        var motorista = await _repositorio.BuscarPorId(id);
        if (motorista == null)
            return ResultWrapper<MotoristaDto>.NaoEncontrado("Motorista não encontrado.");

        return ResultWrapper<MotoristaDto>.Ok(MotoristaDto.Converter(motorista), "Motorista encontrado.");
    }

    public async Task<ResultWrapper<MotoristaDto>> Cadastrar(SalvarMotoristaDto dados)
    {
        var erro = await ValidarDados(dados, 0);
        if (erro != null)
            return ResultWrapper<MotoristaDto>.Falha(erro);

        var motorista = new Motorista();
        PreencherDados(motorista, dados);
        await _repositorio.Adicionar(motorista);

        // Busca novamente para trazer o nome da transportadora
        var salvo = await _repositorio.BuscarPorId(motorista.Id);
        return ResultWrapper<MotoristaDto>.Criado(MotoristaDto.Converter(salvo!), "Motorista cadastrado com sucesso!");
    }

    public async Task<ResultWrapper<MotoristaDto>> Atualizar(int id, SalvarMotoristaDto dados)
    {
        var motorista = await _repositorio.BuscarPorId(id);
        if (motorista == null)
            return ResultWrapper<MotoristaDto>.NaoEncontrado("Motorista não encontrado.");

        var erro = await ValidarDados(dados, id);
        if (erro != null)
            return ResultWrapper<MotoristaDto>.Falha(erro);

        PreencherDados(motorista, dados);
        await _repositorio.Atualizar(motorista);

        var salvo = await _repositorio.BuscarPorId(id);
        return ResultWrapper<MotoristaDto>.Ok(MotoristaDto.Converter(salvo!), "Motorista atualizado com sucesso!");
    }

    public async Task<ResultWrapper<object>> Excluir(int id)
    {
        var motorista = await _repositorio.BuscarPorId(id);
        if (motorista == null)
            return ResultWrapper<object>.NaoEncontrado("Motorista não encontrado.");

        if (await _repositorioViagem.MotoristaPossuiViagens(id))
            return ResultWrapper<object>.Falha("Não é possível excluir: o motorista possui viagens registradas.");

        await _repositorio.Remover(motorista);
        return ResultWrapper<object>.Ok(null, "Motorista removido com sucesso!");
    }

    // Regras: transportadora precisa existir e estar ATIVA; CPF e CNH não podem se repetir
    private async Task<string?> ValidarDados(SalvarMotoristaDto dados, int idMotorista)
    {
        var transportadora = await _repositorioTransportadora.BuscarPorId(dados.TransportadoraId);
        if (transportadora == null)
            return "Transportadora não encontrada.";

        if (!transportadora.EstaAtiva())
            return "A transportadora selecionada está inativa.";

        if (await _repositorio.CpfOuCnhJaCadastrado(dados.Cpf, dados.Cnh.Trim(), idMotorista))
            return "CPF ou CNH já registrado.";

        return null;
    }

    private static void PreencherDados(Motorista motorista, SalvarMotoristaDto dados)
    {
        motorista.TransportadoraId = dados.TransportadoraId;
        motorista.Nome = dados.Nome.Trim();
        motorista.Cpf = string.IsNullOrWhiteSpace(dados.Cpf) ? null : dados.Cpf.Trim();
        motorista.Cnh = dados.Cnh.Trim();
        motorista.CategoriaCnh = dados.CategoriaCnh;
        motorista.ValidadeCnh = dados.ValidadeCnh;
        motorista.Telefone = dados.Telefone;
    }
}

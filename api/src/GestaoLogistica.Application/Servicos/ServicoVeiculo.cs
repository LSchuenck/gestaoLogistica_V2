using GestaoLogistica.Application.Comum;
using GestaoLogistica.Application.DTOs;
using GestaoLogistica.Application.Interfaces;
using GestaoLogistica.Domain.Constantes;
using GestaoLogistica.Domain.Entidades;
using GestaoLogistica.Domain.Interfaces;

namespace GestaoLogistica.Application.Servicos;

public class ServicoVeiculo : IServicoVeiculo
{
    private readonly IRepositorioVeiculo _repositorio;
    private readonly IRepositorioTransportadora _repositorioTransportadora;

    public ServicoVeiculo(IRepositorioVeiculo repositorio, IRepositorioTransportadora repositorioTransportadora)
    {
        _repositorio = repositorio;
        _repositorioTransportadora = repositorioTransportadora;
    }

    public async Task<ResultWrapper<List<VeiculoDto>>> Listar()
    {
        var veiculos = await _repositorio.ListarTodos();
        var lista = veiculos.Select(VeiculoDto.Converter).ToList();
        return ResultWrapper<List<VeiculoDto>>.Ok(lista, "Veículos listados com sucesso.");
    }

    public async Task<ResultWrapper<VeiculoDto>> BuscarPorId(int id)
    {
        var veiculo = await _repositorio.BuscarPorId(id);
        if (veiculo == null)
            return ResultWrapper<VeiculoDto>.NaoEncontrado("Veículo não encontrado.");

        return ResultWrapper<VeiculoDto>.Ok(VeiculoDto.Converter(veiculo), "Veículo encontrado.");
    }

    public async Task<ResultWrapper<VeiculoDto>> Cadastrar(SalvarVeiculoDto dados)
    {
        var erro = await ValidarDados(dados, 0);
        if (erro != null)
            return ResultWrapper<VeiculoDto>.Falha(erro);

        var veiculo = new Veiculo();
        PreencherDados(veiculo, dados);
        await _repositorio.Adicionar(veiculo);

        var salvo = await _repositorio.BuscarPorId(veiculo.Id);
        return ResultWrapper<VeiculoDto>.Criado(VeiculoDto.Converter(salvo!), "Veículo cadastrado com sucesso!");
    }

    public async Task<ResultWrapper<VeiculoDto>> Atualizar(int id, SalvarVeiculoDto dados)
    {
        var veiculo = await _repositorio.BuscarPorId(id);
        if (veiculo == null)
            return ResultWrapper<VeiculoDto>.NaoEncontrado("Veículo não encontrado.");

        var erro = await ValidarDados(dados, id);
        if (erro != null)
            return ResultWrapper<VeiculoDto>.Falha(erro);

        PreencherDados(veiculo, dados);
        await _repositorio.Atualizar(veiculo);

        var salvo = await _repositorio.BuscarPorId(id);
        return ResultWrapper<VeiculoDto>.Ok(VeiculoDto.Converter(salvo!), "Veículo atualizado com sucesso!");
    }

    public async Task<ResultWrapper<VeiculoDto>> AlterarStatus(int id, string novoStatus)
    {
        // Só aceita os status conhecidos (DISPONIVEL, EM_VIAGEM, MANUTENCAO)
        if (!StatusVeiculo.Todos.Contains(novoStatus))
            return ResultWrapper<VeiculoDto>.Falha("Status inválido. Use DISPONIVEL, EM_VIAGEM ou MANUTENCAO.");

        var veiculo = await _repositorio.BuscarPorId(id);
        if (veiculo == null)
            return ResultWrapper<VeiculoDto>.NaoEncontrado("Veículo não encontrado.");

        veiculo.Status = novoStatus;
        await _repositorio.Atualizar(veiculo);

        return ResultWrapper<VeiculoDto>.Ok(VeiculoDto.Converter(veiculo), "Status do veículo atualizado com sucesso!");
    }

    public async Task<ResultWrapper<object>> Excluir(int id)
    {
        var veiculo = await _repositorio.BuscarPorId(id);
        if (veiculo == null)
            return ResultWrapper<object>.NaoEncontrado("Veículo não encontrado.");

        if (veiculo.Status == StatusVeiculo.EmViagem)
            return ResultWrapper<object>.Falha("Não é possível excluir: o veículo está em viagem.");

        await _repositorio.Remover(veiculo);
        return ResultWrapper<object>.Ok(null, "Veículo removido com sucesso!");
    }

    // Regras: transportadora precisa existir e estar ATIVA; a placa não pode se repetir
    private async Task<string?> ValidarDados(SalvarVeiculoDto dados, int idVeiculo)
    {
        var transportadora = await _repositorioTransportadora.BuscarPorId(dados.TransportadoraId);
        if (transportadora == null)
            return "Transportadora não encontrada.";

        if (!transportadora.EstaAtiva())
            return "A transportadora selecionada está inativa.";

        if (await _repositorio.PlacaJaCadastrada(dados.Placa.Trim().ToUpper(), idVeiculo))
            return "Placa já registrada.";

        return null;
    }

    private static void PreencherDados(Veiculo veiculo, SalvarVeiculoDto dados)
    {
        veiculo.TransportadoraId = dados.TransportadoraId;
        veiculo.Placa = dados.Placa.Trim().ToUpper();
        veiculo.Modelo = dados.Modelo;
        veiculo.TipoVeiculo = dados.TipoVeiculo;
        veiculo.CapacidadeCarga = dados.CapacidadeCarga;
    }
}

using GestaoLogistica.Application.Comum;
using GestaoLogistica.Application.DTOs;
using GestaoLogistica.Application.Interfaces;
using GestaoLogistica.Domain.Entidades;
using GestaoLogistica.Domain.Interfaces;

namespace GestaoLogistica.Application.Servicos;

public class ServicoCliente : IServicoCliente
{
    private readonly IRepositorioCliente _repositorio;

    public ServicoCliente(IRepositorioCliente repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<ResultWrapper<List<ClienteDto>>> Listar()
    {
        var clientes = await _repositorio.ListarTodos();
        var lista = clientes.Select(ClienteDto.Converter).ToList();
        return ResultWrapper<List<ClienteDto>>.Ok(lista, "Clientes listados com sucesso.");
    }

    public async Task<ResultWrapper<ClienteDto>> BuscarPorId(int id)
    {
        var cliente = await _repositorio.BuscarPorId(id);
        if (cliente == null)
            return ResultWrapper<ClienteDto>.NaoEncontrado("Cliente não encontrado.");

        return ResultWrapper<ClienteDto>.Ok(ClienteDto.Converter(cliente), "Cliente encontrado.");
    }

    public async Task<ResultWrapper<ClienteDto>> Cadastrar(SalvarClienteDto dados)
    {
        var cliente = new Cliente();
        PreencherDados(cliente, dados);

        await _repositorio.Adicionar(cliente);
        return ResultWrapper<ClienteDto>.Criado(ClienteDto.Converter(cliente), "Cliente cadastrado com sucesso!");
    }

    public async Task<ResultWrapper<ClienteDto>> Atualizar(int id, SalvarClienteDto dados)
    {
        var cliente = await _repositorio.BuscarPorId(id);
        if (cliente == null)
            return ResultWrapper<ClienteDto>.NaoEncontrado("Cliente não encontrado.");

        PreencherDados(cliente, dados);

        await _repositorio.Atualizar(cliente);
        return ResultWrapper<ClienteDto>.Ok(ClienteDto.Converter(cliente), "Cliente atualizado com sucesso!");
    }

    public async Task<ResultWrapper<object>> Excluir(int id)
    {
        var cliente = await _repositorio.BuscarPorId(id);
        if (cliente == null)
            return ResultWrapper<object>.NaoEncontrado("Cliente não encontrado.");

        await _repositorio.Remover(cliente);
        return ResultWrapper<object>.Ok(null, "Cliente removido com sucesso!");
    }

    private static void PreencherDados(Cliente cliente, SalvarClienteDto dados)
    {
        cliente.Nome = dados.Nome.Trim();
        cliente.CpfCnpj = dados.CpfCnpj;
        cliente.Telefone = dados.Telefone;
        cliente.Endereco = dados.Endereco.ConverterParaEntidade();
    }
}

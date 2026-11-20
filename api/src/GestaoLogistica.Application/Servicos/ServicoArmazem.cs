using GestaoLogistica.Application.Comum;
using GestaoLogistica.Application.DTOs;
using GestaoLogistica.Application.Interfaces;
using GestaoLogistica.Domain.Entidades;
using GestaoLogistica.Domain.Interfaces;

namespace GestaoLogistica.Application.Servicos;

public class ServicoArmazem : IServicoArmazem
{
    private readonly IRepositorioArmazem _repositorio;

    public ServicoArmazem(IRepositorioArmazem repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<ResultWrapper<List<ArmazemDto>>> Listar()
    {
        var armazens = await _repositorio.ListarTodos();
        var lista = armazens.Select(ArmazemDto.Converter).ToList();
        return ResultWrapper<List<ArmazemDto>>.Ok(lista, "Armazéns listados com sucesso.");
    }

    public async Task<ResultWrapper<ArmazemDto>> BuscarPorId(int id)
    {
        var armazem = await _repositorio.BuscarPorId(id);
        if (armazem == null)
            return ResultWrapper<ArmazemDto>.NaoEncontrado("Armazém não encontrado.");

        return ResultWrapper<ArmazemDto>.Ok(ArmazemDto.Converter(armazem), "Armazém encontrado.");
    }

    public async Task<ResultWrapper<ArmazemDto>> Cadastrar(SalvarArmazemDto dados)
    {
        var armazem = new Armazem
        {
            Nome = dados.Nome.Trim(),
            Endereco = dados.Endereco.ConverterParaEntidade()
        };

        await _repositorio.Adicionar(armazem);
        return ResultWrapper<ArmazemDto>.Criado(ArmazemDto.Converter(armazem), "Armazém cadastrado com sucesso!");
    }

    public async Task<ResultWrapper<ArmazemDto>> Atualizar(int id, SalvarArmazemDto dados)
    {
        var armazem = await _repositorio.BuscarPorId(id);
        if (armazem == null)
            return ResultWrapper<ArmazemDto>.NaoEncontrado("Armazém não encontrado.");

        armazem.Nome = dados.Nome.Trim();
        armazem.Endereco = dados.Endereco.ConverterParaEntidade();

        await _repositorio.Atualizar(armazem);
        return ResultWrapper<ArmazemDto>.Ok(ArmazemDto.Converter(armazem), "Armazém atualizado com sucesso!");
    }

    public async Task<ResultWrapper<object>> Excluir(int id)
    {
        var armazem = await _repositorio.BuscarPorId(id);
        if (armazem == null)
            return ResultWrapper<object>.NaoEncontrado("Armazém não encontrado.");

        if (await _repositorio.PossuiEstoqueOuMovimentacoes(id))
            return ResultWrapper<object>.Falha("Não é possível excluir: existem produtos ou movimentações neste armazém.");

        await _repositorio.Remover(armazem);
        return ResultWrapper<object>.Ok(null, "Armazém removido com sucesso!");
    }
}

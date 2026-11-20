using GestaoLogistica.Application.Comum;
using GestaoLogistica.Application.DTOs;
using GestaoLogistica.Application.Interfaces;
using GestaoLogistica.Domain.Entidades;
using GestaoLogistica.Domain.Interfaces;

namespace GestaoLogistica.Application.Servicos;

public class ServicoProduto : IServicoProduto
{
    private readonly IRepositorioProduto _repositorio;

    public ServicoProduto(IRepositorioProduto repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<ResultWrapper<List<ProdutoDto>>> Listar()
    {
        var produtos = await _repositorio.ListarTodos();
        var lista = produtos.Select(ProdutoDto.Converter).ToList();
        return ResultWrapper<List<ProdutoDto>>.Ok(lista, "Produtos listados com sucesso.");
    }

    public async Task<ResultWrapper<ProdutoDto>> BuscarPorId(int id)
    {
        var produto = await _repositorio.BuscarPorId(id);
        if (produto == null)
            return ResultWrapper<ProdutoDto>.NaoEncontrado("Produto não encontrado.");

        return ResultWrapper<ProdutoDto>.Ok(ProdutoDto.Converter(produto), "Produto encontrado.");
    }

    public async Task<ResultWrapper<ProdutoDto>> Cadastrar(SalvarProdutoDto dados)
    {
        var produto = new Produto();
        PreencherDados(produto, dados);

        await _repositorio.Adicionar(produto);
        return ResultWrapper<ProdutoDto>.Criado(ProdutoDto.Converter(produto), "Produto cadastrado com sucesso!");
    }

    public async Task<ResultWrapper<ProdutoDto>> Atualizar(int id, SalvarProdutoDto dados)
    {
        var produto = await _repositorio.BuscarPorId(id);
        if (produto == null)
            return ResultWrapper<ProdutoDto>.NaoEncontrado("Produto não encontrado.");

        PreencherDados(produto, dados);

        await _repositorio.Atualizar(produto);
        return ResultWrapper<ProdutoDto>.Ok(ProdutoDto.Converter(produto), "Produto atualizado com sucesso!");
    }

    public async Task<ResultWrapper<object>> Excluir(int id)
    {
        var produto = await _repositorio.BuscarPorId(id);
        if (produto == null)
            return ResultWrapper<object>.NaoEncontrado("Produto não encontrado.");

        if (await _repositorio.PossuiEstoqueOuMovimentacoes(id))
            return ResultWrapper<object>.Falha("Não é possível excluir: produto possui saldo em estoque ou movimentações.");

        await _repositorio.Remover(produto);
        return ResultWrapper<object>.Ok(null, "Produto removido com sucesso!");
    }

    private static void PreencherDados(Produto produto, SalvarProdutoDto dados)
    {
        produto.Descricao = dados.Descricao.Trim();
        produto.Peso = dados.Peso;
        produto.Volume = dados.Volume;
        produto.Validade = dados.Validade;
    }
}

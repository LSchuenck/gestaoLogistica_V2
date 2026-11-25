using GestaoLogistica.Application.Comum;
using GestaoLogistica.Application.DTOs;
using GestaoLogistica.Application.Interfaces;
using GestaoLogistica.Domain.Constantes;
using GestaoLogistica.Domain.Entidades;
using GestaoLogistica.Domain.Interfaces;

namespace GestaoLogistica.Application.Servicos;

public class ServicoEstoque : IServicoEstoque
{
    private const int QuantidadeHistorico = 20;

    private readonly IRepositorioEstoque _repositorio;
    private readonly IRepositorioProduto _repositorioProduto;
    private readonly IRepositorioArmazem _repositorioArmazem;

    public ServicoEstoque(IRepositorioEstoque repositorio,
                          IRepositorioProduto repositorioProduto,
                          IRepositorioArmazem repositorioArmazem)
    {
        _repositorio = repositorio;
        _repositorioProduto = repositorioProduto;
        _repositorioArmazem = repositorioArmazem;
    }

    public async Task<ResultWrapper<List<SaldoEstoqueDto>>> ListarSaldos()
    {
        var saldos = await _repositorio.ListarSaldos();
        var lista = saldos.Select(SaldoEstoqueDto.Converter).ToList();
        return ResultWrapper<List<SaldoEstoqueDto>>.Ok(lista, "Saldos de estoque listados com sucesso.");
    }

    public async Task<ResultWrapper<List<MovimentacaoDto>>> ListarUltimasMovimentacoes()
    {
        var movimentacoes = await _repositorio.ListarUltimasMovimentacoes(QuantidadeHistorico);
        var lista = movimentacoes.Select(MovimentacaoDto.Converter).ToList();
        return ResultWrapper<List<MovimentacaoDto>>.Ok(lista, "Histórico de movimentações listado com sucesso.");
    }

    // Registra uma ENTRADA (aumenta o saldo) ou uma SAIDA (diminui o saldo) de um produto em um armazém
    public async Task<ResultWrapper<SaldoEstoqueDto>> RegistrarMovimentacao(RegistrarMovimentacaoDto dados, int idUsuarioLogado)
    {
        return await _repositorio.ExecutarComProdutoTravado(dados.ProdutoId,
            () => AplicarMovimentacao(dados, idUsuarioLogado));
    }

    // Confere as regras, atualiza o saldo e grava o histórico
    private async Task<ResultWrapper<SaldoEstoqueDto>> AplicarMovimentacao(RegistrarMovimentacaoDto dados, int idUsuarioLogado)
    {
        if (!TipoMovimentacao.Todos.Contains(dados.Tipo))
            return ResultWrapper<SaldoEstoqueDto>.Falha("Tipo inválido. Use ENTRADA ou SAIDA.");

        var produto = await _repositorioProduto.BuscarPorId(dados.ProdutoId);
        if (produto == null)
            return ResultWrapper<SaldoEstoqueDto>.NaoEncontrado("Produto não encontrado.");

        var armazem = await _repositorioArmazem.BuscarPorId(dados.ArmazemId);
        if (armazem == null)
            return ResultWrapper<SaldoEstoqueDto>.NaoEncontrado("Armazém não encontrado.");

        // Se o produto ainda não tem saldo neste armazém, começa com zero
        var estoque = await _repositorio.BuscarSaldo(dados.ProdutoId, dados.ArmazemId)
                      ?? new Estoque { ProdutoId = produto.Id, ArmazemId = armazem.Id, Quantidade = 0 };

        if (dados.Tipo == TipoMovimentacao.Saida)
        {
            // Regra: não deixar o estoque ficar negativo
            if (!estoque.TemSaldoSuficiente(dados.Quantidade))
                return ResultWrapper<SaldoEstoqueDto>.Falha($"Saldo insuficiente neste armazém. Disponível: {estoque.Quantidade} unidades.");

            estoque.DarSaida(dados.Quantidade);
        }
        else
        {
            estoque.DarEntrada(dados.Quantidade);
        }

        var movimentacao = new MovimentacaoEstoque
        {
            ProdutoId = produto.Id,
            ArmazemId = armazem.Id,
            UsuarioId = idUsuarioLogado,
            Tipo = dados.Tipo,
            Quantidade = dados.Quantidade
        };

        // Saldo e histórico são gravados juntos
        await _repositorio.SalvarMovimentacao(estoque, movimentacao);

        estoque.Produto = produto;
        estoque.Armazem = armazem;
        return ResultWrapper<SaldoEstoqueDto>.Ok(SaldoEstoqueDto.Converter(estoque), "Movimentação registrada com sucesso!", 201);
    }
}

using GestaoLogistica.Domain.Entidades;
using GestaoLogistica.Domain.Interfaces;
using GestaoLogistica.Infrastructure.Dados;
using Microsoft.EntityFrameworkCore;

namespace GestaoLogistica.Infrastructure.Repositorios;

public class RepositorioEstoque : IRepositorioEstoque
{
    private readonly ContextoBanco _contexto;

    public RepositorioEstoque(ContextoBanco contexto)
    {
        _contexto = contexto;
    }

    public async Task<List<Estoque>> ListarSaldos()
    {
        return await _contexto.Estoques
            .Include(e => e.Produto)
            .Include(e => e.Armazem)
            .Where(e => e.Quantidade > 0)
            .OrderBy(e => e.Produto!.Descricao)
            .ThenBy(e => e.Armazem!.Nome)
            .ToListAsync();
    }

    public async Task<Estoque?> BuscarSaldo(int idProduto, int idArmazem)
    {
        return await _contexto.Estoques.FirstOrDefaultAsync(e => e.ProdutoId == idProduto && e.ArmazemId == idArmazem);
    }

    public async Task<List<MovimentacaoEstoque>> ListarUltimasMovimentacoes(int quantidade)
    {
        return await _contexto.MovimentacoesEstoque
            .Include(m => m.Produto)
            .Include(m => m.Armazem)
            .Include(m => m.Usuario)
            .OrderByDescending(m => m.DataMovimentacao)
            .Take(quantidade)
            .ToListAsync();
    }

    // Um único SaveChanges grava o saldo e o histórico na mesma transação
    public async Task SalvarMovimentacao(Estoque estoque, MovimentacaoEstoque movimentacao)
    {
        if (estoque.Id == 0)
            _contexto.Estoques.Add(estoque);

        _contexto.MovimentacoesEstoque.Add(movimentacao);
        await _contexto.SaveChangesAsync();
    }

    public async Task<T> ExecutarComProdutoTravado<T>(int idProduto, Func<Task<T>> operacao)
    {
        await using var transacao = await _contexto.Database.BeginTransactionAsync();

        // FOR UPDATE trava a linha do produto até o fim da transação
        await _contexto.Database.ExecuteSqlAsync($"SELECT Id FROM Produtos WHERE Id = {idProduto} FOR UPDATE");

        var resultado = await operacao();
        await transacao.CommitAsync();
        return resultado;
    }
}

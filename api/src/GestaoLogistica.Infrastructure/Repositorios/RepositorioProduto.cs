using GestaoLogistica.Domain.Entidades;
using GestaoLogistica.Domain.Interfaces;
using GestaoLogistica.Infrastructure.Dados;
using Microsoft.EntityFrameworkCore;

namespace GestaoLogistica.Infrastructure.Repositorios;

public class RepositorioProduto : RepositorioBase<Produto>, IRepositorioProduto
{
    public RepositorioProduto(ContextoBanco contexto) : base(contexto) { }

    // Sempre traz os saldos para calcular a quantidade total do produto
    protected override IQueryable<Produto> Consulta() => _contexto.Produtos.Include(p => p.Estoques);

    public async Task<bool> PossuiEstoqueOuMovimentacoes(int idProduto)
    {
        return await _contexto.Estoques.AnyAsync(e => e.ProdutoId == idProduto && e.Quantidade > 0)
            || await _contexto.MovimentacoesEstoque.AnyAsync(m => m.ProdutoId == idProduto);
    }
}

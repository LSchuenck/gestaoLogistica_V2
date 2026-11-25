using GestaoLogistica.Domain.Entidades;
using GestaoLogistica.Domain.Interfaces;
using GestaoLogistica.Infrastructure.Dados;
using Microsoft.EntityFrameworkCore;

namespace GestaoLogistica.Infrastructure.Repositorios;

// Repositórios específicos: herdam o CRUD do RepositorioBase e acrescentam as consultas de cada módulo.

public class RepositorioUsuario : RepositorioBase<Usuario>, IRepositorioUsuario
{
    public RepositorioUsuario(ContextoBanco contexto) : base(contexto) { }

    public async Task<Usuario?> BuscarPorEmail(string email)
    {
        return await _contexto.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> EmailJaCadastrado(string email, int idIgnorado = 0)
    {
        return await _contexto.Usuarios.AnyAsync(u => u.Email == email && u.Id != idIgnorado);
    }

    public async Task<bool> PossuiMovimentacoes(int idUsuario)
    {
        return await _contexto.MovimentacoesEstoque.AnyAsync(m => m.UsuarioId == idUsuario);
    }
}

public class RepositorioTransportadora : RepositorioBase<Transportadora>, IRepositorioTransportadora
{
    public RepositorioTransportadora(ContextoBanco contexto) : base(contexto) { }

    public async Task<bool> CnpjJaCadastrado(string cnpj, int idIgnorado = 0)
    {
        return await _contexto.Transportadoras.AnyAsync(t => t.Cnpj == cnpj && t.Id != idIgnorado);
    }

    public async Task<bool> PossuiMotoristasOuVeiculos(int idTransportadora)
    {
        return await _contexto.Motoristas.AnyAsync(m => m.TransportadoraId == idTransportadora)
            || await _contexto.Veiculos.AnyAsync(v => v.TransportadoraId == idTransportadora);
    }
}

public class RepositorioMotorista : RepositorioBase<Motorista>, IRepositorioMotorista
{
    public RepositorioMotorista(ContextoBanco contexto) : base(contexto) { }

    protected override IQueryable<Motorista> Consulta() => _contexto.Motoristas.Include(m => m.Transportadora);

    public async Task<bool> CpfOuCnhJaCadastrado(string? cpf, string cnh, int idIgnorado = 0)
    {
        return await _contexto.Motoristas.AnyAsync(m =>
            m.Id != idIgnorado && (m.Cnh == cnh || (cpf != null && cpf != "" && m.Cpf == cpf)));
    }
}

public class RepositorioVeiculo : RepositorioBase<Veiculo>, IRepositorioVeiculo
{
    public RepositorioVeiculo(ContextoBanco contexto) : base(contexto) { }

    protected override IQueryable<Veiculo> Consulta() => _contexto.Veiculos.Include(v => v.Transportadora);

    public async Task<bool> PlacaJaCadastrada(string placa, int idIgnorado = 0)
    {
        return await _contexto.Veiculos.AnyAsync(v => v.Placa == placa && v.Id != idIgnorado);
    }
}

public class RepositorioCliente : RepositorioBase<Cliente>, IRepositorioCliente
{
    public RepositorioCliente(ContextoBanco contexto) : base(contexto) { }
}

public class RepositorioArmazem : RepositorioBase<Armazem>, IRepositorioArmazem
{
    public RepositorioArmazem(ContextoBanco contexto) : base(contexto) { }

    public async Task<bool> PossuiEstoqueOuMovimentacoes(int idArmazem)
    {
        return await _contexto.Estoques.AnyAsync(e => e.ArmazemId == idArmazem && e.Quantidade > 0)
            || await _contexto.MovimentacoesEstoque.AnyAsync(m => m.ArmazemId == idArmazem);
    }
}

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
}

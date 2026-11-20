using GestaoLogistica.Domain.Entidades;
using Microsoft.EntityFrameworkCore;

namespace GestaoLogistica.Infrastructure.Dados;

// Contexto do Entity Framework: representa o banco de dados MySQL
public class ContextoBanco : DbContext
{
    public ContextoBanco(DbContextOptions<ContextoBanco> opcoes) : base(opcoes) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Transportadora> Transportadoras => Set<Transportadora>();
    public DbSet<Motorista> Motoristas => Set<Motorista>();
    public DbSet<Veiculo> Veiculos => Set<Veiculo>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Armazem> Armazens => Set<Armazem>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Estoque> Estoques => Set<Estoque>();
    public DbSet<MovimentacaoEstoque> MovimentacoesEstoque => Set<MovimentacaoEstoque>();

    protected override void OnModelCreating(ModelBuilder modelo)
    {
        // Usuário: e-mail único
        modelo.Entity<Usuario>().HasIndex(u => u.Email).IsUnique();

        // Transportadora: CNPJ único e endereço gravado na mesma tabela
        modelo.Entity<Transportadora>().HasIndex(t => t.Cnpj).IsUnique();
        modelo.Entity<Transportadora>().OwnsOne(t => t.Endereco);
        modelo.Entity<Transportadora>().Navigation(t => t.Endereco).IsRequired();

        // Motorista: CPF e CNH únicos; não deixa apagar a transportadora se tiver motorista
        modelo.Entity<Motorista>().HasIndex(m => m.Cpf).IsUnique();
        modelo.Entity<Motorista>().HasIndex(m => m.Cnh).IsUnique();
        modelo.Entity<Motorista>()
            .HasOne(m => m.Transportadora).WithMany()
            .HasForeignKey(m => m.TransportadoraId).OnDelete(DeleteBehavior.Restrict);

        // Veículo: placa única; não deixa apagar a transportadora se tiver veículo
        modelo.Entity<Veiculo>().HasIndex(v => v.Placa).IsUnique();
        modelo.Entity<Veiculo>().Property(v => v.CapacidadeCarga).HasPrecision(10, 2);
        modelo.Entity<Veiculo>()
            .HasOne(v => v.Transportadora).WithMany()
            .HasForeignKey(v => v.TransportadoraId).OnDelete(DeleteBehavior.Restrict);

        // Cliente e Armazém: endereço gravado na mesma tabela
        modelo.Entity<Cliente>().OwnsOne(c => c.Endereco);
        modelo.Entity<Cliente>().Navigation(c => c.Endereco).IsRequired();
        modelo.Entity<Armazem>().OwnsOne(a => a.Endereco);
        modelo.Entity<Armazem>().Navigation(a => a.Endereco).IsRequired();

        // Produto
        modelo.Entity<Produto>().Property(p => p.Peso).HasPrecision(10, 2);
        modelo.Entity<Produto>().Property(p => p.Volume).HasPrecision(10, 2);

        // Estoque: um único registro por produto + armazém
        modelo.Entity<Estoque>().HasIndex(e => new { e.ProdutoId, e.ArmazemId }).IsUnique();
        modelo.Entity<Estoque>()
            .HasOne(e => e.Produto).WithMany(p => p.Estoques)
            .HasForeignKey(e => e.ProdutoId).OnDelete(DeleteBehavior.Cascade);
        modelo.Entity<Estoque>()
            .HasOne(e => e.Armazem).WithMany()
            .HasForeignKey(e => e.ArmazemId).OnDelete(DeleteBehavior.Cascade);

        // Movimentações: o histórico impede apagar produto, armazém ou usuário ligados a ele
        modelo.Entity<MovimentacaoEstoque>()
            .HasOne(m => m.Produto).WithMany()
            .HasForeignKey(m => m.ProdutoId).OnDelete(DeleteBehavior.Restrict);
        modelo.Entity<MovimentacaoEstoque>()
            .HasOne(m => m.Armazem).WithMany()
            .HasForeignKey(m => m.ArmazemId).OnDelete(DeleteBehavior.Restrict);
        modelo.Entity<MovimentacaoEstoque>()
            .HasOne(m => m.Usuario).WithMany()
            .HasForeignKey(m => m.UsuarioId).OnDelete(DeleteBehavior.Restrict);
    }
}

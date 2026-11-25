using GestaoLogistica.Domain.Constantes;
using GestaoLogistica.Domain.Entidades;
using GestaoLogistica.Domain.Interfaces;
using GestaoLogistica.Infrastructure.Dados;
using Microsoft.EntityFrameworkCore;

namespace GestaoLogistica.Infrastructure.Repositorios;

public class RepositorioViagem : RepositorioBase<Viagem>, IRepositorioViagem
{
    public RepositorioViagem(ContextoBanco contexto) : base(contexto) { }

    protected override IQueryable<Viagem> Consulta()
    {
        return _contexto.Viagens
            .Include(v => v.Armazem)
            .Include(v => v.Cliente)
            .Include(v => v.Veiculo)
            .Include(v => v.Motorista)
            .Include(v => v.Produto);
    }

    public async Task<bool> VeiculoEstaEmViagem(int idVeiculo)
    {
        return await EmAndamento().AnyAsync(v => v.VeiculoId == idVeiculo);
    }

    public async Task<bool> MotoristaEstaEmViagem(int idMotorista)
    {
        return await EmAndamento().AnyAsync(v => v.MotoristaId == idMotorista);
    }

    public async Task<bool> VeiculoPossuiViagens(int idVeiculo)
    {
        return await _contexto.Viagens.AnyAsync(v => v.VeiculoId == idVeiculo);
    }

    public async Task<bool> MotoristaPossuiViagens(int idMotorista)
    {
        return await _contexto.Viagens.AnyAsync(v => v.MotoristaId == idMotorista);
    }

    public async Task<bool> ClientePossuiViagens(int idCliente)
    {
        return await _contexto.Viagens.AnyAsync(v => v.ClienteId == idCliente);
    }

    public async Task<bool> ArmazemPossuiViagens(int idArmazem)
    {
        return await _contexto.Viagens.AnyAsync(v => v.ArmazemId == idArmazem);
    }

    public async Task<T> ExecutarComTrava<T>(int idMotorista, int idVeiculo, int idProduto, Func<Task<T>> operacao)
    {
        await using var transacao = await _contexto.Database.BeginTransactionAsync();
        await _contexto.Database.ExecuteSqlAsync($"SELECT Id FROM Motoristas WHERE Id = {idMotorista} FOR UPDATE");
        await _contexto.Database.ExecuteSqlAsync($"SELECT Id FROM Veiculos WHERE Id = {idVeiculo} FOR UPDATE");
        await _contexto.Database.ExecuteSqlAsync($"SELECT Id FROM Produtos WHERE Id = {idProduto} FOR UPDATE");

        var resultado = await operacao();
        await transacao.CommitAsync();
        return resultado;
    }

    public async Task<T> ExecutarComViagemTravada<T>(int idViagem, Func<Task<T>> operacao)
    {
        await using var transacao = await _contexto.Database.BeginTransactionAsync();

        // Trava na ordem viagem, veículo e produto (STRAIGHT_JOIN mantém essa ordem).
        // O veículo vem sempre antes do produto, como em ExecutarComTrava, para uma operação
        // não ficar esperando a outra para sempre.
        await _contexto.Database.ExecuteSqlAsync($@"
            SELECT STRAIGHT_JOIN v.Id
            FROM Viagens v
            JOIN Veiculos c ON c.Id = v.VeiculoId
            LEFT JOIN Produtos p ON p.Id = v.ProdutoId
            WHERE v.Id = {idViagem}
            FOR UPDATE");

        var resultado = await operacao();
        await transacao.CommitAsync();
        return resultado;
    }

    private IQueryable<Viagem> EmAndamento()
    {
        return _contexto.Viagens.Where(v => v.Status == StatusViagem.Iniciada || v.Status == StatusViagem.EmTransito);
    }
}

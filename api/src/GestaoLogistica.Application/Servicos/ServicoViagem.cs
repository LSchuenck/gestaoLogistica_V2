using GestaoLogistica.Application.Comum;
using GestaoLogistica.Application.DTOs;
using GestaoLogistica.Application.Interfaces;
using GestaoLogistica.Domain.Constantes;
using GestaoLogistica.Domain.Entidades;
using GestaoLogistica.Domain.Interfaces;

namespace GestaoLogistica.Application.Servicos;

public class ServicoViagem : IServicoViagem
{
    private readonly IRepositorioViagem _repositorio;
    private readonly IRepositorioArmazem _repositorioArmazem;
    private readonly IRepositorioCliente _repositorioCliente;
    private readonly IRepositorioVeiculo _repositorioVeiculo;
    private readonly IRepositorioMotorista _repositorioMotorista;
    private readonly IRepositorioProduto _repositorioProduto;
    private readonly IRepositorioEstoque _repositorioEstoque;

    public ServicoViagem(IRepositorioViagem repositorio,
                         IRepositorioArmazem repositorioArmazem,
                         IRepositorioCliente repositorioCliente,
                         IRepositorioVeiculo repositorioVeiculo,
                         IRepositorioMotorista repositorioMotorista,
                         IRepositorioProduto repositorioProduto,
                         IRepositorioEstoque repositorioEstoque)
    {
        _repositorio = repositorio;
        _repositorioArmazem = repositorioArmazem;
        _repositorioCliente = repositorioCliente;
        _repositorioVeiculo = repositorioVeiculo;
        _repositorioMotorista = repositorioMotorista;
        _repositorioProduto = repositorioProduto;
        _repositorioEstoque = repositorioEstoque;
    }

    public async Task<ResultWrapper<List<ViagemDto>>> Listar()
    {
        var viagens = await _repositorio.ListarTodos();
        var lista = viagens.Select(ViagemDto.Converter).ToList();
        return ResultWrapper<List<ViagemDto>>.Ok(lista, "Viagens listadas com sucesso.");
    }

    public async Task<ResultWrapper<ViagemDto>> Iniciar(IniciarViagemDto dados, int idUsuarioLogado)
    {
        return await _repositorio.ExecutarComTrava(dados.MotoristaId, dados.VeiculoId, dados.ProdutoId,
            () => ValidarEIniciar(dados, idUsuarioLogado));
    }

    private async Task<ResultWrapper<ViagemDto>> ValidarEIniciar(IniciarViagemDto dados, int idUsuarioLogado)
    {
        if (await _repositorioArmazem.BuscarPorId(dados.ArmazemId) == null)
            return ResultWrapper<ViagemDto>.NaoEncontrado("Armazém não encontrado.");

        if (await _repositorioCliente.BuscarPorId(dados.ClienteId) == null)
            return ResultWrapper<ViagemDto>.NaoEncontrado("Cliente não encontrado.");

        var veiculo = await _repositorioVeiculo.BuscarPorId(dados.VeiculoId);
        if (veiculo == null)
            return ResultWrapper<ViagemDto>.NaoEncontrado("Veículo não encontrado.");

        var motorista = await _repositorioMotorista.BuscarPorId(dados.MotoristaId);
        if (motorista == null)
            return ResultWrapper<ViagemDto>.NaoEncontrado("Motorista não encontrado.");

        var produto = await _repositorioProduto.BuscarPorId(dados.ProdutoId);
        if (produto == null)
            return ResultWrapper<ViagemDto>.NaoEncontrado("Produto não encontrado.");

        var erro = await ValidarRegras(veiculo, motorista);
        if (erro != null)
            return ResultWrapper<ViagemDto>.Falha(erro);

        var peso = produto.CalcularPeso(dados.Quantidade);
        if (!veiculo.SuportaCarga(peso))
            return ResultWrapper<ViagemDto>.Falha($"A carga ({peso:0.##} kg) passa da capacidade do veículo ({veiculo.CapacidadeCarga:0.##} kg).");

        var estoque = await _repositorioEstoque.BuscarSaldo(dados.ProdutoId, dados.ArmazemId);
        if (estoque == null || !estoque.TemSaldoSuficiente(dados.Quantidade))
            return ResultWrapper<ViagemDto>.Falha($"Saldo insuficiente no armazém de origem. Disponível: {estoque?.Quantidade ?? 0} unidades.");

        var viagem = new Viagem
        {
            ArmazemId = dados.ArmazemId,
            ClienteId = dados.ClienteId,
            VeiculoId = dados.VeiculoId,
            MotoristaId = dados.MotoristaId,
            ProdutoId = dados.ProdutoId,
            Quantidade = dados.Quantidade,
            DistanciaKm = Math.Round(dados.DistanciaKm, 2),
            ValorPorKm = Math.Round(dados.ValorPorKm, 2)
        };

        veiculo.Status = StatusVeiculo.EmViagem;
        await _repositorio.Adicionar(viagem);

        // O produto sai do estoque junto com o veículo
        estoque.DarSaida(viagem.Quantidade);
        await _repositorioEstoque.SalvarMovimentacao(estoque, NovaMovimentacao(viagem, TipoMovimentacao.Saida, idUsuarioLogado));

        var salva = await _repositorio.BuscarPorId(viagem.Id);
        return ResultWrapper<ViagemDto>.Criado(ViagemDto.Converter(salva!), "Viagem iniciada com sucesso!");
    }

    // Retorna a mensagem de erro ou null se estiver tudo certo
    private async Task<string?> ValidarRegras(Veiculo veiculo, Motorista motorista)
    {
        if (veiculo.Status != StatusVeiculo.Disponivel)
            return "O veículo não está disponível.";

        if (motorista.CnhEstaVencida())
            return "A CNH do motorista está vencida.";

        if (motorista.TransportadoraId != veiculo.TransportadoraId)
            return "O motorista e o veículo precisam ser da mesma transportadora.";

        if (!veiculo.Transportadora!.EstaAtiva())
            return "A transportadora está inativa.";

        if (await _repositorio.MotoristaEstaEmViagem(motorista.Id))
            return "O motorista já está em outra viagem.";

        return null;
    }

    public async Task<ResultWrapper<ViagemDto>> Avancar(int id)
    {
        return await _repositorio.ExecutarComViagemTravada(id, () => AplicarAvanco(id));
    }

    private async Task<ResultWrapper<ViagemDto>> AplicarAvanco(int id)
    {
        var viagem = await _repositorio.BuscarPorId(id);
        if (viagem == null)
            return ResultWrapper<ViagemDto>.NaoEncontrado("Viagem não encontrada.");

        if (!viagem.PodeAvancar())
            return ResultWrapper<ViagemDto>.Falha("Só é possível avançar uma viagem iniciada.");

        viagem.Avancar();
        await _repositorio.Atualizar(viagem);
        return ResultWrapper<ViagemDto>.Ok(ViagemDto.Converter(viagem), "Viagem em trânsito.");
    }

    public async Task<ResultWrapper<ViagemDto>> Concluir(int id)
    {
        return await _repositorio.ExecutarComViagemTravada(id, () => AplicarConclusao(id));
    }

    private async Task<ResultWrapper<ViagemDto>> AplicarConclusao(int id)
    {
        var viagem = await _repositorio.BuscarPorId(id);
        if (viagem == null)
            return ResultWrapper<ViagemDto>.NaoEncontrado("Viagem não encontrada.");

        if (!viagem.PodeConcluir())
            return ResultWrapper<ViagemDto>.Falha("Só é possível concluir uma viagem em trânsito.");

        viagem.Concluir();
        viagem.Veiculo!.Status = StatusVeiculo.Disponivel;
        await _repositorio.Atualizar(viagem);
        return ResultWrapper<ViagemDto>.Ok(ViagemDto.Converter(viagem), "Viagem concluída com sucesso!");
    }

    public async Task<ResultWrapper<ViagemDto>> Cancelar(int id, int idUsuarioLogado)
    {
        return await _repositorio.ExecutarComViagemTravada(id, () => AplicarCancelamento(id, idUsuarioLogado));
    }

    private async Task<ResultWrapper<ViagemDto>> AplicarCancelamento(int id, int idUsuarioLogado)
    {
        var viagem = await _repositorio.BuscarPorId(id);
        if (viagem == null)
            return ResultWrapper<ViagemDto>.NaoEncontrado("Viagem não encontrada.");

        if (!viagem.EstaEmAndamento())
            return ResultWrapper<ViagemDto>.Falha("Só é possível cancelar uma viagem em andamento.");

        viagem.Cancelar();
        viagem.Veiculo!.Status = StatusVeiculo.Disponivel;
        await _repositorio.Atualizar(viagem);

        await DevolverAoEstoque(viagem, idUsuarioLogado);
        return ResultWrapper<ViagemDto>.Ok(ViagemDto.Converter(viagem), "Viagem cancelada.");
    }

    private async Task DevolverAoEstoque(Viagem viagem, int idUsuarioLogado)
    {
        // Viagens antigas não têm produto
        if (viagem.ProdutoId == null)
            return;

        var estoque = await _repositorioEstoque.BuscarSaldo(viagem.ProdutoId.Value, viagem.ArmazemId);
        estoque!.DarEntrada(viagem.Quantidade);
        await _repositorioEstoque.SalvarMovimentacao(estoque, NovaMovimentacao(viagem, TipoMovimentacao.Entrada, idUsuarioLogado));
    }

    private static MovimentacaoEstoque NovaMovimentacao(Viagem viagem, string tipo, int idUsuarioLogado)
    {
        return new MovimentacaoEstoque
        {
            ProdutoId = viagem.ProdutoId!.Value,
            ArmazemId = viagem.ArmazemId,
            UsuarioId = idUsuarioLogado,
            Tipo = tipo,
            Quantidade = viagem.Quantidade
        };
    }
}

using GestaoLogistica.Application.Comum;
using GestaoLogistica.Application.DTOs;
using GestaoLogistica.Application.Interfaces;
using GestaoLogistica.Domain.Entidades;
using GestaoLogistica.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace GestaoLogistica.Application.Servicos;

// A lista de transportadoras usa CACHE: ela é consultada na tela de transportadoras
// e também nos formulários de motoristas e veículos, mas muda pouco.
public class ServicoTransportadora : IServicoTransportadora
{
    private const string ChaveCacheLista = "lista_transportadoras";
    private static readonly TimeSpan TempoExpiracaoCache = TimeSpan.FromMinutes(5);

    private readonly IRepositorioTransportadora _repositorio;
    private readonly IMemoryCache _cache;
    private readonly ILogger<ServicoTransportadora> _logger;

    public ServicoTransportadora(IRepositorioTransportadora repositorio,
                                 IMemoryCache cache,
                                 ILogger<ServicoTransportadora> logger)
    {
        _repositorio = repositorio;
        _cache = cache;
        _logger = logger;
    }

    public async Task<ResultWrapper<List<TransportadoraDto>>> Listar()
    {
        // 1) Tenta pegar a lista do cache
        if (_cache.TryGetValue(ChaveCacheLista, out List<TransportadoraDto>? listaEmCache) && listaEmCache != null)
        {
            _logger.LogInformation("[CACHE] Transportadoras recuperadas do CACHE.");
            return ResultWrapper<List<TransportadoraDto>>.Ok(listaEmCache, "Transportadoras recuperadas do cache.");
        }

        // 2) Não estava no cache: busca no banco de dados (fonte original)
        _logger.LogInformation("[CACHE] Transportadoras buscadas no BANCO DE DADOS.");
        var transportadoras = await _repositorio.ListarTodos();
        var lista = transportadoras.Select(TransportadoraDto.Converter).ToList();

        // 3) Guarda no cache com tempo de expiração
        _cache.Set(ChaveCacheLista, lista, TempoExpiracaoCache);
        _logger.LogInformation("[CACHE] Lista de transportadoras armazenada no cache por {Minutos} minutos.", TempoExpiracaoCache.TotalMinutes);

        return ResultWrapper<List<TransportadoraDto>>.Ok(lista, "Transportadoras buscadas no banco de dados.");
    }

    public async Task<ResultWrapper<TransportadoraDto>> BuscarPorId(int id)
    {
        var transportadora = await _repositorio.BuscarPorId(id);
        if (transportadora == null)
            return ResultWrapper<TransportadoraDto>.NaoEncontrado("Transportadora não encontrada.");

        return ResultWrapper<TransportadoraDto>.Ok(TransportadoraDto.Converter(transportadora), "Transportadora encontrada.");
    }

    public async Task<ResultWrapper<TransportadoraDto>> Cadastrar(SalvarTransportadoraDto dados)
    {
        if (await _repositorio.CnpjJaCadastrado(dados.Cnpj.Trim()))
            return ResultWrapper<TransportadoraDto>.Falha("CNPJ já cadastrado.");

        var transportadora = new Transportadora();
        PreencherDados(transportadora, dados);

        await _repositorio.Adicionar(transportadora);
        InvalidarCache();

        return ResultWrapper<TransportadoraDto>.Criado(TransportadoraDto.Converter(transportadora), "Transportadora cadastrada com sucesso!");
    }

    public async Task<ResultWrapper<TransportadoraDto>> Atualizar(int id, SalvarTransportadoraDto dados)
    {
        var transportadora = await _repositorio.BuscarPorId(id);
        if (transportadora == null)
            return ResultWrapper<TransportadoraDto>.NaoEncontrado("Transportadora não encontrada.");

        if (await _repositorio.CnpjJaCadastrado(dados.Cnpj.Trim(), id))
            return ResultWrapper<TransportadoraDto>.Falha("CNPJ já cadastrado.");

        PreencherDados(transportadora, dados);

        await _repositorio.Atualizar(transportadora);
        InvalidarCache();

        return ResultWrapper<TransportadoraDto>.Ok(TransportadoraDto.Converter(transportadora), "Transportadora atualizada com sucesso!");
    }

    public async Task<ResultWrapper<TransportadoraDto>> AlternarStatus(int id)
    {
        var transportadora = await _repositorio.BuscarPorId(id);
        if (transportadora == null)
            return ResultWrapper<TransportadoraDto>.NaoEncontrado("Transportadora não encontrada.");

        transportadora.AlternarStatus();

        await _repositorio.Atualizar(transportadora);
        InvalidarCache();

        return ResultWrapper<TransportadoraDto>.Ok(TransportadoraDto.Converter(transportadora), $"Transportadora agora está {transportadora.Status}.");
    }

    public async Task<ResultWrapper<object>> Excluir(int id)
    {
        var transportadora = await _repositorio.BuscarPorId(id);
        if (transportadora == null)
            return ResultWrapper<object>.NaoEncontrado("Transportadora não encontrada.");

        if (await _repositorio.PossuiMotoristasOuVeiculos(id))
            return ResultWrapper<object>.Falha("Não é possível excluir: existem motoristas ou veículos vinculados a esta transportadora.");

        await _repositorio.Remover(transportadora);
        InvalidarCache();

        return ResultWrapper<object>.Ok(null, "Transportadora removida com sucesso!");
    }

    // Sempre que os dados mudam, o cache é apagado para a próxima consulta buscar a versão nova no banco
    private void InvalidarCache()
    {
        _cache.Remove(ChaveCacheLista);
        _logger.LogInformation("[CACHE] Cache de transportadoras INVALIDADO (dados alterados).");
    }

    private static void PreencherDados(Transportadora transportadora, SalvarTransportadoraDto dados)
    {
        transportadora.Cnpj = dados.Cnpj.Trim();
        transportadora.RazaoSocial = dados.RazaoSocial.Trim();
        transportadora.NomeFantasia = dados.NomeFantasia.Trim();
        transportadora.Telefone = dados.Telefone;
        transportadora.Email = string.IsNullOrWhiteSpace(dados.Email) ? null : dados.Email.Trim();
        transportadora.Endereco = dados.Endereco.ConverterParaEntidade();
    }
}

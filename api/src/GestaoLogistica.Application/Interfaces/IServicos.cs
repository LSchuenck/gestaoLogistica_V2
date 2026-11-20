using GestaoLogistica.Application.Comum;
using GestaoLogistica.Application.DTOs;

namespace GestaoLogistica.Application.Interfaces;

// Contratos dos serviços (casos de uso) usados pelos controllers da API.

public interface IServicoAutenticacao
{
    Task<ResultWrapper<LoginRespostaDto>> Entrar(LoginDto dados);
}

public interface IServicoUsuario
{
    Task<ResultWrapper<List<UsuarioDto>>> Listar();
    Task<ResultWrapper<UsuarioDto>> BuscarPorId(int id);
    Task<ResultWrapper<UsuarioDto>> Cadastrar(SalvarUsuarioDto dados);
    Task<ResultWrapper<UsuarioDto>> Atualizar(int id, SalvarUsuarioDto dados);
    Task<ResultWrapper<UsuarioDto>> AlternarStatus(int id, int idUsuarioLogado);
    Task<ResultWrapper<object>> Excluir(int id, int idUsuarioLogado);
}

public interface IServicoTransportadora
{
    Task<ResultWrapper<List<TransportadoraDto>>> Listar();
    Task<ResultWrapper<TransportadoraDto>> BuscarPorId(int id);
    Task<ResultWrapper<TransportadoraDto>> Cadastrar(SalvarTransportadoraDto dados);
    Task<ResultWrapper<TransportadoraDto>> Atualizar(int id, SalvarTransportadoraDto dados);
    Task<ResultWrapper<TransportadoraDto>> AlternarStatus(int id);
    Task<ResultWrapper<object>> Excluir(int id);
}

public interface IServicoMotorista
{
    Task<ResultWrapper<List<MotoristaDto>>> Listar();
    Task<ResultWrapper<MotoristaDto>> BuscarPorId(int id);
    Task<ResultWrapper<MotoristaDto>> Cadastrar(SalvarMotoristaDto dados);
    Task<ResultWrapper<MotoristaDto>> Atualizar(int id, SalvarMotoristaDto dados);
    Task<ResultWrapper<object>> Excluir(int id);
}

public interface IServicoVeiculo
{
    Task<ResultWrapper<List<VeiculoDto>>> Listar();
    Task<ResultWrapper<VeiculoDto>> BuscarPorId(int id);
    Task<ResultWrapper<VeiculoDto>> Cadastrar(SalvarVeiculoDto dados);
    Task<ResultWrapper<VeiculoDto>> Atualizar(int id, SalvarVeiculoDto dados);
    Task<ResultWrapper<VeiculoDto>> AlterarStatus(int id, string novoStatus);
    Task<ResultWrapper<object>> Excluir(int id);
}

public interface IServicoCliente
{
    Task<ResultWrapper<List<ClienteDto>>> Listar();
    Task<ResultWrapper<ClienteDto>> BuscarPorId(int id);
    Task<ResultWrapper<ClienteDto>> Cadastrar(SalvarClienteDto dados);
    Task<ResultWrapper<ClienteDto>> Atualizar(int id, SalvarClienteDto dados);
    Task<ResultWrapper<object>> Excluir(int id);
}

public interface IServicoArmazem
{
    Task<ResultWrapper<List<ArmazemDto>>> Listar();
    Task<ResultWrapper<ArmazemDto>> BuscarPorId(int id);
    Task<ResultWrapper<ArmazemDto>> Cadastrar(SalvarArmazemDto dados);
    Task<ResultWrapper<ArmazemDto>> Atualizar(int id, SalvarArmazemDto dados);
    Task<ResultWrapper<object>> Excluir(int id);
}

public interface IServicoProduto
{
    Task<ResultWrapper<List<ProdutoDto>>> Listar();
    Task<ResultWrapper<ProdutoDto>> BuscarPorId(int id);
    Task<ResultWrapper<ProdutoDto>> Cadastrar(SalvarProdutoDto dados);
    Task<ResultWrapper<ProdutoDto>> Atualizar(int id, SalvarProdutoDto dados);
    Task<ResultWrapper<object>> Excluir(int id);
}

public interface IServicoEstoque
{
    Task<ResultWrapper<List<SaldoEstoqueDto>>> ListarSaldos();
    Task<ResultWrapper<List<MovimentacaoDto>>> ListarUltimasMovimentacoes();
    Task<ResultWrapper<SaldoEstoqueDto>> RegistrarMovimentacao(RegistrarMovimentacaoDto dados, int idUsuarioLogado);
}

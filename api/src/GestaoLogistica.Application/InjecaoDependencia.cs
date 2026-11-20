using GestaoLogistica.Application.Interfaces;
using GestaoLogistica.Application.Servicos;
using Microsoft.Extensions.DependencyInjection;

namespace GestaoLogistica.Application;

// Registra os serviços da camada Application na injeção de dependência
public static class InjecaoDependencia
{
    public static IServiceCollection AdicionarCamadaAplicacao(this IServiceCollection servicos)
    {
        servicos.AddMemoryCache();

        servicos.AddScoped<IServicoAutenticacao, ServicoAutenticacao>();
        servicos.AddScoped<IServicoUsuario, ServicoUsuario>();
        servicos.AddScoped<IServicoTransportadora, ServicoTransportadora>();
        servicos.AddScoped<IServicoMotorista, ServicoMotorista>();
        servicos.AddScoped<IServicoVeiculo, ServicoVeiculo>();
        servicos.AddScoped<IServicoCliente, ServicoCliente>();
        servicos.AddScoped<IServicoArmazem, ServicoArmazem>();
        servicos.AddScoped<IServicoProduto, ServicoProduto>();
        servicos.AddScoped<IServicoEstoque, ServicoEstoque>();

        return servicos;
    }
}

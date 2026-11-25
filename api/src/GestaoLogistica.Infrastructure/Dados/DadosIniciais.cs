using GestaoLogistica.Application.Interfaces;
using GestaoLogistica.Domain.Constantes;
using GestaoLogistica.Domain.Entidades;

namespace GestaoLogistica.Infrastructure.Dados;

// Cria o banco (se não existir) e insere os dados de exemplo do sistema original
public static class DadosIniciais
{
    public static void CriarBancoEPopular(ContextoBanco contexto, ICriptografiaSenha criptografiaSenha, string? senhaAdministrador)
    {
        contexto.Database.EnsureCreated();

        // Se já existe usuário, o banco já foi populado antes
        if (contexto.Usuarios.Any())
            return;

        if (string.IsNullOrWhiteSpace(senhaAdministrador) || senhaAdministrador.Length < 6)
            throw new InvalidOperationException("Configure 'AdministradorInicial:Senha' com pelo menos 6 caracteres (veja o README).");

        contexto.Usuarios.Add(new Usuario
        {
            Nome = "Administrador",
            Email = "admin@gestao.com",
            SenhaHash = criptografiaSenha.GerarHash(senhaAdministrador),
            Perfil = PerfisUsuario.Administrador
        });

        // Transportadoras
        var translog = NovaTransportadora("12.345.678/0001-90", "TransLog Sul Transportes Ltda", "TransLog Sul", "(51) 3210-4000", "contato@translogsul.com.br",
            NovoEndereco("90010-000", "Av. Farrapos", "1200", "Porto Alegre", "RS"));
        var rotaCerta = NovaTransportadora("23.456.789/0001-01", "Rota Certa Logística e Transporte", "Rota Certa", "(11) 3900-5500", "operacoes@rotacerta.com.br",
            NovoEndereco("01310-100", "Av. Paulista", "2500", "São Paulo", "SP"));
        var norteFrete = NovaTransportadora("34.567.890/0001-12", "NorteFrete Soluções Logísticas SA", "NorteFrete", "(91) 3222-8800", "frete@nortefrete.com.br",
            NovoEndereco("66010-000", "Av. Presidente Vargas", "800", "Belém", "PA"));
        contexto.Transportadoras.AddRange(translog, rotaCerta, norteFrete);

        // Motoristas (2 por transportadora)
        contexto.Motoristas.AddRange(
            NovoMotorista(translog, "Carlos Eduardo Souza", "111.222.333-44", "RS1234567", "E", "2027-08-15", "(51) 99801-1111"),
            NovoMotorista(translog, "Fernanda Lima Pereira", "222.333.444-55", "RS2345678", "C", "2026-03-20", "(51) 98702-2222"),
            NovoMotorista(rotaCerta, "Ricardo Alves Mendes", "333.444.555-66", "SP3456789", "E", "2028-11-10", "(11) 97603-3333"),
            NovoMotorista(rotaCerta, "Ana Paula Rodrigues", "444.555.666-77", "SP4567890", "D", "2027-05-30", "(11) 96504-4444"),
            NovoMotorista(norteFrete, "José Raimundo Costa", "555.666.777-88", "PA5678901", "E", "2026-09-25", "(91) 95405-5555"),
            NovoMotorista(norteFrete, "Mariana Oliveira Neto", "666.777.888-99", "PA6789012", "C", "2028-01-14", "(91) 94306-6666"));

        // Veículos (2 por transportadora)
        contexto.Veiculos.AddRange(
            NovoVeiculo(translog, "RSA-1A01", "Volvo FH 540", "Bitrem", 42000),
            NovoVeiculo(translog, "RSB-2B02", "Scania R450", "Carreta Baú", 28000),
            NovoVeiculo(rotaCerta, "SPC-3C03", "Mercedes Actros", "Carreta Graneleira", 35000),
            NovoVeiculo(rotaCerta, "SPD-4D04", "Volkswagen Meteor", "Truck Baú", 15000),
            NovoVeiculo(norteFrete, "PAE-5E05", "Iveco Stralis", "Carreta Frigorífica", 22000),
            NovoVeiculo(norteFrete, "PAF-6F06", "Ford Cargo 2429", "Truck Carga Seca", 12000));

        // Clientes
        contexto.Clientes.AddRange(
            NovoCliente("Grupo Mineração Horizonte", "10.111.222/0001-01", "(31) 3300-1000", NovoEndereco("30130-010", "Av. do Contorno", "500", "Belo Horizonte", "MG")),
            NovoCliente("Distribuidora Carioca Ltda", "20.222.333/0001-02", "(21) 2500-2000", NovoEndereco("20040-020", "Av. Rio Branco", "156", "Rio de Janeiro", "RJ")),
            NovoCliente("Atacadão Nordeste SA", "30.333.444/0001-03", "(71) 3100-3000", NovoEndereco("40020-010", "Av. Sete de Setembro", "1000", "Salvador", "BA")),
            NovoCliente("Redes Supermercados Fortaleza", "40.444.555/0001-04", "(85) 3400-4000", NovoEndereco("60135-000", "Av. Beira Mar", "3000", "Fortaleza", "CE")),
            NovoCliente("Construtora Paraná Ltda", "50.555.666/0001-05", "(41) 3200-5000", NovoEndereco("80010-100", "Rua XV de Novembro", "700", "Curitiba", "PR")),
            NovoCliente("Farmácias Pernambuco SA", "60.666.777/0001-06", "(81) 3300-6000", NovoEndereco("50010-010", "Av. Boa Viagem", "450", "Recife", "PE")),
            NovoCliente("Agro Centro-Oeste Ltda", "70.777.888/0001-07", "(62) 3500-7000", NovoEndereco("74010-010", "Av. Goiás", "1100", "Goiânia", "GO")),
            NovoCliente("Eletrônicos do Sul ME", "80.888.999/0001-08", "(48) 3600-8000", NovoEndereco("88010-000", "Av. Beira-Mar Norte", "220", "Florianópolis", "SC")));

        // Armazéns / Centros de Distribuição
        var cdSaoPaulo = new Armazem { Nome = "CD São Paulo", Endereco = NovoEndereco("04794-000", "Rod. dos Imigrantes", "km 4", "São Paulo", "SP") };
        var cdPortoAlegre = new Armazem { Nome = "CD Porto Alegre", Endereco = NovoEndereco("91020-000", "Av. Assis Brasil", "3500", "Porto Alegre", "RS") };
        var cdBrasilia = new Armazem { Nome = "CD Brasília", Endereco = NovoEndereco("71218-000", "SCIA Quadra 14", "s/n", "Brasília", "DF") };
        var cdCuritiba = new Armazem { Nome = "CD Curitiba", Endereco = NovoEndereco("81200-000", "Rod. BR-116", "km 98", "Curitiba", "PR") };
        contexto.Armazens.AddRange(cdSaoPaulo, cdPortoAlegre, cdBrasilia, cdCuritiba);

        // Produtos
        var notebook = NovoProduto("Notebook Dell Inspiron 15", 2.50m, 0.01m, null);
        var monitor = NovoProduto("Monitor LG UltraWide 29\"", 4.80m, 0.03m, null);
        var teclado = NovoProduto("Teclado Mecânico Redragon", 1.20m, 0.01m, null);
        var mouse = NovoProduto("Mouse Gamer Logitech G502", 0.20m, 0.01m, null);
        var caboHdmi = NovoProduto("Cabo HDMI 2.1 — 5m", 0.30m, 0.01m, null);
        var arroz = NovoProduto("Arroz Branco Tipo 1 — 5kg", 5.00m, 0.01m, "2026-12-31");
        var feijao = NovoProduto("Feijão Preto — 1kg", 1.00m, 0.01m, "2026-09-30");
        var azeite = NovoProduto("Azeite de Oliva Extra Virgem 500ml", 0.60m, 0.01m, "2027-03-15");
        var capacete = NovoProduto("Capacete de Segurança CA-31097", 0.40m, 0.02m, null);
        var luva = NovoProduto("Luva Nitrílica Descartável cx/100", 0.30m, 0.01m, "2028-06-01");
        var parafuso = NovoProduto("Parafuso Sextavado M10 cx/500", 5.00m, 0.01m, null);
        var caixa = NovoProduto("Caixa de Papelão 60x40x40 (un)", 0.50m, 0.10m, null);
        contexto.Produtos.AddRange(notebook, monitor, teclado, mouse, caboHdmi, arroz, feijao, azeite, capacete, luva, parafuso, caixa);

        // Estoque inicial por armazém (alguns itens abaixo de 10 unidades = nível crítico)
        contexto.Estoques.AddRange(
            NovoEstoque(notebook, cdSaoPaulo, 45), NovoEstoque(monitor, cdSaoPaulo, 30), NovoEstoque(teclado, cdSaoPaulo, 80),
            NovoEstoque(mouse, cdSaoPaulo, 120), NovoEstoque(caboHdmi, cdSaoPaulo, 200), NovoEstoque(arroz, cdSaoPaulo, 500),
            NovoEstoque(capacete, cdSaoPaulo, 60), NovoEstoque(luva, cdSaoPaulo, 7), NovoEstoque(caixa, cdSaoPaulo, 300),
            NovoEstoque(notebook, cdPortoAlegre, 20), NovoEstoque(monitor, cdPortoAlegre, 15), NovoEstoque(arroz, cdPortoAlegre, 300),
            NovoEstoque(feijao, cdPortoAlegre, 250), NovoEstoque(azeite, cdPortoAlegre, 180), NovoEstoque(parafuso, cdPortoAlegre, 4),
            NovoEstoque(caixa, cdPortoAlegre, 150),
            NovoEstoque(teclado, cdBrasilia, 50), NovoEstoque(mouse, cdBrasilia, 90), NovoEstoque(caboHdmi, cdBrasilia, 110),
            NovoEstoque(feijao, cdBrasilia, 180), NovoEstoque(capacete, cdBrasilia, 35), NovoEstoque(luva, cdBrasilia, 85),
            NovoEstoque(parafuso, cdBrasilia, 220),
            NovoEstoque(notebook, cdCuritiba, 8), NovoEstoque(monitor, cdCuritiba, 12), NovoEstoque(arroz, cdCuritiba, 400),
            NovoEstoque(azeite, cdCuritiba, 95), NovoEstoque(capacete, cdCuritiba, 5), NovoEstoque(caixa, cdCuritiba, 500));

        contexto.SaveChanges();
    }

    // ----- Funções auxiliares para deixar a lista de dados mais curta -----

    private static Endereco NovoEndereco(string cep, string logradouro, string numero, string cidade, string estado)
    {
        return new Endereco { Cep = cep, Logradouro = logradouro, Numero = numero, Cidade = cidade, Estado = estado };
    }

    private static Transportadora NovaTransportadora(string cnpj, string razaoSocial, string nomeFantasia, string telefone, string email, Endereco endereco)
    {
        return new Transportadora { Cnpj = cnpj, RazaoSocial = razaoSocial, NomeFantasia = nomeFantasia, Telefone = telefone, Email = email, Endereco = endereco };
    }

    private static Motorista NovoMotorista(Transportadora transportadora, string nome, string cpf, string cnh, string categoria, string validade, string telefone)
    {
        return new Motorista
        {
            Transportadora = transportadora, Nome = nome, Cpf = cpf, Cnh = cnh,
            CategoriaCnh = categoria, ValidadeCnh = DateOnly.Parse(validade), Telefone = telefone
        };
    }

    private static Veiculo NovoVeiculo(Transportadora transportadora, string placa, string modelo, string tipo, decimal capacidade)
    {
        return new Veiculo { Transportadora = transportadora, Placa = placa, Modelo = modelo, TipoVeiculo = tipo, CapacidadeCarga = capacidade };
    }

    private static Cliente NovoCliente(string nome, string cpfCnpj, string telefone, Endereco endereco)
    {
        return new Cliente { Nome = nome, CpfCnpj = cpfCnpj, Telefone = telefone, Endereco = endereco };
    }

    private static Produto NovoProduto(string descricao, decimal peso, decimal volume, string? validade)
    {
        return new Produto
        {
            Descricao = descricao, Peso = peso, Volume = volume,
            Validade = validade == null ? null : DateOnly.Parse(validade)
        };
    }

    private static Estoque NovoEstoque(Produto produto, Armazem armazem, int quantidade)
    {
        return new Estoque { Produto = produto, Armazem = armazem, Quantidade = quantidade };
    }
}

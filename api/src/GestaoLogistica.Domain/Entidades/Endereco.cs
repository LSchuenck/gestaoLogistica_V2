namespace GestaoLogistica.Domain.Entidades;

// Endereço usado por Transportadora, Cliente e Armazém.
// É gravado junto com a entidade "dona" (não possui Id próprio).
public class Endereco
{
    public string? Cep { get; set; }
    public string? Logradouro { get; set; }
    public string? Numero { get; set; }
    public string? Complemento { get; set; }
    public string? Bairro { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
}

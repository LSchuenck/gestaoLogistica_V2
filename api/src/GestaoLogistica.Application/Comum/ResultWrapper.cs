using System.Text.Json.Serialization;

namespace GestaoLogistica.Application.Comum;

// Padrão único de resposta da API (sucesso ou falha sempre têm o mesmo formato):
// { "sucesso": true, "mensagem": "...", "dados": {...}, "erros": [] }
public class ResultWrapper<T>
{
    public bool Sucesso { get; private set; }
    public string Mensagem { get; private set; } = string.Empty;
    public T? Dados { get; private set; }
    public List<string> Erros { get; private set; } = new();

    // Código HTTP que o controller deve devolver (não aparece no JSON)
    [JsonIgnore]
    public int CodigoHttp { get; private set; }

    public static ResultWrapper<T> Ok(T? dados, string mensagem, int codigoHttp = 200)
    {
        return new ResultWrapper<T>
        {
            Sucesso = true,
            Mensagem = mensagem,
            Dados = dados,
            CodigoHttp = codigoHttp
        };
    }

    public static ResultWrapper<T> Criado(T dados, string mensagem) => Ok(dados, mensagem, 201);

    public static ResultWrapper<T> Falha(string mensagem, List<string>? erros = null, int codigoHttp = 400)
    {
        return new ResultWrapper<T>
        {
            Sucesso = false,
            Mensagem = mensagem,
            Dados = default,
            Erros = erros ?? new List<string> { mensagem },
            CodigoHttp = codigoHttp
        };
    }

    public static ResultWrapper<T> NaoEncontrado(string mensagem) => Falha(mensagem, null, 404);
}

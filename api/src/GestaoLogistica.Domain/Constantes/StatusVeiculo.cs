namespace GestaoLogistica.Domain.Constantes;

public static class StatusVeiculo
{
    public const string Disponivel = "DISPONIVEL";
    public const string EmViagem = "EM_VIAGEM";
    public const string Manutencao = "MANUTENCAO";

    public static readonly string[] Todos = { Disponivel, EmViagem, Manutencao };
}

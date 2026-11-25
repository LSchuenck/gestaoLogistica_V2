namespace GestaoLogistica.Domain.Constantes;

public static class PerfisUsuario
{
    public const string Administrador = "ADMIN";
    public const string Gerente = "GERENTE";
    public const string Operador = "OPERADOR";

    public const string AdministradorOuGerente = "ADMIN,GERENTE";

    public static readonly string[] Todos = { Administrador, Gerente, Operador };
}

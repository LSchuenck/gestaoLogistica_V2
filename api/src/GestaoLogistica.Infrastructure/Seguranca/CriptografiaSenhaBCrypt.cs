using GestaoLogistica.Application.Interfaces;

namespace GestaoLogistica.Infrastructure.Seguranca;

// Usa o algoritmo BCrypt (o mesmo do password_hash do PHP no sistema original).
// O hash já contém um "salt" aleatório, então duas senhas iguais geram hashes diferentes.
public class CriptografiaSenhaBCrypt : ICriptografiaSenha
{
    public string GerarHash(string senha)
    {
        return BCrypt.Net.BCrypt.HashPassword(senha);
    }

    public bool SenhaConfere(string senha, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(senha, hash);
    }
}

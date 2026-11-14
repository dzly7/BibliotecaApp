using System.Security.Cryptography;
using System.Text;

namespace BibliotecaApp.Services;

public static class Seguridad
{
    public static string Hash(string texto)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(texto));
        return Convert.ToHexString(bytes);
    }
}

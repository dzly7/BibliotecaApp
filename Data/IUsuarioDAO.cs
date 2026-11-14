using BibliotecaApp.Models;

namespace BibliotecaApp.Data;

public interface IUsuarioDAO
{
    Task<List<Usuario>> ObtenerTodosAsync();
    Task<Usuario?> ObtenerPorIdAsync(int id);
    Task<Usuario?> AutenticarAsync(string usuarioOCorreo, string passwordHash);
    Task<bool> ExisteCorreoAsync(string correo);
    Task<int> InsertarAsync(Usuario usuario);
    Task<int> EliminarAsync(int id);
    Task<int> ContarAsync();
}

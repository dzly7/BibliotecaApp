using BibliotecaApp.Models;

namespace BibliotecaApp.Data;

public class UsuarioDAO : IUsuarioDAO
{
    private readonly BibliotecaDb _db;

    public UsuarioDAO(BibliotecaDb db)
    {
        _db = db;
    }

    public async Task<List<Usuario>> ObtenerTodosAsync()
    {
        var conexion = await _db.ObtenerConexionAsync();
        return await conexion.QueryAsync<Usuario>(
            "SELECT Id, Nombre, Correo, PasswordHash, Rol FROM Usuarios ORDER BY Rol, Nombre");
    }

    public async Task<Usuario?> ObtenerPorIdAsync(int id)
    {
        var conexion = await _db.ObtenerConexionAsync();
        return await conexion.FindAsync<Usuario>(id);
    }

    public async Task<Usuario?> AutenticarAsync(string usuarioOCorreo, string passwordHash)
    {
        var conexion = await _db.ObtenerConexionAsync();
        var resultado = await conexion.QueryAsync<Usuario>(
            "SELECT Id, Nombre, Correo, PasswordHash, Rol FROM Usuarios " +
            "WHERE (Correo = ? COLLATE NOCASE OR Nombre = ? COLLATE NOCASE) AND PasswordHash = ? LIMIT 1",
            usuarioOCorreo, usuarioOCorreo, passwordHash);
        return resultado.FirstOrDefault();
    }

    public async Task<bool> ExisteCorreoAsync(string correo)
    {
        var conexion = await _db.ObtenerConexionAsync();
        var total = await conexion.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM Usuarios WHERE Correo = ? COLLATE NOCASE", correo);
        return total > 0;
    }

    public async Task<int> InsertarAsync(Usuario usuario)
    {
        var conexion = await _db.ObtenerConexionAsync();
        return await conexion.InsertAsync(usuario);
    }

    public async Task<int> EliminarAsync(int id)
    {
        var conexion = await _db.ObtenerConexionAsync();
        return await conexion.DeleteAsync<Usuario>(id);
    }

    public async Task<int> ContarAsync()
    {
        var conexion = await _db.ObtenerConexionAsync();
        return await conexion.Table<Usuario>().CountAsync();
    }
}

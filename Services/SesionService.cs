using BibliotecaApp.Data;
using BibliotecaApp.Models;

namespace BibliotecaApp.Services;

public class SesionService
{
    private const string ClaveUsuario = "sesion_usuario_id";

    private readonly IUsuarioDAO _usuarios;

    public SesionService(IUsuarioDAO usuarios)
    {
        _usuarios = usuarios;
    }

    public Usuario? Actual { get; private set; }

    public bool EsAdministrador => Actual?.EsAdministrador == true;

    public async Task<bool> IniciarAsync(string usuarioOCorreo, string password, bool recordar)
    {
        var usuario = await _usuarios.AutenticarAsync(usuarioOCorreo.Trim(), Seguridad.Hash(password));
        if (usuario is null)
            return false;

        Actual = usuario;

        if (recordar)
            Preferences.Default.Set(ClaveUsuario, usuario.Id);
        else
            Preferences.Default.Remove(ClaveUsuario);

        return true;
    }

    public async Task<bool> RestaurarAsync()
    {
        if (Actual is not null)
            return true;

        var id = Preferences.Default.Get(ClaveUsuario, 0);
        if (id == 0)
            return false;

        Actual = await _usuarios.ObtenerPorIdAsync(id);
        if (Actual is null)
            Preferences.Default.Remove(ClaveUsuario);

        return Actual is not null;
    }

    public void Cerrar()
    {
        Actual = null;
        Preferences.Default.Remove(ClaveUsuario);
    }
}

using BibliotecaApp.Services;

namespace BibliotecaApp.Views;

public partial class LoginPage : ContentPage
{
    private readonly SesionService _sesion;

    public LoginPage(SesionService sesion)
    {
        InitializeComponent();
        _sesion = sesion;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            if (await _sesion.RestaurarAsync())
                await Shell.Current.GoToAsync("//libros");
        }
        catch (Exception ex)
        {
            MostrarError($"No se pudo abrir la base de datos: {ex.Message}");
        }
    }

    private async void OnIniciarSesionClicked(object? sender, EventArgs e)
    {
        var usuario = UsuarioEntry.Text?.Trim() ?? string.Empty;
        var contrasena = ContrasenaEntry.Text ?? string.Empty;

        if (usuario.Length == 0 || contrasena.Length == 0)
        {
            MostrarError("Escribe tu usuario y contraseña.");
            return;
        }

        Cargando(true);
        try
        {
            if (!await _sesion.IniciarAsync(usuario, contrasena, RecordarCheck.IsChecked))
            {
                MostrarError("Usuario o contraseña incorrectos.");
                return;
            }

            ErrorLabel.IsVisible = false;
            ContrasenaEntry.Text = string.Empty;
            await Shell.Current.GoToAsync("//libros");
        }
        catch (Exception ex)
        {
            MostrarError($"Error al iniciar sesión: {ex.Message}");
        }
        finally
        {
            Cargando(false);
        }
    }

    private void Cargando(bool activo)
    {
        CargandoIndicator.IsRunning = activo;
        CargandoIndicator.IsVisible = activo;
        IngresarButton.IsEnabled = !activo;
    }

    private void MostrarError(string mensaje)
    {
        ErrorLabel.Text = mensaje;
        ErrorLabel.IsVisible = true;
    }
}

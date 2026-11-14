using BibliotecaApp.Data;
using BibliotecaApp.Models;
using BibliotecaApp.Services;

namespace BibliotecaApp.Views;

public partial class UsuariosPage : ContentPage
{
    private readonly IUsuarioDAO _usuarios;
    private readonly IPrestamoDAO _prestamos;
    private readonly SesionService _sesion;

    public UsuariosPage(IUsuarioDAO usuarios, IPrestamoDAO prestamos, SesionService sesion)
    {
        InitializeComponent();
        _usuarios = usuarios;
        _prestamos = prestamos;
        _sesion = sesion;
        RolPicker.ItemsSource = Catalogos.Roles;
        RolPicker.SelectedItem = Catalogos.RolLector;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        FormularioBorder.IsVisible = _sesion.EsAdministrador;
        await CargarAsync();
    }

    private async Task CargarAsync()
    {
        try
        {
            var lista = await _usuarios.ObtenerTodosAsync();
            UsuariosView.ItemsSource = lista;
            TotalLabel.Text = $"{lista.Count} usuario(s) registrados";
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }

    private async void OnAgregarClicked(object? sender, EventArgs e)
    {
        var nombre = NombreEntry.Text?.Trim() ?? string.Empty;
        var correo = CorreoEntry.Text?.Trim().ToLowerInvariant() ?? string.Empty;
        var contrasena = ContrasenaEntry.Text ?? string.Empty;
        var rol = RolPicker.SelectedItem as string ?? Catalogos.RolLector;

        if (nombre.Length == 0 || correo.Length == 0 || contrasena.Length == 0)
        {
            MostrarMensaje("Completa nombre, correo y contraseña.");
            return;
        }

        if (!correo.Contains('@') || !correo.Contains('.'))
        {
            MostrarMensaje("El correo no es válido.");
            return;
        }

        if (contrasena.Length < 4)
        {
            MostrarMensaje("La contraseña debe tener al menos 4 caracteres.");
            return;
        }

        try
        {
            if (await _usuarios.ExisteCorreoAsync(correo))
            {
                MostrarMensaje("Ya existe un usuario con ese correo.");
                return;
            }

            await _usuarios.InsertarAsync(new Usuario
            {
                Nombre = nombre,
                Correo = correo,
                PasswordHash = Seguridad.Hash(contrasena),
                Rol = rol
            });

            NombreEntry.Text = CorreoEntry.Text = ContrasenaEntry.Text = string.Empty;
            MensajeLabel.IsVisible = false;
            await CargarAsync();
        }
        catch (Exception ex)
        {
            MostrarMensaje($"No se pudo guardar: {ex.Message}");
        }
    }

    private async void OnEliminarClicked(object? sender, EventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is not Usuario usuario)
            return;

        if (!_sesion.EsAdministrador)
        {
            await DisplayAlertAsync("Permiso denegado", "Solo un administrador puede eliminar usuarios.", "OK");
            return;
        }

        if (usuario.Id == _sesion.Actual?.Id)
        {
            await DisplayAlertAsync("Eliminar", "No puedes eliminar tu propio usuario.", "OK");
            return;
        }

        try
        {
            if (await _prestamos.ContarActivosPorUsuarioAsync(usuario.Id) > 0)
            {
                await DisplayAlertAsync("Eliminar", $"{usuario.Nombre} tiene préstamos sin devolver.", "OK");
                return;
            }

            if (!await DisplayAlertAsync("Eliminar", $"¿Eliminar a {usuario.Nombre}?", "Sí", "No"))
                return;

            await _usuarios.EliminarAsync(usuario.Id);
            await CargarAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }

    private void MostrarMensaje(string mensaje)
    {
        MensajeLabel.Text = mensaje;
        MensajeLabel.IsVisible = true;
    }
}

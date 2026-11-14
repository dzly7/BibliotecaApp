using BibliotecaApp.Data;
using BibliotecaApp.Models;

namespace BibliotecaApp.Views;

public partial class PrestamosPage : ContentPage
{
    private static readonly Color ColorActivo = Color.FromArgb("#2563EB");

    private readonly IPrestamoDAO _prestamos;
    private readonly ILibroDAO _libros;
    private readonly IUsuarioDAO _usuarios;
    private bool _soloActivos = true;

    public PrestamosPage(IPrestamoDAO prestamos, ILibroDAO libros, IUsuarioDAO usuarios)
    {
        InitializeComponent();
        _prestamos = prestamos;
        _libros = libros;
        _usuarios = usuarios;

        LibroPicker.ItemDisplayBinding = new Binding(nameof(Libro.Titulo));
        UsuarioPicker.ItemDisplayBinding = new Binding(nameof(Usuario.Nombre));
        PintarFiltro();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarCatalogosAsync();
        await CargarPrestamosAsync();
    }

    private async Task CargarCatalogosAsync()
    {
        try
        {
            LibroPicker.ItemsSource = await _libros.ObtenerDisponiblesAsync();
            LibroPicker.SelectedIndex = -1;
            UsuarioPicker.ItemsSource = await _usuarios.ObtenerTodosAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }

    private async Task CargarPrestamosAsync()
    {
        try
        {
            PrestamosView.ItemsSource = await _prestamos.ObtenerDetalleAsync(_soloActivos);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }

    private void PintarFiltro()
    {
        ActivosButton.BackgroundColor = _soloActivos ? ColorActivo : Colors.White;
        ActivosButton.TextColor = _soloActivos ? Colors.White : ColorActivo;
        HistorialButton.BackgroundColor = _soloActivos ? Colors.White : ColorActivo;
        HistorialButton.TextColor = _soloActivos ? ColorActivo : Colors.White;
    }

    private async void OnActivosClicked(object? sender, EventArgs e)
    {
        _soloActivos = true;
        PintarFiltro();
        await CargarPrestamosAsync();
    }

    private async void OnHistorialClicked(object? sender, EventArgs e)
    {
        _soloActivos = false;
        PintarFiltro();
        await CargarPrestamosAsync();
    }

    private async void OnRegistrarClicked(object? sender, EventArgs e)
    {
        if (LibroPicker.SelectedItem is not Libro libro || UsuarioPicker.SelectedItem is not Usuario usuario)
        {
            MensajeLabel.Text = "Selecciona un libro y un usuario.";
            MensajeLabel.IsVisible = true;
            return;
        }

        try
        {
            if (!await _prestamos.RegistrarAsync(libro.Id, usuario.Id))
            {
                MensajeLabel.Text = "El libro ya no está disponible.";
                MensajeLabel.IsVisible = true;
                return;
            }

            MensajeLabel.IsVisible = false;
            await DisplayAlertAsync("Préstamo registrado", $"\"{libro.Titulo}\" prestado a {usuario.Nombre}.", "OK");
            await CargarCatalogosAsync();
            await CargarPrestamosAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }

    private async void OnDevolverClicked(object? sender, EventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is not PrestamoDetalle prestamo)
            return;

        if (!await DisplayAlertAsync("Devolver", $"¿Registrar la devolución de \"{prestamo.Titulo}\"?", "Sí", "No"))
            return;

        try
        {
            await _prestamos.DevolverAsync(prestamo.Id);
            await CargarCatalogosAsync();
            await CargarPrestamosAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }
}

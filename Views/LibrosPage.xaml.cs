using BibliotecaApp.Data;
using BibliotecaApp.Models;
using BibliotecaApp.Services;

namespace BibliotecaApp.Views;

public partial class LibrosPage : ContentPage
{
    private const string Todas = "Todas";

    private static readonly Color ColorActivo = Color.FromArgb("#2563EB");

    private readonly ILibroDAO _libros;
    private readonly SesionService _sesion;
    private string _categoria = Todas;

    public LibrosPage(ILibroDAO libros, SesionService sesion)
    {
        InitializeComponent();
        _libros = libros;
        _sesion = sesion;
        CrearFiltros();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_sesion.Actual is null)
        {
            await Shell.Current.GoToAsync("//login");
            return;
        }

        SaludoLabel.Text = $"Hola, {_sesion.Actual.Nombre}";
        await CargarAsync();
    }

    private async Task CargarAsync()
    {
        try
        {
            var texto = BuscadorBar.Text?.Trim() ?? string.Empty;
            var categoria = _categoria == Todas ? string.Empty : _categoria;
            var lista = await _libros.BuscarAsync(texto, categoria);

            LibrosView.ItemsSource = lista;
            ResultadosLabel.Text = $"{lista.Count} libro(s)";
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }

    private void CrearFiltros()
    {
        foreach (var categoria in new[] { Todas }.Concat(Catalogos.Categorias))
        {
            var boton = new Button
            {
                Text = categoria,
                CornerRadius = 18,
                Padding = new Thickness(14, 4),
                Margin = new Thickness(0, 0, 8, 8),
                FontSize = 13,
                BorderColor = ColorActivo,
                BorderWidth = 1
            };
            boton.Clicked += OnFiltroClicked;
            FiltrosLayout.Children.Add(boton);
        }
        PintarFiltros();
    }

    private void PintarFiltros()
    {
        foreach (var boton in FiltrosLayout.Children.OfType<Button>())
        {
            var activo = boton.Text == _categoria;
            boton.BackgroundColor = activo ? ColorActivo : Colors.White;
            boton.TextColor = activo ? Colors.White : ColorActivo;
        }
    }

    private async void OnFiltroClicked(object? sender, EventArgs e)
    {
        if (sender is not Button boton)
            return;

        _categoria = boton.Text;
        PintarFiltros();
        await CargarAsync();
    }

    private async void OnBuscarTextChanged(object? sender, TextChangedEventArgs e) => await CargarAsync();

    private async void OnRefrescar(object? sender, EventArgs e)
    {
        await CargarAsync();
        Refresco.IsRefreshing = false;
    }

    private async void OnNuevoClicked(object? sender, EventArgs e)
    {
        if (!await ValidarAdministradorAsync())
            return;

        await Shell.Current.GoToAsync("libroform");
    }

    private async void OnEditarClicked(object? sender, EventArgs e)
    {
        if (!await ValidarAdministradorAsync())
            return;

        if (LibrosView.SelectedItem is not Libro libro)
        {
            await DisplayAlertAsync("Editar", "Primero selecciona un libro.", "OK");
            return;
        }

        await Shell.Current.GoToAsync($"libroform?id={libro.Id}");
    }

    private async void OnEliminarClicked(object? sender, EventArgs e)
    {
        if (!await ValidarAdministradorAsync())
            return;

        if (LibrosView.SelectedItem is not Libro libro)
        {
            await DisplayAlertAsync("Eliminar", "Primero selecciona un libro.", "OK");
            return;
        }

        if (!libro.Disponible)
        {
            await DisplayAlertAsync("Eliminar", "No se puede eliminar un libro que está prestado.", "OK");
            return;
        }

        if (!await DisplayAlertAsync("Eliminar", $"¿Eliminar \"{libro.Titulo}\"?", "Sí", "No"))
            return;

        try
        {
            await _libros.EliminarAsync(libro.Id);
            LibrosView.SelectedItem = null;
            await CargarAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }

    private async Task<bool> ValidarAdministradorAsync()
    {
        if (_sesion.EsAdministrador)
            return true;

        await DisplayAlertAsync("Permiso denegado", "Solo un administrador puede modificar el catálogo.", "OK");
        return false;
    }
}

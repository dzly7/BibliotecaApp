using BibliotecaApp.Data;
using BibliotecaApp.Models;

namespace BibliotecaApp.Views;

public partial class LibroFormPage : ContentPage, IQueryAttributable
{
    private readonly ILibroDAO _libros;
    private Libro? _libro;
    private int _idLibro;

    public LibroFormPage(ILibroDAO libros)
    {
        InitializeComponent();
        _libros = libros;
        CategoriaPicker.ItemsSource = Catalogos.Categorias;
        ColorPicker.ItemsSource = Catalogos.Colores.Keys.ToList();
        ColorPicker.SelectedIndex = 0;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("id", out var valor) && int.TryParse(valor?.ToString(), out var id))
            _idLibro = id;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_idLibro == 0 || _libro is not null)
            return;

        try
        {
            _libro = await _libros.ObtenerPorIdAsync(_idLibro);
            if (_libro is null)
                return;

            Title = "Editar Libro";
            EncabezadoLabel.Text = "✏️ Editar libro";
            TituloEntry.Text = _libro.Titulo;
            AutorEntry.Text = _libro.Autor;
            CategoriaPicker.SelectedItem = _libro.Categoria;

            var color = Catalogos.Colores.FirstOrDefault(c => c.Value == _libro.ColorHex).Key;
            if (color is not null)
                ColorPicker.SelectedItem = color;

            EstadoLabel.Text = _libro.EstadoTexto;
            EstadoLabel.TextColor = _libro.EstadoColor;
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }

    private async void OnGuardarClicked(object? sender, EventArgs e)
    {
        var titulo = TituloEntry.Text?.Trim() ?? string.Empty;
        var autor = AutorEntry.Text?.Trim() ?? string.Empty;
        var categoria = CategoriaPicker.SelectedItem as string;
        var color = ColorPicker.SelectedItem as string ?? Catalogos.Colores.Keys.First();

        if (titulo.Length == 0 || autor.Length == 0 || categoria is null)
        {
            MensajeLabel.Text = "Completa título, autor y categoría.";
            MensajeLabel.IsVisible = true;
            return;
        }

        try
        {
            var libro = _libro ?? new Libro { FechaRegistro = DateTime.Now, Disponible = true };
            libro.Titulo = titulo;
            libro.Autor = autor;
            libro.Categoria = categoria;
            libro.ColorHex = Catalogos.Colores[color];

            if (libro.Id == 0)
                await _libros.InsertarAsync(libro);
            else
                await _libros.ActualizarAsync(libro);

            await DisplayAlertAsync("Listo", $"\"{titulo}\" se guardó correctamente.", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            MensajeLabel.Text = $"No se pudo guardar: {ex.Message}";
            MensajeLabel.IsVisible = true;
        }
    }

    private async void OnCancelarClicked(object? sender, EventArgs e) => await Shell.Current.GoToAsync("..");
}

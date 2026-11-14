using BibliotecaApp.Data;
using BibliotecaApp.Services;

namespace BibliotecaApp.Views;

public partial class ReportesPage : ContentPage
{
    private readonly ILibroDAO _libros;
    private readonly IUsuarioDAO _usuarios;
    private readonly IPrestamoDAO _prestamos;
    private readonly SesionService _sesion;
    private readonly ArchivoService _archivos;

    public ReportesPage(ILibroDAO libros, IUsuarioDAO usuarios, IPrestamoDAO prestamos,
                        SesionService sesion, ArchivoService archivos)
    {
        InitializeComponent();
        _libros = libros;
        _usuarios = usuarios;
        _prestamos = prestamos;
        _sesion = sesion;
        _archivos = archivos;
        RutaBdLabel.Text = $"Base de datos SQLite: {BibliotecaDb.Ruta}";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        UsuarioLabel.Text = _sesion.Actual?.Nombre ?? string.Empty;
        RolLabel.Text = _sesion.Actual?.Rol ?? string.Empty;

        try
        {
            var total = await _libros.ContarAsync();
            var disponibles = await _libros.ContarDisponiblesAsync();

            TotalLibrosLabel.Text = total.ToString();
            DisponiblesLabel.Text = disponibles.ToString();
            PrestadosLabel.Text = (total - disponibles).ToString();
            UsuariosLabel.Text = (await _usuarios.ContarAsync()).ToString();
            ActivosLabel.Text = (await _prestamos.ContarActivosAsync()).ToString();
            HistoricosLabel.Text = (await _prestamos.ContarTotalAsync()).ToString();

            var categorias = await _libros.ContarPorCategoriaAsync();
            var maximo = categorias.Count == 0 ? 1 : categorias.Max(c => c.Total);
            foreach (var categoria in categorias)
                categoria.Porcentaje = (double)categoria.Total / maximo;

            BindableLayout.SetItemsSource(CategoriasLayout, categorias);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }

    private async void OnExportarClicked(object? sender, EventArgs e)
    {
        try
        {
            var ruta = await _archivos.ExportarCatalogoAsync(await _libros.ObtenerTodosAsync());
            ExportacionLabel.Text = $"Archivo guardado en: {ruta}";
            ExportacionLabel.IsVisible = true;
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }

    private async void OnCerrarSesionClicked(object? sender, EventArgs e)
    {
        if (!await DisplayAlertAsync("Cerrar sesión", "¿Deseas salir?", "Sí", "No"))
            return;

        _sesion.Cerrar();
        await Shell.Current.GoToAsync("//login");
    }
}

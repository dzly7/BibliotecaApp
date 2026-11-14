using System.Text.Json;
using BibliotecaApp.Models;

namespace BibliotecaApp.Services;

public class ArchivoService
{
    private static readonly JsonSerializerOptions Opciones = new() { WriteIndented = true };

    public async Task<string> ExportarCatalogoAsync(IEnumerable<Libro> libros)
    {
        var datos = libros.Select(l => new
        {
            l.Id,
            l.Titulo,
            l.Autor,
            l.Categoria,
            l.Disponible,
            FechaRegistro = l.FechaRegistro.ToString("yyyy-MM-dd")
        });

        var ruta = Path.Combine(FileSystem.AppDataDirectory, $"catalogo_{DateTime.Now:yyyyMMdd_HHmmss}.json");
        await using var archivo = File.Create(ruta);
        await JsonSerializer.SerializeAsync(archivo, datos, Opciones);
        return ruta;
    }
}

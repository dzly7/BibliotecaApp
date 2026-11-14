using BibliotecaApp.Models;

namespace BibliotecaApp.Data;

public interface ILibroDAO
{
    Task<List<Libro>> BuscarAsync(string texto, string categoria);
    Task<List<Libro>> ObtenerTodosAsync();
    Task<List<Libro>> ObtenerDisponiblesAsync();
    Task<Libro?> ObtenerPorIdAsync(int id);
    Task<int> InsertarAsync(Libro libro);
    Task<int> ActualizarAsync(Libro libro);
    Task<int> EliminarAsync(int id);
    Task<int> ContarAsync();
    Task<int> ContarDisponiblesAsync();
    Task<List<ConteoCategoria>> ContarPorCategoriaAsync();
}

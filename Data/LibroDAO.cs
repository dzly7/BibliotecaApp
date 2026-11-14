using BibliotecaApp.Models;

namespace BibliotecaApp.Data;

public class LibroDAO : ILibroDAO
{
    private const string Columnas = "Id, Titulo, Autor, Categoria, Disponible, ColorHex, FechaRegistro";

    private readonly BibliotecaDb _db;

    public LibroDAO(BibliotecaDb db)
    {
        _db = db;
    }

    public async Task<List<Libro>> BuscarAsync(string texto, string categoria)
    {
        var conexion = await _db.ObtenerConexionAsync();
        var patron = $"%{texto}%";

        return await conexion.QueryAsync<Libro>(
            $"SELECT {Columnas} FROM Libros " +
            "WHERE (? = '' OR Categoria = ?) " +
            "AND (? = '' OR Titulo LIKE ? OR Autor LIKE ?) " +
            "ORDER BY Titulo LIMIT 200",
            categoria, categoria, texto, patron, patron);
    }

    public async Task<List<Libro>> ObtenerTodosAsync()
    {
        var conexion = await _db.ObtenerConexionAsync();
        return await conexion.QueryAsync<Libro>($"SELECT {Columnas} FROM Libros ORDER BY Titulo");
    }

    public async Task<List<Libro>> ObtenerDisponiblesAsync()
    {
        var conexion = await _db.ObtenerConexionAsync();
        return await conexion.QueryAsync<Libro>(
            $"SELECT {Columnas} FROM Libros WHERE Disponible = 1 ORDER BY Titulo");
    }

    public async Task<Libro?> ObtenerPorIdAsync(int id)
    {
        var conexion = await _db.ObtenerConexionAsync();
        return await conexion.FindAsync<Libro>(id);
    }

    public async Task<int> InsertarAsync(Libro libro)
    {
        var conexion = await _db.ObtenerConexionAsync();
        return await conexion.InsertAsync(libro);
    }

    public async Task<int> ActualizarAsync(Libro libro)
    {
        var conexion = await _db.ObtenerConexionAsync();
        return await conexion.UpdateAsync(libro);
    }

    public async Task<int> EliminarAsync(int id)
    {
        var conexion = await _db.ObtenerConexionAsync();
        return await conexion.DeleteAsync<Libro>(id);
    }

    public async Task<int> ContarAsync()
    {
        var conexion = await _db.ObtenerConexionAsync();
        return await conexion.Table<Libro>().CountAsync();
    }

    public async Task<int> ContarDisponiblesAsync()
    {
        var conexion = await _db.ObtenerConexionAsync();
        return await conexion.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Libros WHERE Disponible = 1");
    }

    public async Task<List<ConteoCategoria>> ContarPorCategoriaAsync()
    {
        var conexion = await _db.ObtenerConexionAsync();
        return await conexion.QueryAsync<ConteoCategoria>(
            "SELECT Categoria, COUNT(*) AS Total FROM Libros GROUP BY Categoria ORDER BY Total DESC, Categoria");
    }
}

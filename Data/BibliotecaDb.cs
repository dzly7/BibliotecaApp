using BibliotecaApp.Models;
using BibliotecaApp.Services;
using SQLite;

namespace BibliotecaApp.Data;

public class BibliotecaDb
{
    public const string NombreArchivo = "biblioteca.db3";

    private const SQLiteOpenFlags Opciones =
        SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache;

    private readonly SemaphoreSlim _candado = new(1, 1);
    private SQLiteAsyncConnection? _conexion;

    public static string Ruta => Path.Combine(FileSystem.AppDataDirectory, NombreArchivo);

    public async Task<SQLiteAsyncConnection> ObtenerConexionAsync()
    {
        if (_conexion is not null)
            return _conexion;

        await _candado.WaitAsync();
        try
        {
            if (_conexion is null)
            {
                var conexion = new SQLiteAsyncConnection(Ruta, Opciones);
                await CrearEstructuraAsync(conexion);
                await CargarDatosInicialesAsync(conexion);
                _conexion = conexion;
            }
            return _conexion;
        }
        finally
        {
            _candado.Release();
        }
    }

    private static async Task CrearEstructuraAsync(SQLiteAsyncConnection conexion)
    {
        await conexion.CreateTableAsync<Libro>();
        await conexion.CreateTableAsync<Usuario>();
        await conexion.CreateTableAsync<Prestamo>();

        await conexion.ExecuteAsync(
            "CREATE INDEX IF NOT EXISTS idx_libros_categoria_titulo ON Libros (Categoria, Titulo)");
        await conexion.ExecuteAsync(
            "CREATE INDEX IF NOT EXISTS idx_prestamos_devolucion_fecha ON Prestamos (FechaDevolucion, FechaPrestamo)");
    }

    private static async Task CargarDatosInicialesAsync(SQLiteAsyncConnection conexion)
    {
        if (await conexion.Table<Usuario>().CountAsync() == 0)
        {
            await conexion.InsertAllAsync(new[]
            {
                new Usuario { Nombre = "admin", Correo = "admin@biblioteca.mx", PasswordHash = Seguridad.Hash("1234"), Rol = Catalogos.RolAdministrador },
                new Usuario { Nombre = "Juan Pérez", Correo = "juan@utsjr.mx", PasswordHash = Seguridad.Hash("1234"), Rol = Catalogos.RolLector },
                new Usuario { Nombre = "Ana Rojas", Correo = "ana@utsjr.mx", PasswordHash = Seguridad.Hash("1234"), Rol = Catalogos.RolLector },
            });
        }

        if (await conexion.Table<Libro>().CountAsync() == 0)
        {
            var antes = DateTime.Now.AddDays(-30);
            await conexion.InsertAllAsync(new[]
            {
                new Libro { Titulo = "C# Avanzado", Autor = "A. Torres", Categoria = "Programación", ColorHex = "#7C3AED", FechaRegistro = antes },
                new Libro { Titulo = "Bases de Datos", Autor = "M. Rojas", Categoria = "Tecnología", ColorHex = "#0EA5E9", FechaRegistro = antes },
                new Libro { Titulo = "Física Universitaria", Autor = "S. Young", Categoria = "Ciencia", ColorHex = "#0F766E", FechaRegistro = antes },
                new Libro { Titulo = ".NET MAUI", Autor = "Microsoft", Categoria = "Programación", ColorHex = "#4F46E5" },
                new Libro { Titulo = "Ágil Scrum", Autor = "K. Schwaber", Categoria = "Tecnología", ColorHex = "#16A34A", FechaRegistro = antes },
                new Libro { Titulo = "El Principito", Autor = "Antoine de Saint-Exupéry", Categoria = "Novela", ColorHex = "#F59E0B", FechaRegistro = antes },
                new Libro { Titulo = "Cien Años de Soledad", Autor = "Gabriel García Márquez", Categoria = "Novela", ColorHex = "#DC2626", FechaRegistro = antes },
                new Libro { Titulo = "Sapiens", Autor = "Yuval Noah Harari", Categoria = "Historia", ColorHex = "#92400E", FechaRegistro = antes },
            });
        }
    }
}

using BibliotecaApp.Models;

namespace BibliotecaApp.Data;

public class PrestamoDAO : IPrestamoDAO
{
    private readonly BibliotecaDb _db;

    public PrestamoDAO(BibliotecaDb db)
    {
        _db = db;
    }

    public async Task<List<PrestamoDetalle>> ObtenerDetalleAsync(bool soloActivos)
    {
        var conexion = await _db.ObtenerConexionAsync();
        return await conexion.QueryAsync<PrestamoDetalle>(
            "SELECT p.Id, p.IdLibro, l.Titulo AS Titulo, u.Nombre AS Usuario, p.FechaPrestamo, p.FechaDevolucion " +
            "FROM Prestamos p " +
            "INNER JOIN Libros l ON l.Id = p.IdLibro " +
            "INNER JOIN Usuarios u ON u.Id = p.IdUsuario " +
            "WHERE (? = 0 OR p.FechaDevolucion IS NULL) " +
            "ORDER BY p.FechaPrestamo DESC LIMIT 100",
            soloActivos ? 1 : 0);
    }

    public async Task<bool> RegistrarAsync(int idLibro, int idUsuario)
    {
        var conexion = await _db.ObtenerConexionAsync();
        var registrado = false;

        await conexion.RunInTransactionAsync(transaccion =>
        {
            var libro = transaccion.Find<Libro>(idLibro);
            var usuario = transaccion.Find<Usuario>(idUsuario);
            if (libro is null || usuario is null || !libro.Disponible)
                return;

            transaccion.Insert(new Prestamo
            {
                IdLibro = idLibro,
                IdUsuario = idUsuario,
                FechaPrestamo = DateTime.Now
            });
            transaccion.Execute("UPDATE Libros SET Disponible = 0 WHERE Id = ?", idLibro);
            registrado = true;
        });

        return registrado;
    }

    public async Task<bool> DevolverAsync(int idPrestamo)
    {
        var conexion = await _db.ObtenerConexionAsync();
        var devuelto = false;

        await conexion.RunInTransactionAsync(transaccion =>
        {
            var prestamo = transaccion.Find<Prestamo>(idPrestamo);
            if (prestamo is null || prestamo.FechaDevolucion is not null)
                return;

            prestamo.FechaDevolucion = DateTime.Now;
            transaccion.Update(prestamo);
            transaccion.Execute("UPDATE Libros SET Disponible = 1 WHERE Id = ?", prestamo.IdLibro);
            devuelto = true;
        });

        return devuelto;
    }

    public async Task<int> ContarActivosAsync()
    {
        var conexion = await _db.ObtenerConexionAsync();
        return await conexion.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM Prestamos WHERE FechaDevolucion IS NULL");
    }

    public async Task<int> ContarTotalAsync()
    {
        var conexion = await _db.ObtenerConexionAsync();
        return await conexion.Table<Prestamo>().CountAsync();
    }

    public async Task<int> ContarActivosPorUsuarioAsync(int idUsuario)
    {
        var conexion = await _db.ObtenerConexionAsync();
        return await conexion.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM Prestamos WHERE IdUsuario = ? AND FechaDevolucion IS NULL", idUsuario);
    }
}

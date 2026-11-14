using BibliotecaApp.Models;

namespace BibliotecaApp.Data;

public interface IPrestamoDAO
{
    Task<List<PrestamoDetalle>> ObtenerDetalleAsync(bool soloActivos);
    Task<bool> RegistrarAsync(int idLibro, int idUsuario);
    Task<bool> DevolverAsync(int idPrestamo);
    Task<int> ContarActivosAsync();
    Task<int> ContarTotalAsync();
    Task<int> ContarActivosPorUsuarioAsync(int idUsuario);
}

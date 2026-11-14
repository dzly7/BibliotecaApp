using SQLite;

namespace BibliotecaApp.Models;

[Table("Prestamos")]
public class Prestamo
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int IdLibro { get; set; }

    [Indexed]
    public int IdUsuario { get; set; }

    public DateTime FechaPrestamo { get; set; } = DateTime.Now;

    public DateTime? FechaDevolucion { get; set; }
}

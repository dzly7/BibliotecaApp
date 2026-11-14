namespace BibliotecaApp.Models;

public class PrestamoDetalle
{
    public int Id { get; set; }
    public int IdLibro { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Usuario { get; set; } = string.Empty;
    public DateTime FechaPrestamo { get; set; }
    public DateTime? FechaDevolucion { get; set; }

    public bool Activo => FechaDevolucion is null;

    public string EstadoTexto => Activo ? "En préstamo" : "Devuelto";

    public Color EstadoColor => Activo ? Color.FromArgb("#F59E0B") : Color.FromArgb("#16A34A");

    public string FechasTexto => Activo
        ? $"Prestado el {FechaPrestamo:dd/MM/yyyy HH:mm}"
        : $"{FechaPrestamo:dd/MM/yyyy} → {FechaDevolucion:dd/MM/yyyy}";
}

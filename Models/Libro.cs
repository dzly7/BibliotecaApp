using SQLite;

namespace BibliotecaApp.Models;

[Table("Libros")]
public class Libro
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [NotNull, MaxLength(150)]
    public string Titulo { get; set; } = string.Empty;

    [NotNull, MaxLength(120)]
    public string Autor { get; set; } = string.Empty;

    [NotNull, MaxLength(60)]
    public string Categoria { get; set; } = string.Empty;

    public bool Disponible { get; set; } = true;

    [MaxLength(9)]
    public string ColorHex { get; set; } = "#2563EB";

    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    [Ignore]
    public Color ColorPortada => Color.FromArgb(string.IsNullOrWhiteSpace(ColorHex) ? "#2563EB" : ColorHex);

    [Ignore]
    public bool EsNuevo => (DateTime.Now - FechaRegistro).TotalDays <= 7;

    [Ignore]
    public string EstadoTexto => Disponible ? "Disponible" : "Prestado";

    [Ignore]
    public Color EstadoColor => Disponible ? Color.FromArgb("#16A34A") : Color.FromArgb("#DC2626");

    [Ignore]
    public string Iniciales =>
        string.IsNullOrWhiteSpace(Titulo)
            ? "?"
            : string.Concat(Titulo.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                  .Take(2)
                                  .Select(p => char.ToUpper(p[0])));
}

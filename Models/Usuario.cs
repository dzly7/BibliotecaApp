using SQLite;

namespace BibliotecaApp.Models;

[Table("Usuarios")]
public class Usuario
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [NotNull, MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [NotNull, Unique, MaxLength(120)]
    public string Correo { get; set; } = string.Empty;

    [NotNull]
    public string PasswordHash { get; set; } = string.Empty;

    [NotNull, MaxLength(20)]
    public string Rol { get; set; } = Catalogos.RolLector;

    [Ignore]
    public bool EsAdministrador => Rol == Catalogos.RolAdministrador;

    [Ignore]
    public string Inicial => string.IsNullOrWhiteSpace(Nombre) ? "?" : Nombre[..1].ToUpper();

    [Ignore]
    public Color RolColor => EsAdministrador ? Color.FromArgb("#7C3AED") : Color.FromArgb("#0EA5E9");
}

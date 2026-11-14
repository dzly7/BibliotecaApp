namespace BibliotecaApp.Models;

public static class Catalogos
{
    public const string RolAdministrador = "Administrador";
    public const string RolLector = "Lector";

    public static readonly string[] Roles = { RolAdministrador, RolLector };

    public static readonly string[] Categorias =
        { "Programación", "Tecnología", "Novela", "Ciencia", "Historia" };

    public static readonly Dictionary<string, string> Colores = new()
    {
        ["Azul"] = "#2563EB",
        ["Morado"] = "#7C3AED",
        ["Verde"] = "#16A34A",
        ["Rojo"] = "#DC2626",
        ["Naranja"] = "#F59E0B",
        ["Turquesa"] = "#0F766E",
        ["Café"] = "#92400E",
    };
}

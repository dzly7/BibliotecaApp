using BibliotecaApp.Data;
using BibliotecaApp.Services;
using BibliotecaApp.Views;
using Microsoft.Extensions.Logging;

namespace BibliotecaApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

        builder.Services.AddSingleton<BibliotecaDb>();
        builder.Services.AddSingleton<ILibroDAO, LibroDAO>();
        builder.Services.AddSingleton<IUsuarioDAO, UsuarioDAO>();
        builder.Services.AddSingleton<IPrestamoDAO, PrestamoDAO>();
        builder.Services.AddSingleton<SesionService>();
        builder.Services.AddSingleton<ArchivoService>();

        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<LibrosPage>();
        builder.Services.AddTransient<LibroFormPage>();
        builder.Services.AddTransient<UsuariosPage>();
        builder.Services.AddTransient<PrestamosPage>();
        builder.Services.AddTransient<ReportesPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}

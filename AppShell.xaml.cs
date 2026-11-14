using BibliotecaApp.Views;

namespace BibliotecaApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("libroform", typeof(LibroFormPage));
    }
}

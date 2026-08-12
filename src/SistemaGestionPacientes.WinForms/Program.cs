using SistemaGestionPacientes.WinForms.Formularios;

namespace SistemaGestionPacientes.WinForms;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new FormPrincipal());
    }
}

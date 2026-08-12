namespace SistemaGestionPacientes.WinForms.Formularios;

internal static class Estilos
{
    public static readonly Color AzulPrincipal = Color.FromArgb(26, 79, 122);
    public static readonly Color AzulClaro = Color.FromArgb(232, 242, 250);
    public static readonly Color Rojo = Color.FromArgb(176, 50, 50);

    public static Button CrearBoton(string texto, Color? color = null)
    {
        return new Button
        {
            Text = texto,
            Width = 220,
            Height = 48,
            BackColor = color ?? AzulPrincipal,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            Cursor = Cursors.Hand,
            Margin = new Padding(10)
        };
    }

    public static Label CrearTitulo(string texto)
    {
        return new Label
        {
            Text = texto,
            Dock = DockStyle.Top,
            Height = 75,
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = AzulPrincipal,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 18F, FontStyle.Bold)
        };
    }
}

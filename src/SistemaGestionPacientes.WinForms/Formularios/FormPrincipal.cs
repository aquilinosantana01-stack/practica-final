using SistemaGestionPacientes.Core.Servicios;

namespace SistemaGestionPacientes.WinForms.Formularios;

public class FormPrincipal : Form
{
    private readonly GestorPacientes _gestor = new();

    public FormPrincipal()
    {
        Text = "Sistema de Gestión de Pacientes";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(760, 560);
        MinimumSize = new Size(720, 540);
        BackColor = Color.White;
        ConstruirInterfaz();
    }

    private void ConstruirInterfaz()
    {
        Controls.Add(Estilos.CrearTitulo("Sistema de Gestión de Pacientes"));

        var subtitulo = new Label
        {
            Text = "Seleccione la operación que desea realizar",
            Dock = DockStyle.Top,
            Height = 55,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 11F),
            ForeColor = Color.FromArgb(55, 55, 55)
        };
        Controls.Add(subtitulo);
        subtitulo.BringToFront();

        var panelBotones = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            AutoScroll = true,
            Padding = new Padding(120, 25, 100, 20)
        };

        Button btnRegistrar = Estilos.CrearBoton("1. Registrar paciente");
        Button btnListar = Estilos.CrearBoton("2. Listar pacientes");
        Button btnBuscar = Estilos.CrearBoton("3. Buscar paciente");
        Button btnActualizar = Estilos.CrearBoton("4. Actualizar paciente");
        Button btnEliminar = Estilos.CrearBoton("5. Eliminar paciente", Color.FromArgb(194, 104, 34));
        Button btnSalir = Estilos.CrearBoton("6. Salir del sistema", Estilos.Rojo);

        btnRegistrar.Click += (_, _) => AbrirFormulario(new FormPaciente(_gestor, false));
        btnListar.Click += (_, _) => AbrirFormulario(new FormConsulta(_gestor, ModoConsulta.Listar));
        btnBuscar.Click += (_, _) => AbrirFormulario(new FormConsulta(_gestor, ModoConsulta.Buscar));
        btnActualizar.Click += (_, _) => AbrirFormulario(new FormPaciente(_gestor, true));
        btnEliminar.Click += (_, _) => AbrirFormulario(new FormConsulta(_gestor, ModoConsulta.Eliminar));
        btnSalir.Click += (_, _) => SalirDelSistema();

        panelBotones.Controls.AddRange(new Control[]
        {
            btnRegistrar, btnListar, btnBuscar,
            btnActualizar, btnEliminar, btnSalir
        });
        Controls.Add(panelBotones);
        panelBotones.BringToFront();
    }

    private void AbrirFormulario(Form formulario)
    {
        using (formulario)
            formulario.ShowDialog(this);
    }

    private void SalirDelSistema()
    {
        DialogResult respuesta = MessageBox.Show(
            "¿Está seguro de que desea salir del sistema?",
            "Confirmar salida",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (respuesta == DialogResult.Yes)
            Application.Exit();
    }
}

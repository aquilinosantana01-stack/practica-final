using SistemaGestionPacientes.Core.Modelos;
using SistemaGestionPacientes.Core.Servicios;

namespace SistemaGestionPacientes.WinForms.Formularios;

public class FormConsulta : Form
{
    private readonly GestorPacientes _gestor;
    private readonly ModoConsulta _modo;
    private readonly TextBox _txtCriterio = new();
    private readonly DataGridView _grid = new();

    public FormConsulta(GestorPacientes gestor, ModoConsulta modo)
    {
        _gestor = gestor;
        _modo = modo;
        Text = ObtenerTitulo();
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(1050, 600);
        MinimumSize = new Size(900, 520);
        BackColor = Color.White;
        ConstruirInterfaz();
        Shown += (_, _) => PrepararVistaInicial();
    }

    private string ObtenerTitulo() => _modo switch
    {
        ModoConsulta.Buscar => "Buscar paciente",
        ModoConsulta.Eliminar => "Eliminar paciente",
        _ => "Listado de pacientes"
    };

    private void ConstruirInterfaz()
    {
        Controls.Add(Estilos.CrearTitulo(ObtenerTitulo()));

        var barra = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 70,
            Padding = new Padding(18, 14, 10, 8),
            BackColor = Estilos.AzulClaro
        };
        barra.Controls.Add(new Label
        {
            Text = "ID o nombre:",
            AutoSize = true,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            Margin = new Padding(0, 8, 8, 0)
        });
        _txtCriterio.Width = 245;
        _txtCriterio.Font = new Font("Segoe UI", 10F);

        Button btnBuscar = CrearBotonPequeno("Buscar", Estilos.AzulPrincipal);
        Button btnMostrar = CrearBotonPequeno("Mostrar todos", Color.FromArgb(65, 119, 81));
        Button btnEliminar = CrearBotonPequeno("Eliminar seleccionado", Estilos.Rojo);
        Button btnVolver = CrearBotonPequeno("Volver al menú", Color.Gray);

        btnBuscar.Click += (_, _) => BuscarPacientes();
        btnMostrar.Click += (_, _) => MostrarTodos(true);
        btnEliminar.Click += (_, _) => EliminarSeleccionado();
        btnVolver.Click += (_, _) => Close();
        btnEliminar.Visible = _modo == ModoConsulta.Eliminar;

        barra.Controls.AddRange(new Control[]
        {
            _txtCriterio, btnBuscar, btnMostrar, btnEliminar, btnVolver
        });

        ConfigurarGrid();
        Controls.Add(_grid);
        Controls.Add(barra);
        _grid.BringToFront();
        barra.BringToFront();
    }

    private static Button CrearBotonPequeno(string texto, Color color)
    {
        Button boton = Estilos.CrearBoton(texto, color);
        boton.Width = texto.Length > 15 ? 165 : 120;
        boton.Height = 34;
        boton.Margin = new Padding(6, 0, 0, 0);
        return boton;
    }

    private void ConfigurarGrid()
    {
        _grid.Dock = DockStyle.Fill;
        _grid.BackgroundColor = Color.White;
        _grid.BorderStyle = BorderStyle.None;
        _grid.ReadOnly = true;
        _grid.AllowUserToAddRows = false;
        _grid.AllowUserToDeleteRows = false;
        _grid.MultiSelect = false;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.AutoGenerateColumns = false;
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _grid.RowHeadersVisible = false;
        _grid.ColumnHeadersDefaultCellStyle.BackColor = Estilos.AzulPrincipal;
        _grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        _grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        _grid.EnableHeadersVisualStyles = false;

        _grid.Columns.AddRange(
            CrearColumna("Id", "ID", 80),
            CrearColumna("NombreCompleto", "Nombre completo", 165),
            CrearColumna("Edad", "Edad", 55),
            CrearColumna("Sexo", "Sexo", 80),
            CrearColumna("Diagnostico", "Diagnóstico", 160),
            CrearColumna("Estado", "Estado", 100),
            CrearColumna("FechaIngreso", "Fecha de ingreso", 105, "dd/MM/yyyy"));
    }

    private static DataGridViewTextBoxColumn CrearColumna(
        string propiedad, string titulo, int ancho, string? formato = null)
    {
        return new DataGridViewTextBoxColumn
        {
            DataPropertyName = propiedad,
            HeaderText = titulo,
            MinimumWidth = ancho,
            DefaultCellStyle = new DataGridViewCellStyle { Format = formato ?? string.Empty }
        };
    }

    private void PrepararVistaInicial()
    {
        MostrarTodos(false);
        if (_modo == ModoConsulta.Buscar)
            _txtCriterio.Focus();
    }

    private void MostrarTodos(bool preguntar)
    {
        ActualizarGrid(_gestor.ObtenerTodos());
        if (preguntar)
            PreguntarOtraOperacion("listar");
    }

    private void BuscarPacientes()
    {
        try
        {
            List<Paciente> resultados = _gestor.Buscar(_txtCriterio.Text);
            ActualizarGrid(resultados);

            if (resultados.Count == 0)
                MessageBox.Show("No se encontraron pacientes con ese criterio.", "Sin resultados",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

            PreguntarOtraOperacion("buscar");
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error de búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void EliminarSeleccionado()
    {
        try
        {
            if (_grid.CurrentRow?.DataBoundItem is not Paciente paciente)
                throw new InvalidOperationException("Seleccione un paciente de la tabla.");

            DialogResult confirmacion = MessageBox.Show(
                $"¿Está seguro de eliminar a {paciente.NombreCompleto}?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmacion != DialogResult.Yes)
                return;

            _gestor.Eliminar(paciente.Id);
            MostrarTodos(false);
            MessageBox.Show("Paciente eliminado correctamente.", "Operación exitosa",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            PreguntarOtraOperacion("eliminar");
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "No se pudo eliminar", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ActualizarGrid(List<Paciente> pacientes)
    {
        _grid.DataSource = null;
        _grid.DataSource = pacientes;
        _grid.ClearSelection();
    }

    private void PreguntarOtraOperacion(string accion)
    {
        DialogResult respuesta = MessageBox.Show(
            $"¿Desea {accion} pacientes nuevamente?",
            "Continuar",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (respuesta == DialogResult.Yes)
        {
            _txtCriterio.Clear();
            _txtCriterio.Focus();
        }
        else
        {
            Close();
        }
    }
}

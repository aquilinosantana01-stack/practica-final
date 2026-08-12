using SistemaGestionPacientes.Core.Excepciones;
using SistemaGestionPacientes.Core.Modelos;
using SistemaGestionPacientes.Core.Servicios;

namespace SistemaGestionPacientes.WinForms.Formularios;

public class FormPaciente : Form
{
    private readonly GestorPacientes _gestor;
    private readonly bool _esEdicion;
    private string? _idOriginal;

    private readonly TextBox _txtBuscarId = new();
    private readonly TextBox _txtId = new();
    private readonly TextBox _txtNombre = new();
    private readonly TextBox _txtEdad = new();
    private readonly ComboBox _cmbSexo = new();
    private readonly TextBox _txtDiagnostico = new();
    private readonly ComboBox _cmbEstado = new();
    private readonly DateTimePicker _dtpFechaIngreso = new();
    private readonly Button _btnGuardar;

    public FormPaciente(GestorPacientes gestor, bool esEdicion)
    {
        _gestor = gestor;
        _esEdicion = esEdicion;
        _btnGuardar = Estilos.CrearBoton(esEdicion ? "Guardar cambios" : "Registrar paciente");

        Text = esEdicion ? "Actualizar paciente" : "Registrar paciente";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(720, 650);
        MinimumSize = new Size(680, 620);
        BackColor = Color.White;
        ConstruirInterfaz();
    }

    private void ConstruirInterfaz()
    {
        Controls.Add(Estilos.CrearTitulo(_esEdicion ? "Actualizar paciente" : "Registrar nuevo paciente"));

        var contenedor = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(45, 20, 45, 25)
        };

        var tabla = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            ColumnCount = 2,
            RowCount = _esEdicion ? 9 : 8,
            Padding = new Padding(5),
            BackColor = Color.White
        };
        tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32));
        tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 68));

        int fila = 0;
        if (_esEdicion)
        {
            var panelBusqueda = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
            _txtBuscarId.Width = 230;
            Button btnBuscar = Estilos.CrearBoton("Cargar", Color.FromArgb(65, 119, 81));
            btnBuscar.Width = 110;
            btnBuscar.Height = 34;
            btnBuscar.Margin = new Padding(8, 0, 0, 0);
            btnBuscar.Click += (_, _) => BuscarParaEditar();
            panelBusqueda.Controls.AddRange(new Control[] { _txtBuscarId, btnBuscar });
            AgregarFila(tabla, fila++, "ID a buscar:", panelBusqueda);
        }

        ConfigurarEntrada(_txtId);
        ConfigurarEntrada(_txtNombre);
        ConfigurarEntrada(_txtEdad);
        ConfigurarEntrada(_txtDiagnostico);
        _txtDiagnostico.Multiline = true;
        _txtDiagnostico.Height = 62;

        ConfigurarCombo(_cmbSexo, Enum.GetValues<Sexo>());
        ConfigurarCombo(_cmbEstado, Enum.GetValues<EstadoPaciente>());
        _dtpFechaIngreso.Format = DateTimePickerFormat.Short;
        _dtpFechaIngreso.MaxDate = DateTime.Today;
        _dtpFechaIngreso.Dock = DockStyle.Fill;

        AgregarFila(tabla, fila++, "ID o cédula:", _txtId);
        AgregarFila(tabla, fila++, "Nombre completo:", _txtNombre);
        AgregarFila(tabla, fila++, "Edad:", _txtEdad);
        AgregarFila(tabla, fila++, "Sexo:", _cmbSexo);
        AgregarFila(tabla, fila++, "Diagnóstico:", _txtDiagnostico);
        AgregarFila(tabla, fila++, "Estado:", _cmbEstado);
        AgregarFila(tabla, fila++, "Fecha de ingreso:", _dtpFechaIngreso);

        var panelAcciones = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 70,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(0, 10, 0, 0)
        };
        Button btnCancelar = Estilos.CrearBoton("Volver al menú", Color.Gray);
        btnCancelar.Width = 170;
        _btnGuardar.Width = 180;
        _btnGuardar.Click += (_, _) => ProcesarPaciente();
        btnCancelar.Click += (_, _) => Close();
        panelAcciones.Controls.AddRange(new Control[] { btnCancelar, _btnGuardar });

        contenedor.Controls.Add(panelAcciones);
        contenedor.Controls.Add(tabla);
        panelAcciones.BringToFront();
        Controls.Add(contenedor);
        contenedor.BringToFront();

        if (_esEdicion)
            HabilitarCampos(false);
    }

    private static void ConfigurarEntrada(TextBox caja)
    {
        caja.Dock = DockStyle.Fill;
        caja.Font = new Font("Segoe UI", 10F);
    }

    private static void ConfigurarCombo<T>(ComboBox combo, T[] valores)
    {
        combo.Dock = DockStyle.Fill;
        combo.DropDownStyle = ComboBoxStyle.DropDownList;
        combo.DataSource = valores;
    }

    private static void AgregarFila(TableLayoutPanel tabla, int fila, string texto, Control control)
    {
        tabla.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        var etiqueta = new Label
        {
            Text = texto,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            Padding = new Padding(0, 8, 0, 8)
        };
        control.Margin = new Padding(5, 7, 5, 7);
        tabla.Controls.Add(etiqueta, 0, fila);
        tabla.Controls.Add(control, 1, fila);
    }

    private void BuscarParaEditar()
    {
        try
        {
            Paciente paciente = _gestor.BuscarPorId(_txtBuscarId.Text);
            _idOriginal = paciente.Id;
            CargarPaciente(paciente);
            HabilitarCampos(true);
        }
        catch (PacienteNoEncontradoException ex)
        {
            MessageBox.Show(ex.Message, "Paciente no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ProcesarPaciente()
    {
        _btnGuardar.Enabled = false;
        try
        {
            Paciente paciente = CrearPacienteDesdeFormulario();

            if (_esEdicion)
            {
                if (string.IsNullOrWhiteSpace(_idOriginal))
                    throw new InvalidOperationException("Primero debe cargar un paciente para actualizarlo.");
                _gestor.Actualizar(_idOriginal, paciente);
            }
            else
            {
                _gestor.Registrar(paciente);
            }

            string operacion = _esEdicion ? "actualizado" : "registrado";
            MessageBox.Show($"Paciente {operacion} correctamente.", "Operación exitosa",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            PreguntarOtraOperacion();
        }
        catch (FormatException)
        {
            MessageBox.Show("La edad debe contener únicamente un número entero.", "Dato inválido",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (ArgumentException ex)
        {
            MessageBox.Show(ex.Message, "Datos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "No se pudo completar la operación",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            _btnGuardar.Enabled = true;
        }
    }

    private Paciente CrearPacienteDesdeFormulario()
    {
        if (!int.TryParse(_txtEdad.Text.Trim(), out int edad))
            throw new FormatException();

        return new Paciente
        {
            Id = _txtId.Text,
            NombreCompleto = _txtNombre.Text,
            Edad = edad,
            Sexo = (Sexo)_cmbSexo.SelectedItem!,
            Diagnostico = _txtDiagnostico.Text,
            Estado = (EstadoPaciente)_cmbEstado.SelectedItem!,
            FechaIngreso = _dtpFechaIngreso.Value.Date
        };
    }

    private void CargarPaciente(Paciente paciente)
    {
        _txtId.Text = paciente.Id;
        _txtNombre.Text = paciente.NombreCompleto;
        _txtEdad.Text = paciente.Edad.ToString();
        _cmbSexo.SelectedItem = paciente.Sexo;
        _txtDiagnostico.Text = paciente.Diagnostico;
        _cmbEstado.SelectedItem = paciente.Estado;
        _dtpFechaIngreso.Value = paciente.FechaIngreso;
    }

    private void HabilitarCampos(bool habilitados)
    {
        foreach (Control control in new Control[]
        {
            _txtId, _txtNombre, _txtEdad, _cmbSexo,
            _txtDiagnostico, _cmbEstado, _dtpFechaIngreso, _btnGuardar
        })
            control.Enabled = habilitados;
    }

    private void PreguntarOtraOperacion()
    {
        string accion = _esEdicion ? "actualizar" : "registrar";
        DialogResult respuesta = MessageBox.Show(
            $"¿Desea {accion} otro paciente?",
            "Continuar",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (respuesta == DialogResult.Yes)
            LimpiarFormulario();
        else
            Close();
    }

    private void LimpiarFormulario()
    {
        _idOriginal = null;
        _txtBuscarId.Clear();
        _txtId.Clear();
        _txtNombre.Clear();
        _txtEdad.Clear();
        _txtDiagnostico.Clear();
        _cmbSexo.SelectedIndex = 0;
        _cmbEstado.SelectedIndex = 0;
        _dtpFechaIngreso.Value = DateTime.Today;
        if (_esEdicion)
            HabilitarCampos(false);
        else
            _txtId.Focus();
    }
}

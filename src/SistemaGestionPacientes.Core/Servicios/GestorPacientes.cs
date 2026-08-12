using SistemaGestionPacientes.Core.Excepciones;
using SistemaGestionPacientes.Core.Modelos;

namespace SistemaGestionPacientes.Core.Servicios;

/// <summary>Administra en memoria las operaciones CRUD de pacientes.</summary>
public class GestorPacientes
{
    private readonly List<Paciente> _pacientes = new();

    public void Registrar(Paciente paciente)
    {
        ValidarPaciente(paciente);

        if (_pacientes.Any(p => p.Id.Equals(paciente.Id, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("Ya existe un paciente registrado con ese ID.");

        _pacientes.Add(paciente);
    }

    public List<Paciente> ObtenerTodos()
    {
        return _pacientes
            .OrderBy(p => p.NombreCompleto)
            .ToList();
    }

    public Paciente BuscarPorId(string id)
    {
        Paciente? paciente = _pacientes.FirstOrDefault(
            p => p.Id.Equals(id.Trim(), StringComparison.OrdinalIgnoreCase));

        return paciente ?? throw new PacienteNoEncontradoException(id);
    }

    public List<Paciente> Buscar(string criterio)
    {
        if (string.IsNullOrWhiteSpace(criterio))
            return ObtenerTodos();

        string texto = criterio.Trim();
        return _pacientes
            .Where(p => p.Id.Contains(texto, StringComparison.OrdinalIgnoreCase)
                     || p.NombreCompleto.Contains(texto, StringComparison.OrdinalIgnoreCase))
            .OrderBy(p => p.NombreCompleto)
            .ToList();
    }

    public void Actualizar(string idOriginal, Paciente datosActualizados)
    {
        ValidarPaciente(datosActualizados);
        Paciente pacienteExistente = BuscarPorId(idOriginal);

        bool idDuplicado = _pacientes.Any(p =>
            !ReferenceEquals(p, pacienteExistente)
            && p.Id.Equals(datosActualizados.Id, StringComparison.OrdinalIgnoreCase));

        if (idDuplicado)
            throw new InvalidOperationException("El nuevo ID ya pertenece a otro paciente.");

        pacienteExistente.Id = datosActualizados.Id;
        pacienteExistente.NombreCompleto = datosActualizados.NombreCompleto;
        pacienteExistente.Edad = datosActualizados.Edad;
        pacienteExistente.Sexo = datosActualizados.Sexo;
        pacienteExistente.Diagnostico = datosActualizados.Diagnostico;
        pacienteExistente.Estado = datosActualizados.Estado;
        pacienteExistente.FechaIngreso = datosActualizados.FechaIngreso;
    }

    public void Eliminar(string id)
    {
        Paciente paciente = BuscarPorId(id);
        _pacientes.Remove(paciente);
    }

    private static void ValidarPaciente(Paciente paciente)
    {
        if (paciente is null)
            throw new ArgumentNullException(nameof(paciente));
        if (string.IsNullOrWhiteSpace(paciente.Id))
            throw new ArgumentException("El ID o la cédula es obligatorio.");
        if (string.IsNullOrWhiteSpace(paciente.NombreCompleto))
            throw new ArgumentException("El nombre completo es obligatorio.");
        if (paciente.Edad < 0 || paciente.Edad > 120)
            throw new ArgumentOutOfRangeException(nameof(paciente.Edad), "La edad debe estar entre 0 y 120 años.");
        if (string.IsNullOrWhiteSpace(paciente.Diagnostico))
            throw new ArgumentException("El diagnóstico es obligatorio.");
        if (paciente.FechaIngreso.Date > DateTime.Today)
            throw new ArgumentException("La fecha de ingreso no puede ser futura.");

        paciente.Id = paciente.Id.Trim();
        paciente.NombreCompleto = paciente.NombreCompleto.Trim();
        paciente.Diagnostico = paciente.Diagnostico.Trim();
    }
}

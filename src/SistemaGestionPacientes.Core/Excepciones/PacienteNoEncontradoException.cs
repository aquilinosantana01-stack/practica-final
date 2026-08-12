namespace SistemaGestionPacientes.Core.Excepciones;

/// <summary>Se produce cuando no existe un paciente con el identificador solicitado.</summary>
public class PacienteNoEncontradoException : Exception
{
    public PacienteNoEncontradoException(string id)
        : base($"No se encontró un paciente con el ID '{id}'.")
    {
    }
}

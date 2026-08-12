namespace SistemaGestionPacientes.Core.Modelos;

/// <summary>Valores permitidos para el sexo del paciente.</summary>
public enum Sexo
{
    Masculino,
    Femenino
}

/// <summary>Situaciones posibles de un paciente dentro del centro de salud.</summary>
public enum EstadoPaciente
{
    Ingresado,
    EnObservacion,
    DeAlta,
    Hospitalizado
}

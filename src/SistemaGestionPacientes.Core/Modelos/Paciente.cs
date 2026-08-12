namespace SistemaGestionPacientes.Core.Modelos;

/// <summary>Representa la información de un paciente.</summary>
public class Paciente
{
    public string Id { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public int Edad { get; set; }
    public Sexo Sexo { get; set; }
    public string Diagnostico { get; set; } = string.Empty;
    public EstadoPaciente Estado { get; set; }
    public DateTime FechaIngreso { get; set; }
}

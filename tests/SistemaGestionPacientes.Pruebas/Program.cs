using SistemaGestionPacientes.Core.Excepciones;
using SistemaGestionPacientes.Core.Modelos;
using SistemaGestionPacientes.Core.Servicios;

var gestor = new GestorPacientes();
var paciente = new Paciente
{
    Id = "001-0000001-1",
    NombreCompleto = "Paciente de Prueba",
    Edad = 25,
    Sexo = Sexo.Masculino,
    Diagnostico = "Evaluación general",
    Estado = EstadoPaciente.EnObservacion,
    FechaIngreso = DateTime.Today
};

gestor.Registrar(paciente);
Verificar(gestor.ObtenerTodos().Count == 1, "Registrar paciente");
Verificar(gestor.Buscar("Prueba").Count == 1, "Buscar por nombre");
Verificar(gestor.BuscarPorId(paciente.Id).NombreCompleto == paciente.NombreCompleto, "Buscar por ID");

var actualizado = new Paciente
{
    Id = paciente.Id,
    NombreCompleto = "Paciente Actualizado",
    Edad = 26,
    Sexo = Sexo.Masculino,
    Diagnostico = "Paciente estable",
    Estado = EstadoPaciente.DeAlta,
    FechaIngreso = DateTime.Today
};
gestor.Actualizar(paciente.Id, actualizado);
Verificar(gestor.BuscarPorId(paciente.Id).Estado == EstadoPaciente.DeAlta, "Actualizar paciente");

bool duplicadoControlado = false;
try
{
    gestor.Registrar(actualizado);
}
catch (InvalidOperationException)
{
    duplicadoControlado = true;
}
Verificar(duplicadoControlado, "Validar ID duplicado");

gestor.Eliminar(paciente.Id);
Verificar(gestor.ObtenerTodos().Count == 0, "Eliminar paciente");

bool inexistenteControlado = false;
try
{
    gestor.BuscarPorId("NO-EXISTE");
}
catch (PacienteNoEncontradoException)
{
    inexistenteControlado = true;
}
Verificar(inexistenteControlado, "Excepción de paciente no encontrado");

Console.WriteLine("Todas las pruebas de la lógica CRUD terminaron correctamente.");

static void Verificar(bool condicion, string prueba)
{
    if (!condicion)
        throw new Exception($"Falló la prueba: {prueba}");
    Console.WriteLine($"[OK] {prueba}");
}

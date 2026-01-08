using System.Diagnostics;
using System.IO;

namespace Aula013._0.Services
{
    // Esta clase será responsable de iniciar el juego Godot
    public class GameLauncherService
    {
        public void LaunchGodotGame(string gameExecutablePath, string arguments = "")
        {
            // Verificación para asegurar que el archivo exista antes de intentar ejecutarlo
            if (!File.Exists(gameExecutablePath))
            {
                // En un caso real, aquí deberías loggear el error o notificar a la UI
                // Para este ejemplo, solo lanzamos una excepción.
                throw new FileNotFoundException("No se encontró el ejecutable del juego Godot.", gameExecutablePath);
            }

            try
            {
                // Prepara la información del proceso
                var startInfo = new ProcessStartInfo(gameExecutablePath, arguments)
                {
                    // Opcional: Usar ShellExecute es a menudo necesario para 
                    // ejecutar aplicaciones en Windows desde el código.
                    UseShellExecute = true,

                    // Opcional: No mostrar la ventana de comandos negra.
                    CreateNoWindow = true
                };

    
                Process.Start(startInfo);

                Console.WriteLine($"Juego Godot lanzado con éxito: {gameExecutablePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al intentar lanzar el juego de Godot: {ex.Message}");
                // Puedes relanzar el error para que sea manejado por quien llama al servicio
                throw new Exception($"Error al ejecutar Godot: {ex.Message}", ex);
            }
        }
    }
}
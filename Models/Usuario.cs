using SQLite; // Importante: Usamos SQLite, no EntityFramework

namespace Aula013._0.Models
{
    public class Usuario
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Unique] // No permite emails repetidos
        public string Email { get; set; }
        public string Nombre { get; set; }
        public string Password { get; set; } // Guardaremos la contraseña simple por ahora para no complicarnos con Hashes
    }
}
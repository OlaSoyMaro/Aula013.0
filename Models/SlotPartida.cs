using SQLite;

namespace Aula013._0.Models
{
    public class SlotPartida
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Nombre { get; set; }
        public string TiempoJugado { get; set; }
        public string Ubicacion { get; set; }

        public int UsuarioId { get; set; } // Para saber de quién es la partida
    }
}
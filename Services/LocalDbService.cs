using SQLite;
using Aula013._0.Models;

namespace Aula013._0.Services
{
    public class LocalDbService
    {
        private const string DB_NAME = "aula013_local.db3";
        private readonly SQLiteAsyncConnection _connection;

        public LocalDbService()
        {
            // Esto busca la carpeta correcta en Android o Windows automáticamente
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, DB_NAME);

            _connection = new SQLiteAsyncConnection(dbPath);

            // Crea las tablas si no existen (la primera vez que abres la app)
            _connection.CreateTableAsync<Usuario>().Wait();
            _connection.CreateTableAsync<SlotPartida>().Wait();
        }

        // --- MÉTODOS DE USUARIO ---

        public async Task<Usuario> Login(string email, string password)
        {
            // Busca un usuario que coincida en email y password
            return await _connection.Table<Usuario>()
                            .Where(u => u.Email == email && u.Password == password)
                            .FirstOrDefaultAsync();
        }

        public async Task<bool> Registrar(string nombre, string email, string password)
        {
            var existe = await _connection.Table<Usuario>()
                                .Where(u => u.Email == email)
                                .CountAsync();

            if (existe > 0) return false; // Ya existe el email

            var nuevoUsuario = new Usuario
            {
                Nombre = nombre,
                Email = email,
                Password = password
            };

            await _connection.InsertAsync(nuevoUsuario);
            return true;
        }

        // --- MÉTODOS DE SLOTS (PARTIDAS) ---

        public async Task<List<SlotPartida>> ObtenerSlots(int usuarioId)
        {
            return await _connection.Table<SlotPartida>()
                            .Where(s => s.UsuarioId == usuarioId)
                            .ToListAsync();
        }

        public async Task GuardarSlot(SlotPartida slot)
        {
            if (slot.Id == 0)
                await _connection.InsertAsync(slot); // Es nuevo
            else
                await _connection.UpdateAsync(slot); // Ya existe, actualizamos
        }
    }
}
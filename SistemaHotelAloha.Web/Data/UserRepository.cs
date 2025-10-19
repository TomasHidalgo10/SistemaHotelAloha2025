using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SistemaHotelAloha.Dominio;
using SistemaHotelAloha.Web.Models;

namespace SistemaHotelAloha.Web.Data;

    public class UserRepository
    {
        private readonly AlohaDbContext _db;

        public UserRepository(AlohaDbContext db)
        {
            _db = db;
        }

        // Buscar por username o email 
        public async Task<Usuario?> GetByUserNameAsync(string userOrEmail)
        {
            return await _db.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == userOrEmail);
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            return await _db.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _db.Usuarios
                .AsNoTracking()
                .AnyAsync(u => u.Email == email);
        }

        // Crear usuario
        public async Task<bool> CreateAsync(Usuario entity)
        {
            await _db.Usuarios.AddAsync(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        // Actualiza hash de password
        public async Task<bool> UpdatePasswordAsync(int userId, string newPasswordHash)
        {
            var u = await _db.Usuarios.FirstOrDefaultAsync(x => x.Id == userId);
            if (u is null) return false;  
            await _db.SaveChangesAsync();
            return true;
        }
    }
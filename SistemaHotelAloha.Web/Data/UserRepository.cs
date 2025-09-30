using Microsoft.Data.SqlClient;
using SistemaHotelAloha.Web.Models;

namespace SistemaHotelAloha.Web.Data;

public class UserRepository
{
    private readonly Db _db;
    public UserRepository(Db db) => _db = db;

    public async Task<User?> GetByUserNameAsync(string userName)
    {
        using var conn = _db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT TOP 1 Id, UserName, Email, PasswordHash, Salt, CreatedAt FROM dbo.Users WHERE UserName = @u";
        var p = cmd.CreateParameter(); p.ParameterName = "@u"; p.Value = userName; cmd.Parameters.Add(p);
        using var reader = await ((SqlCommand)cmd).ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new User
            {
                Id = reader.GetInt32(0),
                UserName = reader.GetString(1),
                Email = reader.IsDBNull(2) ? null : reader.GetString(2),
                PasswordHash = (byte[])reader[3],
                Salt = (byte[])reader[4],
                CreatedAt = reader.GetDateTime(5)
            };
        }
        return null;
    }

    public async Task<int> CreateAsync(User u)
    {
        using var conn = _db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"INSERT INTO dbo.Users(UserName, Email, PasswordHash, Salt, CreatedAt)
                            VALUES(@u, @e, @ph, @s, SYSUTCDATETIME());
                            SELECT CAST(SCOPE_IDENTITY() AS INT);";
        var p1 = cmd.CreateParameter(); p1.ParameterName = "@u"; p1.Value = u.UserName; cmd.Parameters.Add(p1);
        var p2 = cmd.CreateParameter(); p2.ParameterName = "@e"; p2.Value = (object?)u.Email ?? DBNull.Value; cmd.Parameters.Add(p2);
        var p3 = cmd.CreateParameter(); p3.ParameterName = "@ph"; p3.Value = u.PasswordHash; cmd.Parameters.Add(p3);
        var p4 = cmd.CreateParameter(); p4.ParameterName = "@s"; p4.Value = u.Salt; cmd.Parameters.Add(p4);
        var newId = (int)(await ((SqlCommand)cmd).ExecuteScalarAsync() ?? 0);
        return newId;
    }
}

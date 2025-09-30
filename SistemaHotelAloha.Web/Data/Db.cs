using Microsoft.Data.SqlClient;
using System.Data;

namespace SistemaHotelAloha.Web.Data;

public class Db
{
    private readonly IConfiguration _cfg;
    public Db(IConfiguration cfg) => _cfg = cfg;

    public IDbConnection Open()
    {
        var cs = _cfg.GetConnectionString("DefaultConnection")
                 ?? throw new InvalidOperationException("Missing ConnectionStrings:DefaultConnection");
        var cn = new SqlConnection(cs);
        cn.Open();
        return cn;
    }
}

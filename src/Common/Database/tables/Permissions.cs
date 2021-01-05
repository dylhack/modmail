using Npgsql;
using System.Threading.Tasks;
using Modmail.Models;

namespace Modmail.Database.Tables
{
  public class Permissions : Table<Role>
  {
    const string INIT = @"
    CREATE TABLE IF NOT EXISTS modmail.permissions (
      category_id BIGINT NOT NULL
        REFERENCES modmail.categories,
      role_id TEXT UNIQUE NOT NULL,
      level modmail.role_level DEFAULT 'mod'::modmail.role_level NOT NULL);";

    public Permissions(ref string connStr) : base("Permissions", connStr)
    {}

    protected override async Task Prepare()
    {
      NpgsqlConnection connection = await GetConnection();
      NpgsqlCommand cmd = new NpgsqlCommand(INIT, connection);

      await cmd.ExecuteNonQueryAsync();
    }
  }

}

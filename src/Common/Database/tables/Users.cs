using Npgsql;
using System.Threading.Tasks;
using Modmail.Models;

namespace Modmail.Database.Tables
{
  public class Users : Table<ModmailUser>
  {
    const string INIT = @"
    CREATE TABLE IF NOT EXISTS modmail.users (
      id BIGINT NOT NULL
        CONSTRAINT users_pk PRIMARY KEY);";

    const string INIT_ID_UINDEX = @"
    CREATE UNIQUE INDEX IF NOT EXISTS users_id_uindex
      ON modmail.users (id);";

    public Users(ref string connStr) : base("Users", connStr)
    {}

    protected override async Task Prepare()
    {
      NpgsqlConnection connection = await GetConnection();

      await new NpgsqlCommand(INIT, connection)
        .ExecuteNonQueryAsync();

      await new NpgsqlCommand(INIT_ID_UINDEX, connection)
        .ExecuteNonQueryAsync();
    }
  }
}

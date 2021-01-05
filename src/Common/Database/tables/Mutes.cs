using Npgsql;
using System.Threading.Tasks;
using Modmail.Models;

namespace Modmail.Database.Tables
{
  public class Mutes : Table<MuteStatus>
  {
    const string INIT = @"
    CREATE TABLE IF NOT EXISTS modmail.mutes (
      user_id BIGINT NOT NULL
        CONSTRAINT threads_users_id_fk
        REFERENCES modmail.users,
      till BIGINT NOT NULL,
        category_id BIGINT NOT NULL,
      reason TEXT NOT NULL);";

    public Mutes(ref string connStr) : base("Mutes", connStr)
    {}

    protected override async Task Prepare()
    {
      NpgsqlConnection connection = await GetConnection();
      NpgsqlCommand cmd = new NpgsqlCommand(INIT, connection);

      await cmd.ExecuteNonQueryAsync();
    }
  }
}

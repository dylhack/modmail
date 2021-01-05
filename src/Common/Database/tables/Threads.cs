using Npgsql;
using System.Threading.Tasks;
using Modmail.Models;

namespace Modmail.Database.Tables
{
  public class Threads : Table<Thread>
  {
    const string INIT = @"
    CREATE TABLE IF NOT EXISTS modmail.threads (
      id BIGINT NOT NULL
        CONSTRAINT threads_pk PRIMARY KEY,
      author BIGINT NOT NULL
        CONSTRAINT threads_users_id_fk
        REFERENCES modmail.users,
      channel BIGINT NOT NULL,
      is_active BOOLEAN DEFAULT true NOT NULL,
      category BIGINT NOT NULL
        CONSTRAINT threads_categories_id_fk
        REFERENCES modmail.categories);";

    const string INIT_CHANNEL_UINDEX = @"
    CREATE UNIQUE INDEX IF NOT EXISTS threads_channel_uindex
      ON modmail.threads (channel);";

    const string INIT_ID_UINDEX = @"
    CREATE UNIQUE INDEX IF NOT EXISTS threads_id_uindex
      ON modmail.threads (id);";

    public Threads(ref string connStr) : base("Threads", connStr)
    {}

    protected override async Task Prepare()
    {
      NpgsqlConnection connection = await GetConnection();

      await new NpgsqlCommand(INIT, connection)
        .ExecuteNonQueryAsync();

      await new NpgsqlCommand(
        INIT_CHANNEL_UINDEX,
        connection).ExecuteNonQueryAsync();

      await new NpgsqlCommand(
        INIT_ID_UINDEX,
        connection).ExecuteNonQueryAsync();
    }
  }
}

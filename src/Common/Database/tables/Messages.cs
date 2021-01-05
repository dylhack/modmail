using Npgsql;
using System.Threading.Tasks;
using Modmail.Models;

namespace Modmail.Database.Tables
{
  public class Messages : Table<Message>
  {
    const string INIT = @"
    CREATE TABLE IF NOT EXISTS modmail.messages (
      sender BIGINT NOT NULL
        CONSTRAINT messages_users_id_fk
        REFERENCES modmail.users,
      client_id BIGINT,
      modmail_id BIGINT NOT NULL,
      content TEXT NOT NULL,
      thread_id BIGINT NOT NULL
        CONSTRAINT messages_threads_id_fk
        REFERENCES modmail.threads,
      is_deleted BOOLEAN DEFAULT false NOT NULL,
      internal BOOLEAN DEFAULT false NOT NULL);";

    const string INIT_CLID_UINDEX = @"
    CREATE UNIQUE INDEX IF NOT EXISTS messages_client_id_uindex
      ON modmail.messages (client_id);";

    const string INIT_MMID_UINDEX = @"
    CREATE UNIQUE INDEX IF NOT EXISTS messages_modmail_id_uindex
      ON modmail.messages (modmail_id);";

    public Messages(ref string connStr) : base("Messages", connStr)
    {}

    protected override async Task Prepare()
    {
      NpgsqlConnection connection = await GetConnection();

      await new NpgsqlCommand(INIT, connection)
        .ExecuteNonQueryAsync();

      await new NpgsqlCommand(
        INIT_CLID_UINDEX,
        connection).ExecuteNonQueryAsync();

      await new NpgsqlCommand(
        INIT_MMID_UINDEX,
        connection).ExecuteNonQueryAsync();
    }
  }
}

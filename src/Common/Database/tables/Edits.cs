using Npgsql;
using System.Threading.Tasks;
using Modmail.Models;

namespace Modmail.Database.Tables
{
  public class Edits : Table<Edit>
  {
    const string INIT = @"
    CREATE TABLE IF NOT EXISTS modmail.edits (
      content TEXT NOT NULL,
      message BIGINT NOT NULL
        CONSTRAINT edits_messages_modmail_id_fk
        REFERENCES modmail.messages (modmail_id),
      version INTEGER DEFAULT 1 NOT NULL)";

    public Edits(ref string connStr) : base("Edits", connStr)
    {}

    protected override async Task Prepare()
    {
      NpgsqlConnection connection = await GetConnection();
      NpgsqlCommand cmd = new NpgsqlCommand(INIT, connection);

      await cmd.ExecuteNonQueryAsync();
    }
  }

}

using Npgsql;
using System.Threading.Tasks;

namespace Modmail.Database.Tables
{
  public class Attachments : Table
  {
    const string INIT = @"
    CREATE TABLE IF NOT EXISTS modmail.attachments (
      id BIGINT NOT NULL
        CONSTRAINT attachments_pk PRIMARY KEY,
      message_id BIGINT NOT NULL
        CONSTRAINT attachments_messages_modmail_id_fk
        REFERENCES modmail.messages (modmail_id),
      name TEXT NOT NULL,
      source TEXT NOT NULL,
      sender BIGINT NOT NULL
        CONSTRAINT attachments_users_id_fk
        REFERENCES modmail.users,
      file_type modmail.file_type DEFAULT 'file'::modmail.file_type NOT NULL);";

    public Attachments(ref string connStr) : base("Attachments", connStr)
    {}

    protected override async Task Prepare()
    {
      NpgsqlConnection connection = await GetConnection();

      await new NpgsqlCommand(
        INIT,
        connection).ExecuteNonQueryAsync();
    }
  }
}

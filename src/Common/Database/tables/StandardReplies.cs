using Npgsql;
using System.Threading.Tasks;

namespace Modmail.Database.Tables
{
  public class StandardReplies : Table
  {
    const string INIT = @"
    CREATE TABLE IF NOT EXISTS modmail.standard_replies (
      id BIGINT NOT NULL
        CONSTRAINT standard_replies_pk PRIMARY KEY,
      name TEXT NOT NULL,
      reply TEXT NOT NULL);";

    const string INIT_ID_UINDEX = @"
    CREATE UNIQUE INDEX IF NOT EXISTS standard_replies_id_uindex
      ON modmail.standard_replies (id);";

    const string INIT_NAME_UINDEX = @"
    CREATE UNIQUE INDEX IF NOT EXISTS standard_replies_name_uindex
      ON modmail.standard_replies (name);";

    public StandardReplies(ref string connStr) : base("StandardReplies", connStr)
    {}

    protected override async Task Prepare()
    {
      NpgsqlConnection connection = await GetConnection();

      await new NpgsqlCommand(INIT, connection)
        .ExecuteNonQueryAsync();

      await new NpgsqlCommand(
        INIT_ID_UINDEX,
        connection).ExecuteNonQueryAsync();

      await new NpgsqlCommand(
        INIT_NAME_UINDEX,
        connection).ExecuteNonQueryAsync();
    }
  }
}

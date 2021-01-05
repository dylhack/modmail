using Npgsql;
using System.Threading.Tasks;

namespace Modmail.Database.Tables
{
  public class Categories : Table
  {
    const string INIT = @"
    CREATE TABLE IF NOT EXISTS modmail.categories (
      id BIGINT NOT NULL
        CONSTRAINT categories_pk PRIMARY KEY,
      channel_id BIGINT UNIQUE NOT NULL,
      name TEXT NOT NULL,
      is_active BOOLEAN DEFAULT true NOT NULL,
      guild_id BIGINT NOT NULL,
      emoji TEXT NOT NULL);";

    const string INIT_EMOTE_UINDEX = @"
    CREATE UNIQUE INDEX IF NOT EXISTS categories_emote_uindex
      ON modmail.categories (emoji);";

    const string INIT_ID_UINDEX = @"
    CREATE UNIQUE INDEX IF NOT EXISTS categories_id_uindex
      ON modmail.categories (id);";

    const string INIT_NAME_UINDEX = @"
    CREATE UNIQUE INDEX IF NOT EXISTS categories_name_uindex
      ON modmail.categories (name);";

    public Categories(ref string connStr) : base("Categories", connStr)
    {}

    protected override async Task Prepare()
    {
      NpgsqlConnection connection = await GetConnection();

      await new NpgsqlCommand(
        INIT,
        connection).ExecuteNonQueryAsync();

      await new NpgsqlCommand(
        INIT_EMOTE_UINDEX,
        connection).ExecuteNonQueryAsync();

      await new NpgsqlCommand(
        INIT_ID_UINDEX,
        connection).ExecuteNonQueryAsync();

      await new NpgsqlCommand(
        INIT_NAME_UINDEX,
        connection).ExecuteNonQueryAsync();
    }
  }
}

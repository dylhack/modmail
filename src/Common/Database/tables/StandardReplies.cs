using Npgsql;
using System.Threading.Tasks;
using System.Collections.Generic;
using Modmail.Models;

namespace Modmail.Database.Tables
{
  public class StandardReplies : Table<StandardReply>
  {
    private static string COLUMNS =>
    "id, name, reply";
    private static string INSERTION =>
    "@id, @name, @reply";
    private static sbyte OID => 0;
    private static sbyte OName => 1;
    private static sbyte OReply => 2;
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

    /// <summary>
    /// <c>Store</c> store a new standard reply
    /// </summary>
    /// <param name="sr"></param>
    /// <returns>A boolean representing whether or not it was stored
    /// or not.</returns>
    public async Task<bool> Store(StandardReply sr)
    {
      NpgsqlConnection connection = await GetConnection();
      NpgsqlCommand cmd = new NpgsqlCommand(
        $"INSERT INTO modmail.standard_replies ({COLUMNS}) VALUES ({INSERTION})",
        connection);

      cmd.Parameters.AddWithValue("id", sr.ID);
      cmd.Parameters.AddWithValue("name", sr.Name);
      cmd.Parameters.AddWithValue("reply", sr.Reply);

      return await Execute(cmd);
    }

    /// <summary>
    /// <c>Remove</c> remove a standard reply
    /// </summary>
    /// <param name="srID"></param>
    /// <returns>A boolean representing whether or not it was removed
    /// or not.</returns>
    public async Task<bool> Remove(long srID)
    {
      NpgsqlConnection connection = await GetConnection();
      NpgsqlCommand cmd = new NpgsqlCommand(
        "DELETE FROM modmail.standard_replies WHERE id=@id",
        connection);

      cmd.Parameters.AddWithValue("id", srID);

      return await Execute(cmd);
    }

    /// <summary>
    /// <c>GetAll</c> gets all the standard replies
    /// </summary>
    /// <returns>A list of StandardReplies</returns>
    public async Task<List<StandardReply>> GetAll()
    {
      NpgsqlConnection connection = await GetConnection();
      NpgsqlCommand cmd = new NpgsqlCommand(
        $"SELECT {COLUMNS} FROM modmail.standard_replies",
        connection);

      return await ReadAll(cmd);
    }

    protected override StandardReply Read(NpgsqlDataReader reader)
    {
      return new StandardReply
      {
        ID = reader.GetInt64(OID),
        Name = reader.GetString(OName),
        Reply = reader.GetString(OReply),
      };
    }

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

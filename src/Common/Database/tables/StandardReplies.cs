using MySql.Data.MySqlClient;
using System.Data.Common;
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
      id BIGINT UNSIGNED NOT NULL PRIMARY KEY,
      name VARCHAR(32) NOT NULL,
      reply VARCHAR(2000) NOT NULL);";

    const string INIT_ID_UINDEX = @"
    CREATE UNIQUE INDEX uindex_standard_replies_id
      ON modmail.standard_replies (id);";

    const string INIT_NAME_UINDEX = @"
    CREATE UNIQUE INDEX uindex_standard_replies_name
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
      MySqlConnection connection = await GetConnection();
      MySqlCommand cmd = new MySqlCommand(
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
      MySqlConnection connection = await GetConnection();
      MySqlCommand cmd = new MySqlCommand(
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
      MySqlConnection connection = await GetConnection();
      MySqlCommand cmd = new MySqlCommand(
        $"SELECT {COLUMNS} FROM modmail.standard_replies",
        connection);

      return await ReadAll(cmd);
    }

    protected override StandardReply Read(DbDataReader reader)
    {
      return new StandardReply
      {
        ID = reader.GetFieldValue<ulong>(OID),
        Name = reader.GetString(OName),
        Reply = reader.GetString(OReply),
      };
    }

    protected override async Task Prepare()
    {
      MySqlConnection connection = await GetConnection();

      await new MySqlCommand(INIT, connection)
        .ExecuteNonQueryAsync();

      await new MySqlCommand(
        INIT_ID_UINDEX,
        connection).ExecuteNonQueryAsync();

      await new MySqlCommand(
        INIT_NAME_UINDEX,
        connection).ExecuteNonQueryAsync();
    }
  }
}

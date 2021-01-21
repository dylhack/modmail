using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Threading.Tasks;
using System.Collections.Generic;
using Modmail.Models;

namespace Modmail.Database.Tables
{
  public class Edits : Table<Edit>
  {
    private static string COLUMNS =>
    "content, message_id, version";
    private static string INSERTION =>
    "@content, @message_id, @version";
    private static sbyte OContent => 0;
    private static sbyte OMessageID => 1;
    private static sbyte OVersion => 2;
    const string INIT = @"
    CREATE TABLE IF NOT EXISTS modmail.edits (
    content TEXT NOT NULL,
    message_id BIGINT UNSIGNED NOT NULL,
    FOREIGN KEY (message_id)
        REFERENCES messages (modmail_id),
    version INTEGER DEFAULT 1 NOT NULL
    );";

    public Edits(ref string connStr) : base("Edits", connStr)
    {}

    /// <summary>
    /// <c>GetByMessageID</c> returns a sorted (in ascending order) of all the
    /// edits associated with a given message.
    /// </summary>
    /// <param name="id">Message to refer to</param>
    /// <returns>A list of edits associated with the message</returns>
    public async Task<List<Edit>> GetByMessageID(long id)
    {
      MySqlConnection connection = await GetConnection();
      MySqlCommand cmd = new MySqlCommand(
        $"SELECT {COLUMNS} FROM modmail.edits"
        + " WHERE message_id=@id"
        + " ORDER BY version ASC",
        connection);

      cmd.Parameters.AddWithValue("id", id);

      return await ReadAll(cmd);
    }

    /// <summary>
    /// <c>Store</c> stores an edit
    /// </summary>
    /// <param name="edit">Edit to store</param>
    /// <returns>This method returns a boolean representing whether or not the
    /// edit was stored</returns>
    public async Task<bool> Store(Edit edit)
    {
      MySqlConnection connection = await GetConnection();
      MySqlCommand cmd = new MySqlCommand(
        $"INSERT INTO modmail.edits ({COLUMNS}) VALUES ({INSERTION})",
        connection);

      cmd.Parameters.AddWithValue("content", edit.Content);
      cmd.Parameters.AddWithValue("message_id", edit.MessageID);
      cmd.Parameters.AddWithValue("version", edit.Version);

      return await Execute(cmd);
    }

    protected override Edit Read(DbDataReader reader)
    {
      return new Edit
      {
        Content = reader.GetString(OContent),
        MessageID = reader.GetFieldValue<ulong>(OMessageID),
        Version = reader.GetInt32(OVersion),
      };
    }

    protected override async Task Prepare()
    {
      MySqlConnection connection = await GetConnection();
      MySqlCommand cmd = new MySqlCommand(INIT, connection);

      await cmd.ExecuteNonQueryAsync();
    }
  }

}

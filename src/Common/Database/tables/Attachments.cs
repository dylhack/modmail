using Modmail.Models;
using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Modmail.Database.Tables
{
  public class Attachments : Table<Attachment>
  {
    private static string COLUMNS =>
    "id, message_id, name, source, sender, is_image";
    private static string INSERTION =>
    "@id, @message_id, @name, @source, @sender, @is_image";
    private static sbyte OID => 0;
    private static sbyte OMessageID => 1;
    private static sbyte OName => 2;
    private static sbyte OSource => 3;
    private static sbyte OSender => 4;
    private static sbyte OIsImage => 5;

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
      is_image BOOLEAN NOT NULL);";

    public Attachments(ref string connStr) : base("Attachments", connStr)
    { }

    /// <summary>
    /// <c>Store</c> stores a given attachment.
    /// </summary>
    /// <param name="att">Attachment to store</param>
    /// <returns>This method will return a boolean representing whether or not
    /// the data was stored properly</returns>
    public async Task<bool> Store(Attachment att)
    {
      NpgsqlConnection connection = await GetConnection();
      NpgsqlCommand cmd = new NpgsqlCommand(
        $"INSERT INTO modmail.attachments ({COLUMNS})"
        + $" VALUES ({INSERTION});",
        connection);

      cmd.Parameters.AddWithValue("id", att.ID);
      cmd.Parameters.AddWithValue("mid", att.MessageID);
      cmd.Parameters.AddWithValue("name", att.Name);
      cmd.Parameters.AddWithValue("source", att.Source);
      cmd.Parameters.AddWithValue("sender", att.Sender);
      cmd.Parameters.AddWithValue("is_image", att.IsImage);

      int rows = await cmd.ExecuteNonQueryAsync();

      return rows > 0;
    }

    /// <summary>
    /// <c>GetByMessageID</c> resolves all the messages associated
    /// with a given message.
    /// </summary>
    /// <param name="mID">Message ID (Discord Snowflake)</param>
    /// <returns>A list of attachments</returns>
    public async Task<List<Attachment>> GetByMessageID(long mID)
    {
      NpgsqlConnection connection = await GetConnection();
      NpgsqlCommand cmd = new NpgsqlCommand(
        $"SELECT {COLUMNS} FROM modmail.attachments"
        + " WHERE message_id=@mid",
        connection);

      cmd.Parameters.AddWithValue("mid", mID);

      return await ReadAll(cmd);
    }

    protected override async Task Prepare()
    {
      NpgsqlConnection connection = await GetConnection();

      await new NpgsqlCommand(
        INIT,
        connection).ExecuteNonQueryAsync();
    }

    protected override Attachment Read(NpgsqlDataReader reader)
    {
      return new Attachment
      {
        ID = reader.GetInt64(OID),
        MessageID = reader.GetInt64(OMessageID),
        Name = reader.GetString(OName),
        Source = reader.GetString(OSource),
        Sender = reader.GetInt64(OSender),
        IsImage = reader.GetBoolean(OIsImage),
      };
    }

    protected override async Task<List<Attachment>> ReadAll(NpgsqlCommand cmd)
    {
      List<Attachment> res = new List<Attachment>();
      NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

      while (await reader.ReadAsync())
      {
        Attachment attachment = Read(reader);
        res.Add(attachment);
      }

      return res;
    }
  }
}

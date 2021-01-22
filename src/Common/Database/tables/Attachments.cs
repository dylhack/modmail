using Modmail.Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data.Common;
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
    id BIGINT UNSIGNED NOT NULL,
    CONSTRAINT pk_attachments PRIMARY KEY (id),
    message_id BIGINT UNSIGNED NOT NULL,
    FOREIGN KEY (message_id)
        REFERENCES modmail.messages(modmail_id),
    name TEXT NOT NULL,
    source TEXT NOT NULL,
    sender BIGINT UNSIGNED NOT NULL,
    FOREIGN KEY (sender)
        REFERENCES modmail.users(id),
    is_image BOOLEAN NOT NULL
    );";

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
      MySqlConnection connection = await GetConnection();
      MySqlCommand cmd = new MySqlCommand(
        $"INSERT INTO modmail.attachments ({COLUMNS})"
        + $" VALUES ({INSERTION});",
        connection);

      cmd.Parameters.AddWithValue("id", att.ID);
      cmd.Parameters.AddWithValue("mid", att.MessageID);
      cmd.Parameters.AddWithValue("name", att.Name);
      cmd.Parameters.AddWithValue("source", att.Source);
      cmd.Parameters.AddWithValue("sender", att.Sender);
      cmd.Parameters.AddWithValue("is_image", att.IsImage);


      return await Execute(cmd);
    }

    /// <summary>
    /// <c>GetByMessageID</c> resolves all the messages associated
    /// with a given message.
    /// </summary>
    /// <param name="mID">Message ID (Discord Snowflake)</param>
    /// <returns>A list of attachments</returns>
    public async Task<List<Attachment>> GetByMessageID(long mID)
    {
      MySqlConnection connection = await GetConnection();
      MySqlCommand cmd = new MySqlCommand(
        $"SELECT {COLUMNS} FROM modmail.attachments"
        + " WHERE message_id=@mid",
        connection);

      cmd.Parameters.AddWithValue("mid", mID);

      return await ReadAll(cmd);
    }

    protected override async Task Prepare()
    {
      MySqlConnection connection = await GetConnection();

      await new MySqlCommand(
        INIT,
        connection).ExecuteNonQueryAsync();
    }

    protected override Attachment Read(DbDataReader reader)
    {
      return new Attachment
      {
        ID = reader.GetFieldValue<ulong>(OID),
        MessageID = reader.GetFieldValue<ulong>(OMessageID),
        Name = reader.GetString(OName),
        Source = reader.GetString(OSource),
        Sender = reader.GetFieldValue<ulong>(OSender),
        IsImage = reader.GetBoolean(OIsImage),
      };
    }
  }
}

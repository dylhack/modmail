using Modmail.Models;
using Npgsql;
using System.Collections.Generic;
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
      is_image BOOLEAN NOT NULL);";

    public Attachments(ref string connStr) : base("Attachments", connStr)
    {}

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
        @"INSERT INTO modmail.attachments (
          id,
          message_id,
          name,
          source,
          sender,
          is_image) VALUES (@id, @mid, @name, @source, @sender, @is_image);",
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
      List<Attachment> res = new List<Attachment>();
      NpgsqlConnection connection = await GetConnection();
      NpgsqlCommand cmd = new NpgsqlCommand(
        @"SELECT id, message_id, name, source, sender, is_image
        FROM modmail.attachments
        WHERE message_id=@mid",
        connection);

      cmd.Parameters.AddWithValue("mid", mID);

      NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

      while (await reader.ReadAsync())
      {
        Attachment attachment;
        attachment.ID = reader.GetInt64(0);
        attachment.MessageID = reader.GetInt64(1);
        attachment.Name = reader.GetString(2);
        attachment.Source = reader.GetString(3);
        attachment.Sender = reader.GetInt64(4);
        attachment.IsImage = reader.GetBoolean(5);

        res.Add(attachment);
      }

      return res;
    }

    protected override async Task Prepare()
    {
      NpgsqlConnection connection = await GetConnection();

      await new NpgsqlCommand(
        INIT,
        connection).ExecuteNonQueryAsync();
    }
  }
}

using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Common;
using Modmail.Models;

namespace Modmail.Database.Tables
{
  public class Messages : Table<Message>
  {
    private static string COLUMNS =>
    "sender_id, client_id, modmail_id, content, thread_id, is_deleted, is_internal";
    private static string INSERTION =>
    "@senderID, @client_id, @modmail_id, @content, @thread_id, @is_deleted, @is_internal";
    static sbyte OSender => 0;
    static sbyte OClientID => 1;
    static sbyte OModmailID => 2;
    static sbyte OContent => 3;
    static sbyte OThreadID => 4;
    static sbyte OIsDeleted => 5;
    static sbyte OIsInternal => 6;
    const string INIT = @"
    CREATE TABLE IF NOT EXISTS modmail.messages (
    sender_id BIGINT UNSIGNED NOT NULL,
    FOREIGN KEY (sender_id)
        REFERENCES users (id),
    client_id BIGINT UNSIGNED,
    modmail_id BIGINT UNSIGNED NOT NULL,
    content TEXT NOT NULL,
    thread_id BIGINT UNSIGNED NOT NULL,
    FOREIGN KEY (thread_id)
        REFERENCES threads (id),
    is_deleted BOOLEAN DEFAULT FALSE NOT NULL,
    is_internal BOOLEAN DEFAULT FALSE NOT NULL
    );";

    const string INIT_CLID_UINDEX = @"
    CREATE UNIQUE INDEX uindex_messages_client_id
      ON modmail.messages(client_id);";

    const string INIT_MMID_UINDEX = @"
    CREATE UNIQUE INDEX uindex_messages_modmail_id
      ON modmail.messages(modmail_id);";

    public Messages(ref string connStr) : base("Messages", connStr)
    {}

    /// <summary>
    /// <c>GetLastMessage</c>Get the last message of a sender in a thread
    /// </summary>
    /// <param name="threadID">The thread to refer to</param>
    /// <param name="sender">The sender</param>
    /// <returns>A nullable Message</returns>
    public async Task<Message?> GetLastMessage(long threadID, long sender)
    {
      MySqlConnection connection = await GetConnection();
      MySqlCommand cmd = new MySqlCommand(
        $"SELECT ({COLUMNS}) modmail.messages"
        + " WHERE thread_id = @threadID"
        + " AND sender_id = @senderID"
        + " AND is_deleted = False"
        + " AND is_internal = False",
        connection);

      cmd.Parameters.AddWithValue("threadID", threadID);
      cmd.Parameters.AddWithValue("senderID", sender);

      return await ReadOne(cmd);
    }

    /// <summary>
    /// <c>GetByID</c>
    /// </summary>
    /// <param name="messageID"></param>
    /// <returns>A nullable Message</returns>
    public async Task<Message?> GetByID(long messageID)
    {
      MySqlConnection connection = await GetConnection();
      MySqlCommand cmd = new MySqlCommand(
        $"SELECT {COLUMNS} FROM modmail.messages WHERE message_id=@messageID",
        connection);

      cmd.Parameters.AddWithValue("messageID", messageID);

      return await ReadOne(cmd);
    }

    /// <summary>
    /// <c>GetByThreadID</c> Get all the messages of a thread
    /// </summary>
    /// <param name="threadID">The thread to fetch from</param>
    /// <returns>A list of Messages</returns>
    public async Task<List<Message>> GetByThreadID(long threadID)
    {
      MySqlConnection connection = await GetConnection();
      MySqlCommand cmd = new MySqlCommand(
        $"SELECT {COLUMNS} FROM modmail.messages WHERE thread_id=@threadID",
        connection);

      cmd.Parameters.AddWithValue("threadID", threadID);

      return await ReadAll(cmd);
    }

    /// <summary>
    /// <c>SetDeleted</c> Mark a message as deleted
    /// </summary>
    /// <param name="messageID">Message to mark</param>
    /// <param name="isDeleted">Message's deletion state</param>
    /// <returns>A boolean representing whether or not the message was updated
    /// </returns>
    public async Task<bool> SetDeleted(long messageID, bool isDeleted)
    {
      MySqlConnection connection = await GetConnection();
      MySqlCommand cmd = new MySqlCommand(
        "UPDATE modmail.messages SET is_deleted=@isDeleted"
        + " WHERE message_id=@messageID",
        connection);

      cmd.Parameters.AddWithValue("messageID", messageID);
      cmd.Parameters.AddWithValue("isDeleted", isDeleted);

      return await Execute(cmd);
    }

    /// <summary>
    /// <c>Store</c> store a new message. All edits and files will be ignored.
    /// </summary>
    /// <param name="message">message being stored</param>
    /// <returns>A boolean representing whether or not the message was stored
    /// or not</returns>
    public async Task<bool> Store(Message message)
    {
      MySqlConnection connection = await GetConnection();
      MySqlCommand cmd = new MySqlCommand(
        $"INSERT INTO modmail.messages ({COLUMNS}) VALUES ({INSERTION})",
        connection);

      cmd.Parameters.AddWithValue("sender_id", message.SenderID);
      cmd.Parameters.AddWithValue("client_id", message.ClientID);
      cmd.Parameters.AddWithValue("modmail_id", message.ModmailID);
      cmd.Parameters.AddWithValue("content", message.Content);
      cmd.Parameters.AddWithValue("thread_id", message.ThreadID);
      cmd.Parameters.AddWithValue("is_deleted", message.IsDeleted);
      cmd.Parameters.AddWithValue("is_internal", message.IsInternal);

      return await Execute(cmd);
    }

    protected override Message Read(DbDataReader reader)
    {
      return new Message
      {
        ClientID = reader.GetFieldValue<ulong>(OClientID),
        ModmailID = reader.GetFieldValue<ulong>(OModmailID),
        Content = reader.GetString(OContent),
        Edits = new List<Edit>(),
        Files = new List<Attachment>(),
        IsDeleted = reader.GetBoolean(OIsDeleted),
        IsInternal = reader.GetBoolean(OIsInternal),
        SenderID = reader.GetFieldValue<ulong>(OSender),
        ThreadID = reader.GetFieldValue<ulong>(OThreadID),
      };
    }

    protected override async Task Prepare()
    {
      MySqlConnection connection = await GetConnection();

      await new MySqlCommand(INIT, connection)
        .ExecuteNonQueryAsync();

      await new MySqlCommand(
        INIT_CLID_UINDEX,
        connection).ExecuteNonQueryAsync();

      await new MySqlCommand(
        INIT_MMID_UINDEX,
        connection).ExecuteNonQueryAsync();
    }
  }
}

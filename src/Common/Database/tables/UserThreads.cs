using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Threading.Tasks;
using System.Collections.Generic;
using Modmail.Models;

namespace Modmail.Database.Tables
{
  public class UserThreads : Table<UserThread>
  {
    private static string COLUMNS =>
    "id, author_id, channel_id, is_active, category_id";
    private static string INSERTION =>
    "@id, @authorID, @channelID, @isActive, @categoryID";
    private static sbyte OID => 0;
    private static sbyte OAuthorID => 1;
    private static sbyte OChannelID => 2;
    private static sbyte OIsActive => 3;
    private static sbyte OCategoryID => 4;
    const string INIT = @"
    CREATE TABLE IF NOT EXISTS modmail.threads (
    id BIGINT UNSIGNED NOT NULL PRIMARY KEY,
    author_id BIGINT UNSIGNED NOT NULL,
    FOREIGN KEY (author_id)
        REFERENCES modmail.users (id),
    is_active BOOLEAN DEFAULT TRUE NOT NULL,
    category_id BIGINT UNSIGNED NOT NULL,
    FOREIGN KEY (category_id)
        REFERENCES modmail.categories (id)
    );";

    const string INIT_ID_UINDEX = @"
    CREATE UNIQUE INDEX uindex_threads_id
      ON modmail.threads (id);";

    public UserThreads(ref string connStr) : base("Threads", connStr)
    {}

    /// <summary>
    /// <c>Store</c> store a Thread
    /// </summary>
    /// <param name="thread"></param>
    /// <returns>A boolean representing whether or not it was successfully
    /// stored</returns>
    public async Task<bool> Store(UserThread thread)
    {
      MySqlConnection connection = await GetConnection();
      MySqlCommand cmd = new MySqlCommand(
        $"INSERT INTO modmail.threads ({COLUMNS}) VALUES ({INSERTION})",
        connection);

      cmd.Parameters.AddWithValue("id", thread.ID);
      cmd.Parameters.AddWithValue("authorID", thread.AuthorID);
      cmd.Parameters.AddWithValue("channelID", thread.ChannelID);
      cmd.Parameters.AddWithValue("isActive", thread.IsActive);
      cmd.Parameters.AddWithValue("categoryID", thread.CategoryID);

      return await Execute(cmd);
    }

    /// <summary>
    /// <c>SetActive</c> set whether a thread is active or not
    /// </summary>
    /// <param name="threadID"></param>
    /// <param name="isActive"></param>
    /// <returns>A boolean representing whether or not anything was updated
    /// </returns>
    public async Task<bool> SetActive(long threadID, bool isActive)
    {
      MySqlConnection connection = await GetConnection();
      MySqlCommand cmd = new MySqlCommand(
        "UPDATE modmail.threads SET is_active=@isActive"
        + " WHERE id=@id",
        connection);

      cmd.Parameters.AddWithValue("id", threadID);
      cmd.Parameters.AddWithValue("isActive", isActive);

      return await Execute(cmd);
    }

    /// <summary>
    /// <c>GetByID</c>
    /// </summary>
    /// <param name="threadID"></param>
    /// <returns>A nullable Thread</returns>
    public async Task<UserThread?> GetByID(long threadID)
    {
      MySqlConnection connection = await GetConnection();
      MySqlCommand cmd = new MySqlCommand(
        $"SELECT {COLUMNS} FROM modmail.threads"
        + " WHERE id=@id",
        connection);

      cmd.Parameters.AddWithValue("id", threadID);

      return await ReadOne(cmd);
    }

    /// <summary>
    /// <c>GetByAuthor</c>
    /// </summary>
    /// <param name="authorID"></param>
    /// <param name="isActive"></param>
    /// <returns>A nullable Thread</returns>
    public async Task<UserThread?> GetByAuthor(long authorID, bool isActive=true)
    {
      MySqlConnection connection = await GetConnection();
      MySqlCommand cmd = new MySqlCommand(
        $"SELECT {COLUMNS} FROM modmail.threads"
        + " WHERE author_id=@authorID"
        + " AND is_active=@isActive",
        connection);

      cmd.Parameters.AddWithValue("authorID", authorID);
      cmd.Parameters.AddWithValue("isActive", isActive);

      return await ReadOne(cmd);
    }

    /// <summary>
    /// <c>GetByChannelID</c>
    /// </summary>
    /// <param name="channelID"></param>
    /// <returns>A nullable Thread</returns>
    public async Task<UserThread?> GetByChannelID(long channelID)
    {
      MySqlConnection connection = await GetConnection();
      MySqlCommand cmd = new MySqlCommand(
        $"SELECT {COLUMNS} FROM modmail.threads"
        + " WHERE channel_id=@channelID",
        connection);

      cmd.Parameters.AddWithValue("channelID", channelID);

      return await ReadOne(cmd);
    }

    protected override UserThread Read(DbDataReader reader)
    {
      return new UserThread
      {
        AuthorID = reader.GetFieldValue<ulong>(OAuthorID),
        CategoryID = reader.GetFieldValue<ulong>(OCategoryID),
        ChannelID = reader.GetFieldValue<ulong>(OChannelID),
        ID = reader.GetFieldValue<ulong>(OID),
        IsActive = reader.GetBoolean(OIsActive),
        Messages = new List<Message>(),
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
    }
  }
}

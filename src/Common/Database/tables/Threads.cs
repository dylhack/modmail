using Npgsql;
using System.Threading.Tasks;
using System.Collections.Generic;
using Modmail.Models;

namespace Modmail.Database.Tables
{
  public class Threads : Table<Thread>
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
      id BIGINT NOT NULL
        CONSTRAINT threads_pk PRIMARY KEY,
      author_id BIGINT NOT NULL
        CONSTRAINT threads_users_id_fk
        REFERENCES modmail.users,
      channel_id BIGINT NOT NULL,
      is_active BOOLEAN DEFAULT true NOT NULL,
      category_id BIGINT NOT NULL
        CONSTRAINT threads_categories_id_fk
        REFERENCES modmail.categories);";

    const string INIT_CHANNEL_UINDEX = @"
    CREATE UNIQUE INDEX IF NOT EXISTS threads_channel_uindex
      ON modmail.threads (channel);";

    const string INIT_ID_UINDEX = @"
    CREATE UNIQUE INDEX IF NOT EXISTS threads_id_uindex
      ON modmail.threads (id);";

    public Threads(ref string connStr) : base("Threads", connStr)
    {}

    /// <summary>
    /// <c>Store</c> store a Thread
    /// </summary>
    /// <param name="thread"></param>
    /// <returns>A boolean representing whether or not it was successfully
    /// stored</returns>
    public async Task<bool> Store(Thread thread)
    {
      NpgsqlConnection connection = await GetConnection();
      NpgsqlCommand cmd = new NpgsqlCommand(
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
      NpgsqlConnection connection = await GetConnection();
      NpgsqlCommand cmd = new NpgsqlCommand(
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
    public async Task<Thread?> GetByID(long threadID)
    {
      NpgsqlConnection connection = await GetConnection();
      NpgsqlCommand cmd = new NpgsqlCommand(
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
    public async Task<Thread?> GetByAuthor(long authorID, bool isActive=true)
    {
      NpgsqlConnection connection = await GetConnection();
      NpgsqlCommand cmd = new NpgsqlCommand(
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
    public async Task<Thread?> GetByChannelID(long channelID)
    {
      NpgsqlConnection connection = await GetConnection();
      NpgsqlCommand cmd = new NpgsqlCommand(
        $"SELECT {COLUMNS} FROM modmail.threads"
        + " WHERE channel_id=@channelID",
        connection);

      cmd.Parameters.AddWithValue("channelID", channelID);

      return await ReadOne(cmd);
    }

    protected override Thread Read(NpgsqlDataReader reader)
    {
      return new Thread
      {
        AuthorID = reader.GetInt64(OAuthorID),
        CategoryID = reader.GetInt64(OCategoryID),
        ChannelID = reader.GetInt64(OChannelID),
        ID = reader.GetInt64(OID),
        IsActive = reader.GetBoolean(OIsActive),
        Messages = new List<Message>(),
      };
    }

    protected override async Task Prepare()
    {
      NpgsqlConnection connection = await GetConnection();

      await new NpgsqlCommand(INIT, connection)
        .ExecuteNonQueryAsync();

      await new NpgsqlCommand(
        INIT_CHANNEL_UINDEX,
        connection).ExecuteNonQueryAsync();

      await new NpgsqlCommand(
        INIT_ID_UINDEX,
        connection).ExecuteNonQueryAsync();
    }
  }
}

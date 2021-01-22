using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data.Common;
using MySql.Data.MySqlClient;
using Modmail.Models;

namespace Modmail.Database.Tables
{
  public class Categories : Table<Category>
  {
    private static string COLUMNS =>
    "id, name, is_active, guild_id, emoji";
    private static string INSERTION =>
    "@id, @name, True, @gid, @emoji";
    private static sbyte OID => 0;
    private static sbyte OChannelID => 1;
    private static sbyte OName => 2;
    private static sbyte OIsActive => 3;
    private static sbyte OGuildID => 4;
    private static sbyte OEmoji => 5;

    const string INIT = @"
    CREATE TABLE IF NOT EXISTS modmail.categories (
    id BIGINT UNSIGNED NOT NULL,
    CONSTRAINT pk_categories PRIMARY KEY (id),
    channel_id BIGINT UNSIGNED UNIQUE NOT NULL,
    name VARCHAR(32) NOT NULL,
    is_active BOOLEAN DEFAULT TRUE NOT NULL,
    guild_id BIGINT UNSIGNED NOT NULL,
    emoji VARCHAR(2) NOT NULL
    );";

    const string INIT_EMOTE_UINDEX = @"
    CREATE UNIQUE INDEX uindex_categories_emoji
      ON modmail.categories(emoji);";

    const string INIT_ID_UINDEX = @"
    CREATE UNIQUE INDEX uindex_categories_id
      ON modmail.categories(id);";

    const string INIT_NAME_UINDEX = @"
    CREATE UNIQUE INDEX uindex_categories_name
      ON modmail.categories(name);";

    public Categories(ref string connStr) : base("Categories", connStr)
    { }

    public async Task<Category?> GetByEmoji(string emoji)
    {
      MySqlConnection connection = await GetConnection();
      MySqlCommand cmd = new MySqlCommand(
        $"SELECT {COLUMNS} FROM modmail.categories"
        + " WHERE is_active=True AND emoji=@emoji",
        connection);
      cmd.Parameters.AddWithValue("emoji", emoji);

      return await ReadOne(cmd);
    }

    public async Task<Category?> GetByID(ulong id)
    {
      MySqlConnection connection = await GetConnection();
      MySqlCommand cmd = new MySqlCommand(
        $"SELECT {COLUMNS} FROM modmail.categories"
        + " WHERE is_active=True AND id=@id",
        connection);
      cmd.Parameters.AddWithValue("id", id);

      return await ReadOne(cmd);
    }

    /// <summary>
    /// <c>GetActive</c> fetchs all the current active categories
    /// </summary>
    /// <returns>A list of categories that are active</returns>
    public async Task<List<Category>> GetActive()
    {
      MySqlConnection connection = await GetConnection();
      MySqlCommand cmd = new MySqlCommand(
        $"SELECT {COLUMNS} FROM modmail.categories"
        + " WHERE is_active=True",
        connection);

      return await ReadAll(cmd);
    }


    public async Task<bool> SetActive(long id, bool isActive)
    {
      MySqlConnection connection = await GetConnection();
      MySqlCommand cmd = new MySqlCommand(
        "UPDATE modmail.categories SET is_active=@active"
        + " WHERE id=@id",
        connection);

      cmd.Parameters.AddWithValue("id", id);
      cmd.Parameters.AddWithValue("active", isActive);

      return await Execute(cmd);
    }

    /// <summary>
    /// <c>SetEmoji</c> sets the emoji of a Category
    /// </summary>
    /// <param name="id">Category to update</param>
    /// <param name="emoji">The new emoji</param>
    /// <returns>This method returns a boolean which represents whether or not
    /// the category was updated</returns>
    public async Task<bool> SetEmoji(long id, string emoji)
    {
      MySqlConnection connection = await GetConnection();
      MySqlCommand cmd = new MySqlCommand(
        "UPDATE modmail.categories SET emoji=@emoji"
        + " WHERE id=@id",
        connection);

      cmd.Parameters.AddWithValue("id", id);
      cmd.Parameters.AddWithValue("emoji", emoji);

      return await Execute(cmd);
    }

    /// <summary>
    /// <c>SetName</c> sets the name of a Category
    /// </summary>
    /// <param name="id">The category to update</param>
    /// <param name="name">The new name for the category</param>
    /// <returns>This returns a boolean which represents whether or not
    /// the category was updated</returns>
    public async Task<bool> SetName(long id, string name)
    {
      MySqlConnection connection = await GetConnection();
      MySqlCommand cmd = new MySqlCommand(
        "UPDATE modmail.categories SET name=@name"
        + " WHERE id=@id",
        connection);

      cmd.Parameters.AddWithValue("id", id);
      cmd.Parameters.AddWithValue("name", name);

      return await Execute(cmd);
    }

    /// <summary>
    /// <c>Store</c> stores a category into the categories table
    /// </summary>
    /// <param name="category">Category to store</param>
    /// <returns>A boolean representing whether or not it stored properly
    /// </returns>
    public async Task<bool> Store(Category category)
    {
      MySqlConnection connection = await GetConnection();
      MySqlCommand cmd = new MySqlCommand(
        $"INSERT INTO modmail.categories {COLUMNS}"
        + $" VALUES ({INSERTION})",
        connection);

      cmd.Parameters.AddWithValue("id", category.ID);
      cmd.Parameters.AddWithValue("chanid", category.ChannelID);
      cmd.Parameters.AddWithValue("name", category.Name);
      cmd.Parameters.AddWithValue("gid", category.GuildID);
      cmd.Parameters.AddWithValue("emoji", category.Emoji);

      return await Execute(cmd);
    }

    protected override async Task Prepare()
    {

      MySqlConnection connection = await GetConnection();

      await new MySqlCommand(
        INIT,
        connection).ExecuteNonQueryAsync();

      await new MySqlCommand(
        INIT_EMOTE_UINDEX,
        connection).ExecuteNonQueryAsync();

      await new MySqlCommand(
        INIT_ID_UINDEX,
        connection).ExecuteNonQueryAsync();

      await new MySqlCommand(
        INIT_NAME_UINDEX,
        connection).ExecuteNonQueryAsync();
  }

  protected override Category Read(DbDataReader reader)
  {
    return new Category
    {
      ID = reader.GetFieldValue<ulong>(OID),
      ChannelID = reader.GetFieldValue<ulong>(OChannelID),
      Emoji = reader.GetString(OEmoji),
      GuildID = reader.GetFieldValue<ulong>(OGuildID),
      Name = reader.GetString(OName),
      IsActive = reader.GetBoolean(OIsActive),
    };
  }

  protected override async Task<Category?> ReadOne(MySqlCommand cmd)
  {
    DbDataReader reader = await cmd.ExecuteReaderAsync();

    if (await reader.ReadAsync())
    {
      return Read(reader);
    }
    return null;
  }

  protected override async Task<bool> Execute(MySqlCommand cmd)
  {
    int rows = await cmd.ExecuteNonQueryAsync();
    return rows > 0;
  }
}
}

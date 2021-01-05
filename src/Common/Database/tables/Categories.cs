using Npgsql;
using System.Threading.Tasks;
using System.Collections.Generic;
using Modmail.Models;

namespace Modmail.Database.Tables
{
  public class Categories : Table<Category>
  {
    private static string COLUMNS =>
    "id, channel_id, name, is_active, guild_id, emoji";
    private static string INSERTION =>
    "@id, @chanid, @name, True, @gid, @emoji";
    private static sbyte OID => 0;
    private static sbyte OChannelID => 1;
    private static sbyte OName => 2;
    private static sbyte OIsActive => 3;
    private static sbyte OGuildID => 4;
    private static sbyte OEmoji => 5;

    const string INIT = @"
    CREATE TABLE IF NOT EXISTS modmail.categories (
      id BIGINT NOT NULL
        CONSTRAINT categories_pk PRIMARY KEY,
      channel_id BIGINT UNIQUE NOT NULL,
      name TEXT NOT NULL,
      is_active BOOLEAN DEFAULT true NOT NULL,
      guild_id BIGINT NOT NULL,
      emoji TEXT NOT NULL);";

    const string INIT_EMOTE_UINDEX = @"
    CREATE UNIQUE INDEX IF NOT EXISTS categories_emote_uindex
      ON modmail.categories (emoji);";

    const string INIT_ID_UINDEX = @"
    CREATE UNIQUE INDEX IF NOT EXISTS categories_id_uindex
      ON modmail.categories (id);";

    const string INIT_NAME_UINDEX = @"
    CREATE UNIQUE INDEX IF NOT EXISTS categories_name_uindex
      ON modmail.categories (name);";

    public Categories(ref string connStr) : base("Categories", connStr)
    { }

    public async Task<Category?> GetByEmoji(string emoji)
    {
      NpgsqlConnection connection = await GetConnection();
      NpgsqlCommand cmd = new NpgsqlCommand(
        $"SELECT {COLUMNS} FROM modmail.categories"
        + " WHERE is_active=True AND emoji=@emoji",
        connection);
      cmd.Parameters.AddWithValue("emoji", emoji);

      return await ReadOne(cmd);
    }

    public async Task<Category?> GetByID(long id)
    {
      NpgsqlConnection connection = await GetConnection();
      NpgsqlCommand cmd = new NpgsqlCommand(
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
      NpgsqlConnection connection = await GetConnection();
      NpgsqlCommand cmd = new NpgsqlCommand(
        $"SELECT {COLUMNS} FROM modmail.categories"
        + " WHERE is_active=True",
        connection);

      return await ReadAll(cmd);
    }


    public async Task<bool> SetActive(long id, bool isActive)
    {
      NpgsqlConnection connection = await GetConnection();
      NpgsqlCommand cmd = new NpgsqlCommand(
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
      NpgsqlConnection connection = await GetConnection();
      NpgsqlCommand cmd = new NpgsqlCommand(
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
      NpgsqlConnection connection = await GetConnection();
      NpgsqlCommand cmd = new NpgsqlCommand(
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
      NpgsqlConnection connection = await GetConnection();
      NpgsqlCommand cmd = new NpgsqlCommand(
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
      NpgsqlConnection connection = await GetConnection();

      await new NpgsqlCommand(
        INIT,
        connection).ExecuteNonQueryAsync();

      await new NpgsqlCommand(
        INIT_EMOTE_UINDEX,
        connection).ExecuteNonQueryAsync();

      await new NpgsqlCommand(
        INIT_ID_UINDEX,
        connection).ExecuteNonQueryAsync();

      await new NpgsqlCommand(
        INIT_NAME_UINDEX,
        connection).ExecuteNonQueryAsync();
    }

    protected override Category Read(NpgsqlDataReader reader)
    {
      return new Category
      {
        ID = reader.GetInt64(OID),
        ChannelID = reader.GetInt64(OChannelID),
        Emoji = reader.GetString(OEmoji),
        GuildID = reader.GetInt64(OGuildID),
        Name = reader.GetString(OName),
        IsActive = reader.GetBoolean(OIsActive),
      };
    }

    protected override async Task<Category?> ReadOne(NpgsqlCommand cmd)
    {
      NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

      if (await reader.ReadAsync())
      {
        return Read(reader);
      }
      return null;
    }

    protected override async Task<List<Category>> ReadAll(NpgsqlCommand cmd)
    {
      NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
      List<Category> res = new List<Category>();

      while (await reader.ReadAsync())
      {
        Category cat = Read(reader);
        res.Add(cat);
      }

      return res;
    }

    protected override async Task<bool> Execute(NpgsqlCommand cmd)
    {
      int rows = await cmd.ExecuteNonQueryAsync();
      return rows > 0;
    }
  }
}

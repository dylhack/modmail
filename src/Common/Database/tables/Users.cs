using Npgsql;
using System.Threading.Tasks;
using Modmail.Models;

namespace Modmail.Database.Tables
{
  public class Users : Table<ModmailUser>
  {
    static string COLUMNS => "id";
    static string INSERTION => "@id";
    static sbyte OID => 0;
    const string INIT = @"
    CREATE TABLE IF NOT EXISTS modmail.users (
      id BIGINT NOT NULL
        CONSTRAINT users_pk PRIMARY KEY);";

    const string INIT_ID_UINDEX = @"
    CREATE UNIQUE INDEX IF NOT EXISTS users_id_uindex
      ON modmail.users (id);";

    public Users(ref string connStr) : base("Users", connStr)
    {}

    /// <summary>
    /// <c>Store</c> store a new user
    /// </summary>
    /// <param name="user"></param>
    /// <returns>A boolean representing whether or not the user was stored
    /// successfully</returns>
    public async Task<bool> Store(ModmailUser user)
    {
      NpgsqlConnection connection = new NpgsqlConnection();
      NpgsqlCommand cmd = new NpgsqlCommand(
        $"INSERT INTO modmail.users ({COLUMNS}) VALUES ({INSERTION})",
        connection);

      cmd.Parameters.AddWithValue("id", user.ID);

      return await Execute(cmd);
    }

    /// <summary>
    /// <c>Remove</c> remove a user
    /// </summary>
    /// <param name="userID"></param>
    /// <returns>A boolean representing whether or not the user was removed
    /// </returns>
    public async Task<bool> Remove(long userID)
    {
      NpgsqlConnection connection = new NpgsqlConnection();
      NpgsqlCommand cmd = new NpgsqlCommand(
        "DELETE FROM modmail.users WHERE id=@id",
        connection);

      cmd.Parameters.AddWithValue("id", userID);

      return await Execute(cmd);
    }

    protected override ModmailUser Read(NpgsqlDataReader reader)
    {
      return new ModmailUser
      {
        ID = reader.GetInt64(0),
      };
    }

    protected override async Task Prepare()
    {
      NpgsqlConnection connection = await GetConnection();

      await new NpgsqlCommand(INIT, connection)
        .ExecuteNonQueryAsync();

      await new NpgsqlCommand(INIT_ID_UINDEX, connection)
        .ExecuteNonQueryAsync();
    }
  }
}

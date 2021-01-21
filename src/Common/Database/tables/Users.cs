using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Collections.Generic;
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
	    id BIGINT UNSIGNED NOT NULL,
	       CONSTRAINT pk_user PRIMARY KEY (id));";

    const string INIT_ID_UINDEX = @"
    CREATE UNIQUE INDEX uindex_users
      ON modmail.users(id);";

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
      MySqlConnection connection = await this.GetConnection();
      MySqlCommand cmd = new MySqlCommand(
        $"INSERT INTO modmail.users ({COLUMNS}) VALUES ({INSERTION})",
        connection);

      cmd.Parameters.AddWithValue("id", user.ID);

      return await Execute(cmd);
    }

    /// <summary>
    /// <c>GetAll</c> get all users in the database
    /// </summary>
    /// <returns>A list of Modmail Users</returns>
    public async Task<List<ModmailUser>> GetAll()
    {
      MySqlConnection connection = await GetConnection();
      MySqlCommand cmd = new MySqlCommand(
        $"SELECT * FROM modmail.users;",
        connection);

      return await ReadAll(cmd);
    }

    /// <summary>
    /// <c>Remove</c> remove a user
    /// </summary>
    /// <param name="userID"></param>
    /// <returns>A boolean representing whether or not the user was removed
    /// </returns>
    public async Task<bool> Remove(long userID)
    {
      MySqlConnection connection = await GetConnection();
      MySqlCommand cmd = new MySqlCommand(
        "DELETE FROM modmail.users WHERE id=@id",
        connection);

      cmd.Parameters.AddWithValue("id", userID);

      return await Execute(cmd);
    }

    protected override ModmailUser Read(DbDataReader reader)
    {
      return new ModmailUser
      {
        ID = reader.GetFieldValue<ulong>(0),
      };
    }

    protected override async Task Prepare()
    {
      MySqlConnection connection = await GetConnection();

      await new MySqlCommand(INIT, connection)
        .ExecuteNonQueryAsync();

      await new MySqlCommand(INIT_ID_UINDEX, connection)
        .ExecuteNonQueryAsync();
    }
  }
}

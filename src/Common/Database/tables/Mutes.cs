using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Common;
using Modmail.Models;
using Modmail.Util;

namespace Modmail.Database.Tables
{
  public class Mutes : Table<MuteStatus>
  {
    private static string COLUMNS =>
    "user_id, till, category_id, reason, is_ignored";
    private static string INSERTION =>
    "@userID, @till, @categoryID, @reason, @isIgnored";
    private static sbyte OUserID => 0;
    private static sbyte OTill => 1;
    private static sbyte OCategoryID => 2;
    private static sbyte OReason => 3;
    private static sbyte OIsIgnored => 4;
    const string INIT = @"
    CREATE TABLE IF NOT EXISTS modmail.mutes (
    user_id BIGINT UNSIGNED NOT NULL,
    FOREIGN KEY (user_id)
        REFERENCES modmail.users (id),
    till BIGINT UNSIGNED NOT NULL,
    category_id BIGINT UNSIGNED NOT NULL,
    is_ignored BOOLEAN DEFAULT FALSE NOT NULL,
    reason TEXT NOT NULL
    );";

    public Mutes(ref string connStr) : base("Mutes", connStr)
    {}

    /// <summary>
    /// <c>GetMuted</c> this method attempts to get an active mute for a
    /// user of a category.
    /// </summary>
    /// <param name="userID"></param>
    /// <param name="categoryID"></param>
    /// <returns></returns>
    public async Task<MuteStatus?> GetMuted(long userID, long categoryID)
    {
      long now = Time.GetNow();
      MySqlConnection connection = new MySqlConnection();
      MySqlCommand cmd = new MySqlCommand(
        $"SELECT {COLUMNS} FROM modmail.mutes"
        + " WHERE till>@now"
        + " AND category_id=@categoryID"
        + " AND user_id=@userID"
        + " AND NOT is_ignored",
        connection);

      cmd.Parameters.AddWithValue("now", now);
      cmd.Parameters.AddWithValue("userID", userID);
      cmd.Parameters.AddWithValue("categoryID", categoryID);

      return await ReadOne(cmd);
    }

    /// <summary>
    /// <c>GetAllMuted</c> gets all the mutes of a user whether active or not.
    /// </summary>
    /// <param name="userID"></param>
    /// <param name="categoryID">Optional</param>
    /// <returns>A list of Mute Statuses</returns>
    public async Task<List<MuteStatus>> GetAllMuted(long userID, long categoryID=0)
    {
      MySqlConnection connection = new MySqlConnection();
      MySqlCommand cmd = new MySqlCommand(
        $"SELECT {COLUMNS} FROM modmail.mutes"
        + " WHERE user_id=@userID"
        + (categoryID == 0 ? "" : " AND category_id=@categoryID"),
        connection);

      cmd.Parameters.AddWithValue("userID", userID);

      if (categoryID != 0)
      {
        cmd.Parameters.AddWithValue("categoryID", categoryID);
      }

      return await ReadAll(cmd);
    }

    /// <summary>
    /// <c>Store</c> store a new mute status
    /// </summary>
    /// <param name="mute"></param>
    /// <returns>A boolean whether or not it was added to the datbase
    /// </returns>
    public async Task<bool> Store(MuteStatus mute)
    {
      MySqlConnection connection = new MySqlConnection();
      MySqlCommand cmd = new MySqlCommand(
        $"INSERT INTO modmail.mutes ({COLUMNS}) VALUES ({INSERTION})",
        connection);

      cmd.Parameters.AddWithValue("userID", mute.UserID);
      cmd.Parameters.AddWithValue("till", mute.Till);
      cmd.Parameters.AddWithValue("categoryID", mute.CategoryID);
      cmd.Parameters.AddWithValue("reason", mute.Reason);
      cmd.Parameters.AddWithValue("isIgnored", mute.IsIgnored);

      return await Execute(cmd);
    }

    /// <summary>
    /// <c>Remove</c> unmutes a user (marking the is_ignored as true)
    /// </summary>
    /// <param name="userID"></param>
    /// <param name="categoryID"></param>
    /// <returns>A boolean representing whether or not anything was updated
    /// </returns>
    public async Task<bool> Remove(long userID, long categoryID)
    {
      long now = Time.GetNow();
      MySqlConnection connection = new MySqlConnection();
      MySqlCommand cmd = new MySqlCommand(
        "UPDATE modmail.mutes SET is_ignored=True"
        + " WHERE till>@now"
        + " AND category_id=@categoryID"
        + " AND user_id=@userID"
        + " AND NOT is_ignored",
        connection);

      cmd.Parameters.AddWithValue("now", now);
      cmd.Parameters.AddWithValue("userID", userID);
      cmd.Parameters.AddWithValue("categoryID", categoryID);

      return await Execute(cmd);
    }

    protected override MuteStatus Read(DbDataReader reader)
    {
      return new MuteStatus
      {
        CategoryID = reader.GetFieldValue<ulong>(OCategoryID),
        Reason = reader.GetString(OReason),
        Till = reader.GetFieldValue<ulong>(OTill),
        UserID = reader.GetFieldValue<ulong>(OUserID),
        IsIgnored = reader.GetBoolean(OIsIgnored),
      };
    }

    protected override async Task Prepare()
    {
      MySqlConnection connection = await GetConnection();
      MySqlCommand cmd = new MySqlCommand(INIT, connection);

      await cmd.ExecuteNonQueryAsync();
    }
  }
}

using Npgsql;
using System.Threading.Tasks;
using Modmail.Models;

namespace Modmail.Database.Tables
{
  public class Permissions : Table<Role>
  {
    private static string COLUMNS =>
    "category_id, id, level";
    private static string INSERTION =>
    "@categoryID, @id, @level";
    private static sbyte OCategoryID => 0;
    private static sbyte OID => 1;
    private static sbyte OLevel => 2;
    const string INIT = @"
    CREATE TABLE IF NOT EXISTS modmail.permissions (
      category_id BIGINT NOT NULL
        REFERENCES modmail.categories,
      id TEXT UNIQUE NOT NULL,
      level modmail.role_level DEFAULT 'mod'::modmail.role_level NOT NULL);";

    public Permissions(ref string connStr) : base("Permissions", connStr)
    {}

    /// <summary>
    /// <c>Store</c> Store a new role
    /// </summary>
    /// <param name="role"></param>
    /// <returns>A boolean which represents whether or not it was successfully
    /// stored</returns>
    public async Task<bool> Store(Role role)
    {
      string roleLevel = Resolve(role.Level);
      NpgsqlConnection connection = await GetConnection();
      NpgsqlCommand cmd = new NpgsqlCommand(
        $"INSERT INTO modmail.permissions ({COLUMNS}) VALUES ({INSERTION})",
        connection);

      cmd.Parameters.AddWithValue("categoryID", role.CategoryID);
      cmd.Parameters.AddWithValue("id", role.ID);
      cmd.Parameters.AddWithValue("level", roleLevel);

      return await Execute(cmd);
    }

    /// <summary>
    /// <c>Remove</c> remoe a role from the database
    /// </summary>
    /// <param name="roleID"></param>
    /// <returns>A boolean representing if the role was successfully removed
    /// </returns>
    public async Task<bool> Remove(long roleID)
    {
      NpgsqlConnection connection = await GetConnection();
      NpgsqlCommand cmd = new NpgsqlCommand(
        "DELETE FROM modmail.permissions WHERE id=@id",
        connection);

      cmd.Parameters.AddWithValue("id", roleID);

      return await Execute(cmd);
    }

    /// <summary>
    /// <c>SetLevel</c> Set level of a role
    /// </summary>
    /// <param name="roleID"></param>
    /// <param name="roleLevel"></param>
    /// <returns>A boolean representing that it updated properly</returns>
    public async Task<bool> SetLevel(long roleID, string roleLevel)
    {
      NpgsqlConnection connection = await GetConnection();
      NpgsqlCommand cmd = new NpgsqlCommand(
        $"UPDATE modmail.permissions SET role_level=@roleLevel"
        + " WHERE id=@id",
        connection);

      cmd.Parameters.AddWithValue("roleID", roleID);
      cmd.Parameters.AddWithValue("roleLevel", roleLevel);

      return await Execute(cmd);
    }

    /// <summary>
    /// <c>SetLevel</c> This method calls the other SetLevel
    /// </summary>
    /// <seealso cref="SetLevel"/>
    /// <param name="roleID"></param>
    /// <param name="roleLevel"></param>
    /// <returns></returns>
    public async Task<bool> SetLevel(long roleID, RoleLevel roleLevel)
    {
      return await SetLevel(
        roleID,
        Resolve(roleLevel));
    }

    protected string Resolve(RoleLevel roleLevel)
    {
      switch (roleLevel)
      {
        case RoleLevel.Admin:
          return "admin";
        default:
          return "mod";
      }
    }

    protected RoleLevel Resolve(string roleLevel)
    {
      switch (roleLevel.ToLower())
      {
        case "admin":
          return RoleLevel.Admin;
        default:
          return RoleLevel.Mod;
      }
    }

    protected override Role Read(NpgsqlDataReader reader)
    {
      return new Role
      {
        CategoryID = reader.GetInt64(OCategoryID),
        Level = Resolve(reader.GetString(OLevel)),
        ID = reader.GetInt64(OID),
      };
    }

    protected override async Task Prepare()
    {
      NpgsqlConnection connection = await GetConnection();
      NpgsqlCommand cmd = new NpgsqlCommand(INIT, connection);

      await cmd.ExecuteNonQueryAsync();
    }
  }
}

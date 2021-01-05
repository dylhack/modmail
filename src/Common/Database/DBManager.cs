using Npgsql;
using Modmail.Configuration;
using Modmail.Database.Tables;
using System;
using System.Threading.Tasks;

namespace Modmail.Database
{
  public class DBManager
  {
    const string INIT_SCHEMA = "CREATE SCHEMA IF NOT EXISTS modmail;";

    const string INIT_FILE_TYPE =
    "CREATE TYPE modmail.file_type AS ENUM ('image', 'file');";

    const string INIT_ROLE_LEVEL =
    "CREATE TYPE modmail.role_level AS ENUM ('admin', 'mod');";

    private string connStr;
    private Attachments attachments { get; }
    private Categories categories { get; }
    private Edits edits { get; }
    private Messages messages { get; }
    private Mutes mutes { get; }
    private Permissions permissions { get; }
    private StandardReplies standardReplies { get; }
    private Threads threads { get; }
    private Users users { get; }

    internal DBManager(ref string connStr)
    {
      this.connStr = connStr;
      this.attachments = new Attachments(ref connStr);
      this.categories = new Categories(ref connStr);
      this.edits = new Edits(ref connStr);
      this.messages = new Messages(ref connStr);
      this.mutes = new Mutes(ref connStr);
      this.permissions = new Permissions(ref connStr);
      this.standardReplies = new StandardReplies(ref connStr);
      this.threads = new Threads(ref connStr);
      this.users = new Users(ref connStr);
    }

    public static async Task<DBManager> GetDatabase(DBConfig config)
    {
      string connStr = GetConnString(ref config);
      DBManager db = new DBManager(ref connStr);
      Task[] tasks = new Task[5];

      await db.Init();
      await db.users.Init();
      await db.categories.Init();
      await db.threads.Init();
      await db.messages.Init();

      tasks[0] = db.attachments.Init();
      tasks[1] = db.edits.Init();
      tasks[2] = db.mutes.Init();
      tasks[3] = db.permissions.Init();
      tasks[4] = db.standardReplies.Init();

      Task.WaitAll(tasks);

      return db;
    }

    private async Task Init()
    {
      NpgsqlConnection connection = new NpgsqlConnection(this.connStr);
      await connection.OpenAsync();

      await new NpgsqlCommand(
        INIT_SCHEMA,
        connection).ExecuteNonQueryAsync();

      try
      {
        await new NpgsqlCommand(
          INIT_FILE_TYPE,
          connection).ExecuteNonQueryAsync();

        await new NpgsqlCommand(
          INIT_ROLE_LEVEL,
          connection).ExecuteNonQueryAsync();
      } catch (PostgresException)
      {
        return;
      }

    }

    private static string GetConnString(ref DBConfig config)
    {
      var builder = new NpgsqlConnectionStringBuilder();

      builder.Add("Host", config.address);
      builder.Add("Port", config.port);
      builder.Add("Username", config.username);
      builder.Add("Password", config.password);
      builder.Add("Database", config.database);

      return builder.ToString();
    }
  }
}

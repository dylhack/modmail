using MySql.Data;
using MySql.Data.MySqlClient;
using Modmail.Configuration;
using Modmail.Database.Tables;
using System.Threading.Tasks;

namespace Modmail.Database
{
  public class DBManager
  {
    const string INIT_SCHEMA = "CREATE SCHEMA IF NOT EXISTS modmail;";

    public string connStr;
    public Attachments attachments;
    public Categories categories;
    public Edits edits;
    public Messages messages;
    public Mutes mutes;
    public Permissions permissions;
    public StandardReplies standardReplies;
    public UserThreads threads;
    public Users users;

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
      this.threads = new UserThreads(ref connStr);
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
      MySqlConnection connection = new MySqlConnection(this.connStr);
      await connection.OpenAsync();

      await new MySqlCommand(
        INIT_SCHEMA,
        connection).ExecuteNonQueryAsync();
    }

    private static string GetConnString(ref DBConfig config)
    {
      var builder = new MySqlConnectionStringBuilder();

      builder.Add("Host", config.address);
      builder.Add("Port", config.port);
      builder.Add("Username", config.username);
      builder.Add("Password", config.password);
      builder.Add("Database", config.database);

      return builder.ToString();
    }
  }
}

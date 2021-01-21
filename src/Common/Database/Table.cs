using log4net;
using MySql.Data.MySqlClient;
using System;
using System.Data.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Modmail.Database
{
  public class Table<T> where T : struct
  {
    protected string connStr;
    protected string name;
    public Table(string name, string connStr)
    {
      this.connStr = connStr;
      this.name = name;
    }

    public async Task Init()
    {
      ILog logger = GetLogger();
      logger.Info("Initializing");
      try {
        await Prepare();
      } catch (MySqlException e) {
        if (!e.Message.Contains("Duplicate key name")) {
          logger.Error(e);
        }
      }
    }

    protected virtual T Read(DbDataReader reader)
    {
      throw new NotImplementedException();
    }

    protected virtual async Task<T?> ReadOne(MySqlCommand cmd)
    {
      DbDataReader reader = await cmd.ExecuteReaderAsync();

      if (await reader.ReadAsync())
      {
        return Read(reader);
      }

      return null;
    }

    protected virtual async Task<List<T>> ReadAll(MySqlCommand cmd)
    {
      DbDataReader reader = await cmd.ExecuteReaderAsync();
      List<T> res = new List<T>();

      while (await reader.ReadAsync())
      {
        T cat = Read(reader);
        res.Add(cat);
      }

      return res;
    }

    protected virtual async Task<bool> Execute(MySqlCommand cmd)
    {
      int rows = await cmd.ExecuteNonQueryAsync();

      return rows > 0;
    }

    protected async Task<MySqlConnection> GetConnection()
    {
      MySqlConnection connection = new MySqlConnection(connStr);
      await connection.OpenAsync();
      return connection;
    }

    protected ILog GetLogger()
    {
      return LogManager.GetLogger($"(table) {this.name}");
    }

    protected virtual Task Prepare()
    {
      throw new NotImplementedException();
    }
  }
}

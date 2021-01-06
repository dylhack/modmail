using log4net;
using Npgsql;
using System;
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
      await Prepare();
    }

    protected virtual T Read(NpgsqlDataReader reader)
    {
      throw new NotImplementedException();
    }

    protected virtual async Task<T?> ReadOne(NpgsqlCommand cmd)
    {
      NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

      if (await reader.ReadAsync())
      {
        return Read(reader);
      }

      return null;
    }

    protected virtual async Task<List<T>> ReadAll(NpgsqlCommand cmd)
    {
      NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
      List<T> res = new List<T>();

      while (await reader.ReadAsync())
      {
        T cat = Read(reader);
        res.Add(cat);
      }

      return res;
    }

    protected virtual Task<bool> Execute(NpgsqlCommand cmd)
    {
      throw new NotImplementedException();
    }

    protected async Task<NpgsqlConnection> GetConnection()
    {
      NpgsqlConnection connection = new NpgsqlConnection(connStr);
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

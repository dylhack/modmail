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

    protected virtual Task<T?> ReadOne(NpgsqlCommand cmd)
    {
      throw new NotImplementedException();
    }

    protected virtual Task<List<T>> ReadAll(NpgsqlCommand cmd)
    {
      throw new NotImplementedException();
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

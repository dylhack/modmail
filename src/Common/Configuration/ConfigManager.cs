using System;
using System.IO;
using System.Text;
using log4net;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization.NodeDeserializers;

namespace Modmail.Configuration
{
  public class ConfigManager
  {
    #nullable disable
    const string LOCATION = "./config.yml";
    const string ENV_VAR = "MODMAIL_CONFIG";

    const UInt32 CONF_SIZE = 2000;

    const string LOG_NAME = "ConfigManager";

    public static Config GetConfig()
    {
      Config res = null;
      ILog log = LogManager.GetLogger(LOG_NAME);
      IDeserializer des = new DeserializerBuilder()
        .WithNamingConvention(UnderscoredNamingConvention.Instance)
        .WithNodeDeserializer(
          inner => new Validator(inner),
          s => s.InsteadOf<ObjectNodeDeserializer>()
        )
        .Build();

      try
      {
        string contents = GetContents();
        res = des.Deserialize<Config>(contents);
      } catch (FileNotFoundException) {
        log.Error(
          $"Failed to config.yml from {GetPath()} does it exist?"
        );
      } catch (YamlException ye)
      {
        log.Error(
          $"Failed to deserialize the config\n{ye.InnerException.Message}"
        );
      } catch (Exception e) {
        log.Error(
          $"An unexpected error occurred while getting the config.yml\n{e}"
        );
      }
      return res;
    }

    private static string GetContents()
    {
      string res = "";
      string path = GetPath();
      using (FileStream conf = File.OpenRead(path))
      {
        byte[] data = new byte[CONF_SIZE];
        UTF8Encoding encoder = new UTF8Encoding(true);
        while (conf.Read(data, 0, data.Length) > 0)
        {
          res += encoder.GetString(data);
        }
      }

      return res;
    }

    private static string GetPath()
    {
      string path = Environment.GetEnvironmentVariable(ENV_VAR);

      if (path != null) {
        return Path.GetFullPath(path);
      }

      return Path.GetFullPath(LOCATION);
    }
  }
}
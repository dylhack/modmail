using System;
using System.IO;
using System.Threading.Tasks;
using log4net;
using Modmail.Configuration;
using Modmail.Database;

namespace Modmail
{
    class Program
    {
        public static void Main(string[] args)
        {
            StartLogger();
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            ILog log = LogManager.GetLogger("Main");
            Config config = ConfigManager.GetConfig();
            DBManager db = await DBManager.GetDatabase(config.database);

            if (config == null)
            {
                return;
            }
        }


        private static void StartLogger()
        {
            log4net.Config.XmlConfigurator.Configure(
                new FileInfo("./log4net.config")
            );
        }
    }
}

using System;
using System.IO;
using log4net;
using Modmail.Configuration;

namespace Modmail
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure(
                new FileInfo("./log4net.config")
            );
            ILog log = LogManager.GetLogger("Main");
            log.Info("Starting...");

            Config config = ConfigManager.GetConfig();

            if (config == null)
            {
                return;
            }

            Console.WriteLine($"Config acquired.\n{config}");
        }
    }
}

using MexcTriangularArbitrage.Services;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace MexcTriangularArbitrage
{
    class Program
    {
        static void Main(string[] args)
        {
            var rootCommand = new RootCommand
            {
                Description = "Mexc triangular arbitrage executor / simurator"
            };
            var tradeCommand = new Command("trade")
            {
                Handler = CommandHandler.Create(TradeCommandHandler)
            };

            var simurateCommand = new Command("simurate")
            {
                Handler = CommandHandler.Create(SimurateCommandHandler)
            };

            var configCommand = new Command("config")
            {
                Handler = CommandHandler.Create<string>(ConfigCommandHandler)
            };
            configCommand.AddOption(new Option<string>("mode", () => "check", "check/recreate"));

            rootCommand.AddCommand(tradeCommand);
            rootCommand.AddCommand(simurateCommand);
            rootCommand.AddCommand(configCommand);

            rootCommand.Invoke(args);
        }

        static void TradeCommandHandler()
        {
            SetTokenConfigToGlobalSetting();

            var readTrader = new RealTrader();
            readTrader.Execute();
        }

        static void SimurateCommandHandler()
        {
            SetTokenConfigToGlobalSetting();

            var simurator = new Simurator();
            simurator.Execute();
        }

        static void ConfigCommandHandler(string mode)
        {
            if (mode.ToLowerInvariant() == "recreate")
            {
                ConfigService.CreateTokenConfig();
                return;
            }
            var tokenConfig = ConfigService.ReadTokenConfig();
            if (tokenConfig == null)
            {
                Console.WriteLine("No config file exists.");
                return;
            }
            Console.WriteLine(tokenConfig);
        }

        static void SetTokenConfigToGlobalSetting()
        {
            ConfigService.CreateTokenConfigIfNotExists();
            var tokenConfig = ConfigService.ReadTokenConfig();
            GlobalSetting.TokenConfig = tokenConfig;
        }
    }
}

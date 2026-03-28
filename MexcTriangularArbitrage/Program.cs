using MexcTriangularArbitrage.Services;
using System.CommandLine;

namespace MexcTriangularArbitrage
{
    class Program
    {
        static void Main(string[] args)
        {
            var rootCommand = new RootCommand("Mexc triangular arbitrage executor / simulator");

            var tradeCommand = new Command("trade");
            tradeCommand.SetHandler(TradeCommandHandler);

            var simurateCommand = new Command("simulate");
            simurateCommand.SetHandler(SimurateCommandHandler);

            var modeOption = new Option<string>("--mode", () => "check", "check/recreate");
            var configCommand = new Command("config");
            configCommand.AddOption(modeOption);
            configCommand.SetHandler((string mode) => ConfigCommandHandler(mode), modeOption);

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

            var simulator = new Simulator();
            simulator.Execute();
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

using MexcTriangularArbitrage.Services;
using System;
using System.CommandLine;
using System.Threading;

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

            using var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                Console.WriteLine("Stopping...");
                cts.Cancel();
            };

            var readTrader = new RealTrader();
            readTrader.Execute(cts.Token);
        }

        static void SimurateCommandHandler()
        {
            SetTokenConfigToGlobalSetting();

            using var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                Console.WriteLine("Stopping...");
                cts.Cancel();
            };

            var simulator = new Simulator();
            simulator.Execute(cts.Token);
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
            ConfigService.LoadTradeConfigToGlobalSetting();
        }
    }
}

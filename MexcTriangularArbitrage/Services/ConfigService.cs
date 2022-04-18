using MexcTriangularArbitrage.Configs;
using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace MexcTriangularArbitrage.Services
{
    public static class ConfigService
    {
        public static void CreateTokenConfig()
        {
            Console.WriteLine("Create a new token config.");
            var config = new TokenConfig();
            Console.WriteLine("Enter your access key:");
            config.AccessKey = Console.ReadLine().Trim();
            Console.WriteLine("Enter your secret key:");
            config.SecretKey = Console.ReadLine().Trim();

            var json = JsonSerializer.Serialize(config);
            Directory.CreateDirectory(GlobalSetting.ConfigDirectory);
            using var streamWriter = new StreamWriter(GlobalSetting.TokenConfigPath, false, Encoding.UTF8);
            streamWriter.Write(json);
            Console.WriteLine("Succeeded to create the new token config.");
        }

        public static TokenConfig ReadTokenConfig()
        {
            if (!File.Exists(GlobalSetting.TokenConfigPath))
            {
                return null;
            }
            using var streamReader = new StreamReader(GlobalSetting.TokenConfigPath);
            var json = streamReader.ReadToEnd();
            var config = JsonSerializer.Deserialize<TokenConfig>(json);
            return config;
        }
    }
}

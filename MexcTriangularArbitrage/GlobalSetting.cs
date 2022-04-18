using MexcTriangularArbitrage.Configs;
using System.IO;
using System.Reflection;

namespace MexcTriangularArbitrage
{
    internal class GlobalSetting
    {
        internal static string ExePath { get; } = Assembly.GetEntryAssembly().Location;
        internal static string ExeDirectory { get;} = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        internal static string ConfigDirectory { get; } = Path.Combine(ExeDirectory, "config");
        internal static string TokenConfigPath { get; } = Path.Combine(ConfigDirectory, "token_config.json");
        internal static TokenConfig TokenConfig { get; set; } 
    }
}

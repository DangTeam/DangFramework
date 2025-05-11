using System.Collections.Generic;
using Akequ.Plugins;

namespace Dang.API.Interfaces
{
    public interface IPluginLoader
    {
        void LoadPlugin(string pluginPath);
        void UnloadPlugin(string pluginId);
        bool IsPluginLoaded(string pluginId);
        IEnumerable<PluginInfo> GetLoadedPlugins();
        void LoadConfig(string pluginId, string configPath);
        Dictionary<string, string> GetDependencies(string pluginId);
    }
}
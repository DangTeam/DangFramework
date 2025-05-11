using System.Collections.Generic;

namespace Dang.API.Interfaces
{
    public interface IPluginLoader
    {
        void LoadPlugin(string pluginPath);
        void UnloadPlugin(string pluginId);
        bool IsPluginLoaded(string pluginId);
        IEnumerable<IPluginData> GetLoadedPlugins();
        void LoadConfig(string pluginId, string configPath, IConfig config);
        Dictionary<string, string> GetDependencies(string pluginId);
    }
}
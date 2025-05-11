using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Akequ.Plugins;
using Dang.API.Attribute;
using Dang.API.Features;
using Dang.API.Interfaces;
using kcp2k;
using Log = Dang.API.Features.Log;

namespace Dang.API.Managers
{
    public class Manager : IPluginLoader
    {
        private readonly Dictionary<string, IPluginData> _loadedPlugins = new(); 
        private readonly Dictionary<string, Dictionary<string, string>> _pluginDependencies = new(); 
        private readonly Dictionary<string, PluginConfig> _pluginConfigs = new(); 
        private readonly string _pluginsDirectory; private readonly string _configsDirectory;

        public Manager(string pluginsDirectory, string configsDirectory)
        {
            _pluginsDirectory = pluginsDirectory;
            _configsDirectory = configsDirectory;
            Directory.CreateDirectory(_pluginsDirectory);
            Directory.CreateDirectory(_configsDirectory);
        }

        public void LoadAllPlugins()
        {
            try
            {
                Log.Info("Loading plugins...");

                foreach (var dllPath in Directory.GetFiles(_pluginsDirectory, "*.dll"))
                {
                    try
                    {
                        var assembly = Assembly.Load(File.ReadAllBytes(dllPath));
                        LoadPluginsFromAssembly(assembly);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Failed to load {Path.GetFileName(dllPath)}: {ex.Message}");
                    }
                }
                Log.Info("All plugins loaded.");
            }
            catch (Exception ex)
            {
                Log.Error($"Plugin loading error: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void LoadPluginsFromAssembly(Assembly assembly)
        {
            Log.Info($"Проверка сборки: {assembly.GetName().Name}");
            var pluginTypes = assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(Plugin<PluginConfig>)) && !t.IsAbstract)
                .ToList();
            Log.Info($"Найдено типов плагинов: {pluginTypes.Count}");
            if (pluginTypes.Count > 1)
            {
                Log.Warning($"Сборка {assembly.GetName().Name} содержит более одного плагина: {string.Join(", ", pluginTypes.Select(t => t.FullName))}");
            }
            foreach (var type in pluginTypes)
            {
                Log.Info($"Обработка типа: {type.FullName}");
                var attribute = type.GetCustomAttribute<PluginAttribute>();
                if (attribute == null) continue;

                try
                {
                    var plugin = (Plugin<PluginConfig>)Activator.CreateInstance(type);
                    LoadPluginInternal(plugin, attribute.Name.ToLower());
                }
                catch (Exception ex)
                {
                    Log.Error($"Failed to create plugin {type.Name}: {ex.Message}");
                }
            }
        }

        private void LoadPluginInternal(Plugin<PluginConfig> plugin, string pluginId)
        {
            if (_loadedPlugins.ContainsKey(pluginId))
            {
                Log.Warning($"Plugin {pluginId} already loaded");
                return;
            }

            var configPath = Path.Combine(_configsDirectory, $"{pluginId}.json");
            LoadConfig(pluginId, configPath, plugin.Config);

            if (plugin.Config.IsEnabled)
            {
                plugin.OnEnabled();
                _loadedPlugins[pluginId] = new PluginData(
                    pluginId,
                    plugin.Name,
                    plugin.Version.ToString(),
                    plugin,
                    plugin.Config
                );
                Log.Info($"Plugin loaded: {plugin.Name} v{plugin.Version}");
            }
            else
            {
                Log.Warning($"Plugin {plugin.Name} is disabled in config");
            }
        }

        public void LoadPlugin(string pluginPath)
        {
            kcp2k.Log.Warning($"Метод LoadPlugin({pluginPath}) не реализован, используется LoadAllPlugins.");
        }

        public IEnumerable<IPluginData> GetLoadedPlugins()
        {
            return _loadedPlugins.Values;
        }

        public void UnloadPlugin(string pluginId)
        {
            if (_loadedPlugins.ContainsKey(pluginId))
            {
                var plugin = _loadedPlugins[pluginId];
                if (plugin.OriginalInstance is Plugin<PluginConfig> originalPlugin)
                {
                    originalPlugin.OnDisabled();
                }
                _loadedPlugins.Remove(pluginId);
                _pluginConfigs.Remove(pluginId);
                _pluginDependencies.Remove(pluginId);
                kcp2k.Log.Info($"Плагин выгружен: {plugin.Name} (ID: {pluginId})");
            }
            else
            {
                kcp2k.Log.Warning($"Плагин с ID {pluginId} не найден.");
            }
        }

        public bool IsPluginLoaded(string pluginId)
        {
            return _loadedPlugins.ContainsKey(pluginId);
        }

        public void LoadConfig(string pluginId, string configPath, IConfig config)
        {
            try
            {
                if (File.Exists(configPath))
                {
                    var configJson = File.ReadAllText(configPath);
                    config.IsEnabled = configJson.Contains("\"IsEnabled\": true");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Config load error: {ex.Message}");
            }
        }

        public Dictionary<string, string> GetDependencies(string pluginId)
        {
            return _pluginDependencies.ContainsKey(pluginId) ? _pluginDependencies[pluginId] : new Dictionary<string, string>();
        }

        private class PluginData : IPluginData
        {
            public object OriginalInstance { get; }
            public string Name { get; }
            public string Id { get; }
            public string Version { get; }
            public ushort BundleVersion => 1;
            public IConfig Config { get; }

            public PluginData(string id, string name, string version, object instance, IConfig config)
            {
                Id = id;
                Name = name;
                Version = version;
                OriginalInstance = instance;
                Config = config;
            }
        }
    }

}
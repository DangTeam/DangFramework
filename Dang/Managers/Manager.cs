/*
using Dang.Interfaces;
using Dang.Features;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System;

namespace Dang.Managers
{
    public class Manager : IPluginLoader
    {
        private readonly Dictionary<string, IPluginData> _loadedPlugins = new();
        private readonly Dictionary<string, Dictionary<string, string>> _pluginDependencies = new();
        private readonly Dictionary<string, PluginConfig> _pluginConfigs = new();
        private readonly string _pluginsDirectory;
        private readonly string _configsDirectory;

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
                kcp2k.Log.Info("Загрузка всех плагинов из предзагруженных сборок...");
                var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in assemblies)
                {
                    if (assembly.GetName().Name.ToLower() == "dang.api")
                        continue;

                    var pluginTypes = assembly.GetTypes()
                        .Where(t => typeof(Features.DangPlugin<>).MakeGenericType(typeof(PluginConfig)).IsAssignableFrom(t) && !t.IsAbstract && t.GetCustomAttribute<Attribute.PluginAttribute>() != null);

                    foreach (var type in pluginTypes)
                    {
                        kcp2k.Log.Info($"Обнаружен плагин в сборке: {type.FullName} в {assembly.GetName().Name}");
                        var pluginInstance = (DangPlugin<PluginConfig>)Activator.CreateInstance(type);
                        if (pluginInstance != null)
                        {
                            var pluginId = pluginInstance.Id;
                            LoadPluginInternal(pluginInstance, pluginId, assembly.Location);
                        }
                    }
                }
                kcp2k.Log.Info("Все плагины загружены.");
            }
            catch (Exception ex)
            {
                kcp2k.Log.Error($"Ошибка при загрузке плагинов: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void LoadPluginInternal(DangPlugin<PluginConfig> plugin, string pluginId, string filePath)
        {
            try
            {
                if (_loadedPlugins.ContainsKey(pluginId))
                {
                    kcp2k.Log.Warning($"Плагин с ID {pluginId} уже загружен, пропускаем.");
                    return;
                }

                var configPath = Path.Combine(_configsDirectory, $"{pluginId}.json");
                LoadConfig(pluginId, configPath);

                plugin.OnEnable();
                _loadedPlugins[pluginId] = new PluginData(pluginId, plugin.Name, plugin.Version, plugin);
                kcp2k.Log.Info($"Плагин успешно загружен: {plugin.Name} v{plugin.Version}");
            }
            catch (Exception ex)
            {
                kcp2k.Log.Error($"Ошибка при загрузке плагина {pluginId}: {ex.Message}");
            }
        }

        public void LoadPlugin(string pluginId)
        {
            // Не требуется, так как используем LoadAllPlugins
            kcp2k.Log.Warning($"Метод LoadPlugin({pluginId}) не реализован, используется LoadAllPlugins.");
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
                if (plugin.OriginalInstance is DangPlugin<PluginConfig> originalPlugin)
                {
                    originalPlugin.OnDisable();
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

        public void LoadConfig(string pluginId, string configPath)
        {
            if (File.Exists(configPath))
            {
                try
                {
                    var configText = File.ReadAllText(configPath);
                    var config = new PluginConfig { IsEnabled = configText.Contains("IsEnabled=true") };
                    _pluginConfigs[pluginId] = config;
                    kcp2k.Log.Info($"Конфигурация для {pluginId} загружена из {configPath}");
                }
                catch (Exception ex)
                {
                    kcp2k.Log.Error($"Ошибка загрузки конфигурации для {pluginId}: {ex.Message}");
                    _pluginConfigs[pluginId] = new PluginConfig();
                }
            }
            else
            {
                kcp2k.Log.Warning($"Конфигурация для {pluginId} не найдена, создаем пустую.");
                _pluginConfigs[pluginId] = new PluginConfig();
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

            public PluginData(string id, string name, string version, object instance)
            {
                Id = id;
                Name = name;
                Version = version;
                OriginalInstance = instance;
            }
        }
    }
}
*/
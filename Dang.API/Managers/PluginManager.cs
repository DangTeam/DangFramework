using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Akequ.Plugins;
using Dang.API.Attribute;
using Dang.API.Features;
using Dang.API.Interfaces;
using dotnow;
using dotnow.Interop;

namespace Dang.API.Managers
{
    public class PluginManager : IPluginLoader
    {
        private readonly Dictionary<string, PluginInfo> _loadedPlugins = new();
        private readonly Dictionary<string, Dictionary<string, string>> _pluginDependencies = new();
        private readonly Dictionary<string, PluginConfig> _pluginConfigs = new();
        private readonly string _pluginsDirectory;
        private readonly string _configsDirectory;

        public PluginManager(string pluginsDirectory, string configsDirectory)
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
                Log.Info("Загрузка всех плагинов из Dang/Plugins...");
                var pluginFiles = Directory.GetFiles(_pluginsDirectory, "*.dll");
                foreach (var file in pluginFiles)
                {
                    Log.Info($"Обнаружен файл плагина: {file}");
                    try
                    {
                        var assembly = Assembly.LoadFrom(file);
                        var pluginTypes = assembly.GetTypes()
                            .Where(t => typeof(Plugin<>).MakeGenericType(typeof(PluginConfig)).IsAssignableFrom(t) && !t.IsAbstract && t.GetCustomAttribute<PluginAttribute>() != null);

                        foreach (var type in pluginTypes)
                        {
                            Log.Info($"Обнаружен плагин: {type.FullName} в {file}");
                            var configType = typeof(PluginConfig); 
                            var pluginType = type.MakeGenericType(configType);
                            var pluginInstance = (Plugin<PluginConfig>)Activator.CreateInstance(pluginType);
                            if (pluginInstance != null)
                            {
                                var pluginId = pluginInstance.Name.ToLower();
                                LoadPluginInternal(pluginInstance, pluginId, file);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Ошибка при загрузке {file}: {ex.Message}");
                    }
                }
                Log.Info("Все плагины загружены.");
            }
            catch (Exception ex)
            {
                Log.Error($"Ошибка при загрузке плагинов: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void LoadPluginInternal(Plugin<PluginConfig> plugin, string pluginId, string filePath)
        {
            try
            {
                if (_loadedPlugins.ContainsKey(pluginId))
                {
                    Log.Warning($"Плагин с ID {pluginId} уже загружен, пропускаем.");
                    return;
                }

                var configPath = Path.Combine(_configsDirectory, $"{pluginId}.json");
                LoadConfig(pluginId, configPath);

                plugin.OnEnable();
                _loadedPlugins[pluginId] = new DummyPluginInfo(pluginId, plugin.Name, plugin.Version);
                Log.Info($"Плагин успешно загружен: {plugin.Name} v{plugin.Version}");
            }
            catch (Exception ex)
            {
                Log.Error($"Ошибка при загрузке плагина {pluginId}: {ex.Message}");
            }
        }

        public void LoadPlugin(string pluginId)
        {
            // Не требуется, так как используем LoadAllPlugins
        }

        public IEnumerable<PluginInfo> GetLoadedPlugins()
        {
            return _loadedPlugins.Values;
        }

        public void UnloadPlugin(string pluginId)
        {
            if (_loadedPlugins.ContainsKey(pluginId))
            {
                var plugin = _loadedPlugins[pluginId];
                if (plugin is DummyPluginInfo dummy)
                {
                    var originalPlugin = (Plugin<PluginConfig>)dummy.OriginalInstance;
                    originalPlugin.OnDisable();
                }
                _loadedPlugins.Remove(pluginId);
                _pluginConfigs.Remove(pluginId);
                _pluginDependencies.Remove(pluginId);
                Log.Info($"Плагин выгружен: {plugin.Name} (ID: {pluginId})");
            }
            else
            {
                Log.Warning($"Плагин с ID {pluginId} не найден.");
            }
        }

        private PluginInfo FindLoadedPlugin(string pluginId)
        {
            return _loadedPlugins.ContainsKey(pluginId) ? _loadedPlugins[pluginId] : null;
        }

        public bool IsPluginLoaded(string pluginId)
        {
            return _loadedPlugins.ContainsKey(pluginId);
        }

        private Dictionary<string, string> GetDependenciesFromPlugin(PluginInfo plugin)
        {
            return new Dictionary<string, string>(); // Простая реализация без зависимостей
        }

        private void InitializePlugin(PluginInfo plugin, Dictionary<string, string> dependencies)
        {
            // Не требуется, так как плагины инициализируются в LoadPluginInternal
        }

        private dotnow.AppDomain GetAppDomainForPlugin(PluginInfo plugin)
        {
            return new dotnow.AppDomain();
        }

        private CLRInstance CreateCLRInstanceForPlugin(PluginInfo plugin)
        {
            return new CLRInstance(null);
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
                    Log.Info($"Конфигурация для {pluginId} загружена из {configPath}");
                }
                catch (Exception ex)
                {
                    Log.Error($"Ошибка загрузки конфигурации для {pluginId}: {ex.Message}");
                    _pluginConfigs[pluginId] = new PluginConfig();
                }
            }
            else
            {
                Log.Warning($"Конфигурация для {pluginId} не найдена, создаем пустую.");
                _pluginConfigs[pluginId] = new PluginConfig();
            }
        }

        public Dictionary<string, string> GetDependencies(string pluginId)
        {
            return _pluginDependencies.ContainsKey(pluginId) ? _pluginDependencies[pluginId] : new Dictionary<string, string>();
        }

        private bool CheckDependencies(Dictionary<string, string> dependencies)
        {
            return true; // Простая реализация без проверки зависимостей
        }

        private bool CheckVersionCompatibility(string requiredVersion, string actualVersion)
        {
            return true; // Простая реализация без проверки версий
        }

        // Временный класс для обёртки
        private class DummyPluginInfo : PluginInfo
        {
            public object OriginalInstance { get; }
            public override string Name { get; }
            public override string Id { get; }
            public override string Version { get; }
            public override ushort BundleVersion => 1;

            public DummyPluginInfo(string id, string name, string version)
            {
                Id = id;
                Name = name;
                Version = version;
            }

            public DummyPluginInfo(string id, string name, string version, object instance)
                : this(id, name, version)
            {
                OriginalInstance = instance;
            }
        }
    }
}
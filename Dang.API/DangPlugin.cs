using System;
using System.IO;
using System.Linq;
using Akequ.Plugins;
using Dang.API.Features;
using Dang.API.Interfaces;
using dotnow;
using dotnow.Interop;
using Dang.API.Managers;

namespace Dang.API
{
    public class DangPlugin : PluginInfo, ICLRProxy
    {
        public override string Name => "Dang Framework";
        public override string Id => "dang.framework";
        public override string Version => "1.0.0";
        public override ushort BundleVersion => 1;

        private static readonly string DangDirectory = Path.Combine(Environment.CurrentDirectory, "Dang");
        private static readonly string PluginsDirectory = Path.Combine(DangDirectory, "Plugins");
        private static readonly string ConfigsDirectory = Path.Combine(DangDirectory, "Configs");
        private static readonly string LogsDirectory = Path.Combine(DangDirectory, "Logs");

        private PluginManager _pluginManager;

        public CLRInstance Instance { get; private set; }
        public CLRType InstanceType => Instance?.Type;

        public void InitializeProxy(dotnow.AppDomain domain, CLRInstance instance)
        {
            Instance = instance;
            try
            {
                Log.Info("Инициализация Dang Framework...");

                // Диагностика: выводим все классы, реализующие PluginInfo
                Log.Info("Диагностика загруженных сборок:");
                var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in assemblies)
                {
                    try
                    {
                        var pluginTypes = assembly.GetTypes()
                            .Where(t => typeof(PluginInfo).IsAssignableFrom(t) && !t.IsAbstract);
                        foreach (var type in pluginTypes)
                        {
                            Log.Info($"Обнаружен PluginInfo: {type.FullName} в сборке {assembly.GetName().Name}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Ошибка при проверке сборки {assembly.GetName().Name}: {ex.Message}");
                    }
                }

                CreateDirectoryStructure();
                Log.SetLogLevel(Log.LogLevel.Info);
                _pluginManager = new PluginManager(PluginsDirectory, ConfigsDirectory);
                _pluginManager.LoadAllPlugins(); // Вызов загрузки плагинов
                RegisterHooks();
                Log.Info("Dang Framework успешно инициализирован.");
            }
            catch (Exception ex)
            {
                Log.Error($"Ошибка инициализации фреймворка: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void CreateDirectoryStructure()
        {
            try
            {
                Directory.CreateDirectory(DangDirectory);
                Directory.CreateDirectory(LogsDirectory);
                Directory.CreateDirectory(PluginsDirectory);
                Directory.CreateDirectory(ConfigsDirectory);

                string configFile = Path.Combine(ConfigsDirectory, "Dang.yml");
                if (!File.Exists(configFile))
                {
                    File.WriteAllText(configFile, "# Dang Framework Configuration\nlog_level: info\nenabled: true");
                }

                Log.Info("Структура директорий и конфиг созданы или проверены.");
            }
            catch (Exception ex)
            {
                Log.Error($"Ошибка создания структуры директорий: {ex.Message}");
            }
        }

        private void RegisterHooks()
        {
            try
            {
                HookManager.Add(null, "NRServer", _ =>
                {
                    Log.Info("Перезагрузка сервера, перезагрузка плагинов...");
                    _pluginManager = new PluginManager(PluginsDirectory, ConfigsDirectory);
                    _pluginManager.LoadAllPlugins();
                });
            }
            catch (Exception ex)
            {
                Log.Error($"Ошибка при регистрации хуков: {ex.Message}");
            }
        }
    }
}
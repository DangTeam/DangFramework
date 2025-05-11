using Akequ.Plugins;
using System.IO;
using System;

namespace Dang
{
    public class APIDang : PluginInfo
    {
        public override string Name => "Dang API";
        public override string Id => "dang.API";
        public override string Version => "1.0.0";
        public override ushort BundleVersion => 1;
        public APIDang() { StartAsembly(); }

        private static readonly string DangDirectory = Path.Combine(Environment.CurrentDirectory, "Dang");
        private static readonly string PluginsDirectory = Path.Combine(DangDirectory, "Plugins");
        private static readonly string ConfigsDirectory = Path.Combine(DangDirectory, "Configs");
        private static readonly string LogsDirectory = Path.Combine(DangDirectory, "Logs");

        //private Managers.Manager _Manager;

        public void StartAsembly()
        {
            try
            {
                kcp2k.Log.Info("Инициализация Dang Framework...");
                CreateDirectoryStructure();
                //_Manager = new Managers.Manager(PluginsDirectory, ConfigsDirectory);
                //_Manager.LoadAllPlugins();
                RegisterHooks();
                kcp2k.Log.Info("Dang Framework успешно инициализирован.");
            }
            catch (Exception ex)
            {
                kcp2k.Log.Error($"Ошибка инициализации фреймворка: {ex.Message}\n{ex.StackTrace}");
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

                kcp2k.Log.Info("Структура директорий и конфиг созданы или проверены.");
            }
            catch (Exception ex)
            {
                kcp2k.Log.Error($"Ошибка создания структуры директорий: {ex.Message}");
            }
        }

        private void RegisterHooks()
        {
            try
            {
                HookManager.Add(null, "NRServer", _ =>
                {
                    kcp2k.Log.Info("Перезагрузка сервера, перезагрузка плагинов...");
                    //_Manager = new Managers.Manager(PluginsDirectory, ConfigsDirectory);
                    //_Manager.LoadAllPlugins();
                });
            }
            catch (Exception ex)
            {
                kcp2k.Log.Error($"Ошибка при регистрации хуков: {ex.Message}");
            }
        }
    }
}

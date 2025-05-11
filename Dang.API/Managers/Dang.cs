using System;
using System.IO;
using Dang.API.Features;
using Dang.API.Managers;

namespace Dang.API.Managers
{
    public class Dang
    {
        private static readonly string DangDirectory = Path.Combine(Environment.CurrentDirectory, "Dang"); 
        private static readonly string PluginsDirectory = Path.Combine(DangDirectory, "Plugins"); 
        private static readonly string ConfigsDirectory = Path.Combine(DangDirectory, "Configs"); 
        private static readonly string LogsDirectory = Path.Combine(DangDirectory, "Logs");

        private Manager _manager;

        public void StartAssembly()
        {
            try
            {
                Log.Info("Initializing Dang Framework...");
                CreateDirectoryStructure();
                Log.SetLogLevel(Log.LogLevel.Info);
                _manager = new Manager(PluginsDirectory, ConfigsDirectory);
                _manager.LoadAllPlugins();
                RegisterHooks();
                Log.Info("Dang Framework initialized successfully.");
            }
            catch (Exception ex)
            {
                Log.Error($"Framework initialization error: {ex.Message}\n{ex.StackTrace}");
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

                Log.Info("Directory structure verified.");
            }
            catch (Exception ex)
            {
                Log.Error($"Directory creation error: {ex.Message}");
            }
        }

        private void RegisterHooks()
        {
            try
            {
                HookManager.Add(null, "ServerRestart", _ =>
                {
                    Log.Info("Reloading plugins...");
                    _manager = new Manager(PluginsDirectory, ConfigsDirectory);
                    _manager.LoadAllPlugins();
                });
            }
            catch (Exception ex)
            {
                Log.Error($"Hook registration error: {ex.Message}");
            }
        }
    }

}
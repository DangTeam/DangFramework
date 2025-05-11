using System;
using System.IO;
using System.Reflection;
using Dang.API.Enums;
using Dang.API.Interfaces;
using Dang.API.Attribute;
using Akequ.Plugins;

namespace Dang.API.Features
{
    public abstract class Plugin<TConfig> : PluginInfo, IPlugin<TConfig> where TConfig : IConfig, new()
    {
        private readonly Assembly _assembly; private TConfig _config; private readonly Version _versionObj;

        protected Plugin()
        {
            _assembly = Assembly.GetCallingAssembly();
            var attr = GetType().GetCustomAttribute<PluginAttribute>();

            if (attr != null)
            {
                Name = attr.Name;
                Description = attr.Description;
                Author = attr.Author;
                _versionObj = new Version(attr.Version);
            }
            else
            {
                throw new InvalidOperationException($"Плагин {GetType().Name} не имеет атрибута [Plugin]");
            }

            _config = new TConfig();
        }

        public override string Name { get; }
        public virtual string Description { get; }
        public virtual string Author { get; }
        public override string Version => _versionObj.ToString();
        Version IPlugin<TConfig>.Version => _versionObj;
        public virtual PluginPriority Priority { get; } = PluginPriority.Medium;
        public virtual Version RequiredDangVersion { get; } = typeof(IPlugin<>).Assembly.GetName().Version;
        public virtual bool IgnoreVersionCheck { get; }
        public override string Id => Name.ToLower();
        public override ushort BundleVersion => 1;

        public TConfig Config
        {
            get => _config ??= new TConfig();
            internal set => _config = value;
        }

        public virtual string ConfigPath => Path.Combine("Dang", "Configs", $"{Id}.yml");
        public virtual string TranslationsPath => Path.Combine("Dang", "Translations", $"{Id}.yml");

        public virtual void OnEnabled()
        {
            if (Config.IsEnabled)
            {
                Log.Info($"Загружен {Name}, v{Version} от {Author}");
            }
            else
            {
                Log.Warning($"{Name} отключен в конфигурации.");
            }
        }

        public virtual void OnDisabled()
        {
            Log.Info($"Выгружен {Name}");
        }

        public virtual void OnReloaded()
        {
            Log.Info($"Перезагружен {Name}");
        }

        public int CompareTo(IPlugin<IConfig> other)
        {
            return -Priority.CompareTo(other.Priority);
        }
    }

}
using System;
using System.Reflection;
using Dang.API.Attribute;
using Dang.API.Interfaces;

namespace Dang.API.Features
{
    public abstract class Plugin<TConfig> where TConfig : IConfig, new()
    {
        public string Name { get; }
        public string Description { get; }
        public string Author { get; }
        public string Version { get; }
        public TConfig Config { get; private set; }

        protected Plugin()
        {
            var attr = GetType().GetCustomAttribute<PluginAttribute>();

            if (attr != null)
            {
                Name = attr.Name;
                Description = attr.Description;
                Author = attr.Author;
                Version = attr.Version;
            }
            else
            {
                throw new InvalidOperationException($"Плагин {GetType().Name} не имеет атрибута [Plugin]");
            }

            Config = new TConfig();
        }

        public virtual void OnEnable()
        {
            if (Config.IsEnabled)
            {
                Log.Info($"Загружен {Name}, v.{Version} от {Author}");
            }
            else
            {
                Log.Warning($"{Name} отключен в конфигурации.");
            }
        }

        public virtual void OnDisable()
        {
            Log.Info($"Выгружен {Name}, v.{Version} от {Author}");
        }

        public virtual void OnReloaded()
        {
            Log.Info($"Перезагружен {Name}, v.{Version} от {Author}");
        }
    }
}
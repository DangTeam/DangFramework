using Akequ.Plugins;
using Dang.Interfaces;
using System;
using System.Reflection;
using Dang.Attribute; 

namespace Dang.Features
{
    public abstract class DangPlugin<TConfig> : PluginInfo where TConfig : class, IConfig, new()
    {
        public override string Name { get; }
        public string Description { get; }
        public string Author { get; }
        public override string Version { get; }
        public TConfig Config { get; private set; }

        public override string Id => Name.ToLower();
        public override ushort BundleVersion => 1;

        protected DangPlugin()
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
                Dang.Features.Log.Info($"Загружен {Name}, v.{Version} от {Author}");
            }
            else
            {
                Dang.Features.Log.Warning($"{Name} отключен в конфигурации.");
            }
        }

        public virtual void OnDisable()
        {
            Dang.Features.Log.Info($"Выгружен {Name}, v.{Version} от {Author}");
        }

        public virtual void OnReloaded()
        {
            Dang.Features.Log.Info($"Перезагружен {Name}, v.{Version} от {Author}");
        }
    }
}
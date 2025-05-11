using Dang.API.Enums;
using System;
using System.Reflection;

namespace Dang.API.Interfaces
{
    public interface IPlugin<TConfig> : IComparable<IPlugin<IConfig>> where TConfig : IConfig
    {
        string Name { get; }
        string Author { get; }
        string Description { get; }
        PluginPriority Priority { get; }
        Version Version { get; }
        Version RequiredDangVersion { get; }
        bool IgnoreVersionCheck { get; }
        TConfig Config { get; }

        void OnEnabled();
        void OnDisabled();
        void OnReloaded();
    }
}
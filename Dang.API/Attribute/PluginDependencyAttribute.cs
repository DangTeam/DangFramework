using System;
using System.Collections.Generic;

namespace Dang.API.Attribute
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class PluginDependencyAttribute : System.Attribute
    {
        public string PluginId { get; }
        public string Version { get; }

        public PluginDependencyAttribute(string pluginId, string version)
        {
            PluginId = pluginId;
            Version = version;
        }
    }
}

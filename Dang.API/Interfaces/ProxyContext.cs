using dotnow;
using System.Collections.Generic;

namespace Dang.API.Interfaces
{
    public class ProxyContext
    {
        public PluginConfig Config { get; set; }
        public Dictionary<string, string> Dependencies { get; set; }
        public dotnow.AppDomain AppDomain { get; set; } 
        public CLRInstance Instance { get; set; }
    }
}

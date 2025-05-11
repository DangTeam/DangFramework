using Akequ.Plugins;
using Dang.API.Features;

namespace Dang.Events
{
    public class EventsPlugin : PluginInfo
    {
        public override string Name => "Dang Events";
        public override string Id => "dang.events";
        public override string Version => "1.0.0";
        public override ushort BundleVersion => 1;
       // private Harmony? harmony;
        public void Initialize()
        {
            Log.Info("Dang Events initialized.");
        }
    }
}
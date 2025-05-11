using Akequ.Plugins;
using Dang.API.Features;
using Dang.API.Interfaces;
using Dang.API.Attribute;

namespace Dang.API
{
    [Plugin("Dang API", "Core API for Dang Framework", "Author Name", "1.0.0")]
    public class APIDang : Plugin<PluginConfig>
    {
        public APIDang() { Log.Info("Dang API initialized."); }

        public override void OnEnabled()
        {
            base.OnEnabled();
        }
    }

}
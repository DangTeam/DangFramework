namespace Dang.API.Interfaces
{
    public class PluginConfig : IConfig
    {
        public bool IsEnabled { get; set; } = true;
    }
}
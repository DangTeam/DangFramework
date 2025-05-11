using Dang.API.Interfaces;

namespace Dang.Events
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
    }
}

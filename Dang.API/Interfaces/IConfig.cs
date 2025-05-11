using System.ComponentModel;

namespace Dang.API.Interfaces
{
    public interface IConfig
    {
        bool IsEnabled { get; set; }
    }
}
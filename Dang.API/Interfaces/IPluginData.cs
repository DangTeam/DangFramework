namespace Dang.API.Interfaces
{
    public interface IPluginData
    {
        string Id { get; }
        string Name { get; }
        string Version { get; }
        ushort BundleVersion { get; }
        object OriginalInstance { get; }
        IConfig Config { get; }
    }
}
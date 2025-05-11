using System;

namespace Dang.API.Attribute
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class PluginAttribute : System.Attribute
    {
        public string Name { get; }
        public string Description { get; }
        public string Author { get; }
        public string Version { get; }

        public PluginAttribute(string name, string description, string author, string version)
        {
            Name = name;
            Description = description;
            Author = author;
            Version = version;
        }
    }

}
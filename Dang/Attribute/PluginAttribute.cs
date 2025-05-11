using System;

namespace Dang.Attribute
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PluginAttribute : System.Attribute
    {
        public string Name { get; }
        public string Author { get; }
        public string Version { get; }
        public string Description { get; }

        public PluginAttribute(string name, string author, string version, string description)
        {
            Name = name;
            Author = author;
            Version = version;
            Description = description;
        }
    }
}

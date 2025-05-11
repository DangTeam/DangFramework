namespace Dang.Interfaces
{
    public interface ICommand
    {
        string Name { get; }
        string[] Aliases { get; }
        string Description { get; }

        bool Execute(string[] args, out string? response);
    }
}

using System.Drawing;

namespace Server.ArenaItems
{
    public interface ICell : ICommand
    {
        Color Color { get; }
    }
}
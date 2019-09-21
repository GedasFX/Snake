using System.Drawing;

namespace Server.ArenaItems
{
    public interface ICell
    {
        Color Color { get; }
        void Interact(Snek snek);
    }
}
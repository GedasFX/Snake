using System.Drawing;

namespace Snake.ArenaItems
{
    public interface ICell
    {
        Brush Color { get; }
        void Interact(Snek snek);
    }
}
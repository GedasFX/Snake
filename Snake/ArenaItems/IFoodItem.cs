using System.Drawing;

namespace Snake.ArenaItems
{
    public interface IFoodItem : ICell
    {
        Brush Color { get; }
    }
}
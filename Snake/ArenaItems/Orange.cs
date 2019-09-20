using System.Drawing;

namespace Snake.ArenaItems
{
    public class Orange : IFoodItem
    {
        public Brush Color { get; } = Brushes.Orange;

        public void Interact(Snek snek)
        {
            snek.AddGrowth(2);
        }
    }
}
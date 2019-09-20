using System.Drawing;

namespace Snake.ArenaItems
{
    public class Apple : IFoodItem
    {
        public Brush Color { get; } = Brushes.GreenYellow;

        public void Interact(Snek snek)
        {
            snek.Growth += 1;
        }
    }
}
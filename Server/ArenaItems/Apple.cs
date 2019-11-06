using System.Drawing;

namespace Server.ArenaItems
{
    public class Apple : IFoodItem
    {
        public Color Color { get; } = Color.GreenYellow;

        public void Interact(Snek snek)
        {
            snek.Growth += 1;
        }

        public void Undo(Snek snek)
        {
            snek.Growth -= 1;
        }
    }
}
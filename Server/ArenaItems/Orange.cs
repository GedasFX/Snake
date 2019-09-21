using System.Drawing;

namespace Server.ArenaItems
{
    public class Orange : IFoodItem
    {
        public Color Color { get; } = Color.Orange;

        public void Interact(Snek snek)
        {
            snek.Growth += 2;
        }
    }
}
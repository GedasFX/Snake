using System.Drawing;

namespace Server.ArenaItems
{
    public class CustomColorDecorator : IFoodItem
    {
        private readonly IFoodItem _baseFoodItem;

        public CustomColorDecorator(IFoodItem baseFoodItem, Color newColor)
        {
            _baseFoodItem = baseFoodItem;
            Color = newColor;
        }

        public void Interact(Snek snek)
        {
            _baseFoodItem.Interact(snek);
        }

        public void Undo(Snek snek)
        {
            _baseFoodItem.Undo(snek);
        }

        public Color Color { get; }
    }
}

using System.Drawing;
using System.Threading.Tasks;

namespace Server.ArenaItems
{
    public class BlinkingDecorator : IFoodItem
    {
        private readonly IFoodItem _baseFoodItem;
        private readonly Color _blinkColor;

        public BlinkingDecorator(IFoodItem baseFoodItem, Color blinkColor)
        {
            _baseFoodItem = baseFoodItem;
            _blinkColor = blinkColor;
            Blink().ConfigureAwait(false);
        }

        public void Interact(Snek snek)
        {
            _baseFoodItem.Interact(snek);
        }

        public void Undo(Snek snek)
        {
            _baseFoodItem.Undo(snek);
        }

        private async Task Blink()
        {
            while (true)
            {
                Color = _baseFoodItem.Color;
                await Task.Delay(500);

                Color = _blinkColor;
                await Task.Delay(500);
            }
        }

        public Color Color { get; private set; }
    }
}

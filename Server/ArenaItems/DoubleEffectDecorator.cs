using System.Drawing;
using System.Threading.Tasks;

namespace Server.ArenaItems
{
    public class DoubleEffectDecorator : IFoodItem
    {
        private readonly IFoodItem _baseFoodItem;

        public DoubleEffectDecorator(IFoodItem baseFoodItem)
        {
            _baseFoodItem = baseFoodItem;
            Color = Color.BlanchedAlmond;;
        }

        public void Interact(Snek snek)
        {
            _baseFoodItem.Interact(snek);
            _baseFoodItem.Interact(snek);
        }

        public void Undo(Snek snek)
        {
            _baseFoodItem.Undo(snek);
            _baseFoodItem.Undo(snek);
        }

        public Color Color { get; }
    }
}

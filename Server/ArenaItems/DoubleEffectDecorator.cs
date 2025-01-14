﻿using System.Drawing;

namespace Server.ArenaItems
{
    public class DoubleEffectDecorator : IFoodItem
    {
        private readonly IFoodItem _baseFoodItem;

        public DoubleEffectDecorator(IFoodItem baseFoodItem)
        {
            _baseFoodItem = baseFoodItem;
            Color = baseFoodItem.Color;
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

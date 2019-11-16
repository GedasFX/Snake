using System;
using System.Drawing;
using Server.ArenaItems;

namespace Server
{
    public class FoodFactory : IArenaObjectFactory
    {
        private static readonly Random Random = new Random();

        private static IFoodItem Apple { get; } = new Apple();
        private static IFoodItem Orange { get; } = new Orange();
        private static IFoodItem BrownApple { get; } = new CustomColorDecorator(Apple, Color.Brown);
        private static IFoodItem DoubleYellowOrange { get; } = 
            new DoubleEffectDecorator(new CustomColorDecorator(Orange, Color.Yellow));

        public static IFoodItem GenerateFoodItem()
        {
            return GenerateFoodItem(Random.Next(4));
        }

        public static IFoodItem GenerateFoodItem(int index)
        {
            return index switch
            {
                0 => Apple,
                1 => Orange,
                2 => BrownApple,
                3 => DoubleYellowOrange,
                _ => throw new ArgumentOutOfRangeException(nameof(index), index, null),
            };
        }

        public ICell CreateObject()
        {
            return GenerateFoodItem();
        }
    }
}
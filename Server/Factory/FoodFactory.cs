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
                2 => new BlinkingDecorator(new Apple(), Color.Black),
                3 => new DoubleEffectDecorator(new BlinkingDecorator(new Apple(), Color.Blue)),
                _ => throw new ArgumentOutOfRangeException(nameof(index), index, null),
            };
        }

        public ICell CreateObject()
        {
            return GenerateFoodItem();
        }
    }
}
using System;
using Server.ArenaItems;

namespace Server
{
    public class FoodFactory : IArenaObjectFactory
    {
        private static readonly Random Random = new Random();

        private static Apple Apple { get; } = new Apple();
        private static Orange Orange { get; } = new Orange();

        public static IFoodItem GenerateFoodItem()
        {
            return GenerateFoodItem(Random.Next(2));
        }

        public static IFoodItem GenerateFoodItem(int index)
        {
            return index switch
            {
                0 => Apple as IFoodItem,
                1 => Orange,
                _ => throw new ArgumentOutOfRangeException(nameof(index), index, null),
            };
        }

        public ICell CreateObject()
        {
            return GenerateFoodItem();
        }
    }
}
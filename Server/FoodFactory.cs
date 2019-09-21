using System;
using Server.ArenaItems;

namespace Server
{
    public class FoodFactory
    {
        private static readonly Random Random = new Random();

        public static Apple Apple { get; set; } = new Apple();
        public static Orange Orange { get; set; } = new Orange();

        public static IFoodItem GenerateFoodItem()
        {
            return GenerateFoodItem(Random.Next(2));
        }

        public static IFoodItem GenerateFoodItem(int index)
        {
            switch (index)
            {
                case 0:
                    return Apple;
                case 1:
                    return Orange;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
        }
    }
}
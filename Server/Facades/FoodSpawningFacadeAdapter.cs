using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Facades
{
    public class FoodSpawningFacadeAdapter : FoodSpawningFacade
    {
        StrategyAdaptee _adaptee = new StrategyAdaptee();

        public FoodSpawningFacadeAdapter(Arena arena, Random rng) : base(arena, rng)
        {
        }

        public override void SwitchStrategyAtRandom()
        {
            _adaptee.SwitchStrategy(this);
        }

        private class StrategyAdaptee
        {
            Random _rng = new Random();

            public void SwitchStrategy(FoodSpawningFacade facade)
            {
                // make it twice as unlikely
                int roll = _rng.Next(6);
                Console.Out.WriteLine("eeee");
                switch (roll)
                {
                    case 0:
                        facade.SwitchToRandomScatterStrategy();
                        break;
                    case 1:
                        facade.SwitchToSquareStrategy();
                        break;
                    case 2:
                        facade.SwitchToPlusStrategy();
                        break;
                }
            }
        }
    }
}

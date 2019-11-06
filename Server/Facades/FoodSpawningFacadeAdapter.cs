using System;

namespace Server.Facades
{
    public class FoodSpawningFacadeAdapter : FoodSpawningFacade
    {
        private readonly StrategyAdaptee _adaptee = new StrategyAdaptee();

        public FoodSpawningFacadeAdapter(Arena arena, Random rng) : base(arena, rng)
        {
        }

        public override void SwitchStrategyAtRandom()
        {
            _adaptee.SwitchStrategy(this);
        }

        private class StrategyAdaptee
        {
            private readonly Random _rng = new Random();

            public void SwitchStrategy(FoodSpawningFacade facade)
            {
                // make it twice as unlikely to change strategy by adding another failure point
                int roll = _rng.Next(6);
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
                    default:
                        Logger.Instance.LogMessage("Strategy was not changed");
                        break;
                }
            }
        }
    }
}

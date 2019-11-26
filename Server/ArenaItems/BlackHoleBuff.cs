using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Server.ArenaItems
{
    public class BlackHoleBuff : IFoodItem
    {
        public Color Color { get; } = Color.Black;

        private const int Radius = 6;

        //private IEnumerable<(int, int, ICell)> Cells(Arena arena, int x, int y)
        //{
        //    for (int i = x - Radius; i < x + Radius; i++)
        //    {
        //        for (int j = y - Radius; j < y + Radius; j++)
        //        {
        //            if (i == x || j == y)
        //                continue;

        //            var obj = arena.GetCell((arena.Width + i) % arena.Width, (arena.Height + j) % arena.Height);

        //            if (obj == null || obj is ISnekBody || obj is BlackHoleBuff)
        //                continue;

        //            yield return ((arena.Width + i) % arena.Width, (arena.Height + j) % arena.Height, obj);
        //        }
        //    }
        //}

        public void Interact(Snek snek)
        {
            var coords = snek.Body.Last.Value;
            foreach (var item in new ArenaEnumerable(snek.Arena, coords.X, coords.Y))
            {
                var obj = item.Item1;

                obj.Interact(snek);
                snek.Arena.UpdateCell(item.Item2, item.Item3, null);
            }
        }

        public void Undo(Snek snek)
        {
        }
    }

    internal class ArenaEnumerable : IEnumerable<(ICell, int, int)>, IEnumerator<(ICell, int, int)>
    {
        private readonly Arena _arena;

        private readonly int minY;
        private readonly int centerX, centerY;
        private readonly int maxX, maxY;

        private int currentX, currentY;

        private const int Radius = 5;

        public (ICell, int, int) Current => (_arena.GetCell(currentX % _arena.Width, currentY % _arena.Height), currentX % _arena.Width, currentY % _arena.Height);

        object IEnumerator.Current => Current;

        public ArenaEnumerable(Arena arena, int startX, int startY)
        {
            _arena = arena;

            centerX = startX;
            centerY = startY;

            minY = startY + arena.Height - Radius;

            maxX = startX + arena.Width + Radius;
            maxY = startY + arena.Height + Radius;

            Reset();
        }

        public bool MoveNext()
        {
            for (; currentX < maxX; currentX++)
            {
                for (; currentY < maxY; currentY++)
                {
                    if (currentX == centerX || currentY == centerY)
                        continue;

                    var obj = _arena.GetCell(currentX % _arena.Width, currentY % _arena.Height);

                    if (obj == null || obj is ISnekBody || obj is BlackHoleBuff)
                        continue;

                    return true;
                }

                currentY = minY;
            }

            return false;
        }

        public void Reset()
        {
            currentX = centerX + _arena.Width - Radius;
            currentY = centerY + _arena.Height - Radius - 1; // Must start before first element
        }

        public void Dispose() { }

        public IEnumerator<(ICell, int, int)> GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
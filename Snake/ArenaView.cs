using System.Drawing;
using Snake.ArenaItems;

namespace Snake
{
    class ArenaView
    {
        public static void Render(Graphics graphics, Arena arena)
        {
            graphics.FillRectangle(Brushes.AliceBlue, 0, 0, arena.Width * 10, arena.Height * 10);
            for (var x = 0; x < arena.Width; x++)
            {
                for (var y = 0; y < arena.Height; y++)
                {
                    if (arena.Cells[x, y] is ICell cell)
                        graphics.FillRectangle(cell.Color, x * 10, y * 10, 10, 10);
                }
            }
        }
    }
}

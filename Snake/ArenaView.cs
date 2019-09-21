using System.Collections.Generic;
using System.Drawing;

namespace Snake
{
    class ArenaView
    {
        public static void Render(Graphics graphics, Dictionary<Point, Color> colorMap, int height, int width)
        {
            graphics.FillRectangle(Brushes.AliceBlue, 0, 0, width * 10, height * 10);

            foreach (var color in colorMap)
            {
                graphics.FillRectangle(new SolidBrush(color.Value), color.Key.X * 10, color.Key.Y * 10, 10, 10);
            }
        }
    }
}

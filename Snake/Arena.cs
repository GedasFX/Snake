using System;
using System.Drawing;
using Snake.ArenaItems;

namespace Snake
{
    public class Arena
    {
        public int Width { get; }
        public int Height { get; }

        public Snek Snake { get; set; }

        public ICell[,] Cells { get; }
        private readonly Random _random = new Random(0);

        public Arena(int width, int height)
        {
            Width = width;
            Height = height;

            Cells = new ICell[width, height];

            Snake = new Snek(this, new Point(1, 1));
        }

        public void Update()
        {
            Snake.Move();
            if (_random.Next(100) <= 4)
            {
                CreateFood();
            }
        }

        public void CreateFood()
        {
            Cells[_random.Next(0, Width), _random.Next(0, Height)] = FoodFactory.GenerateFoodItem();
        }
    }
}

using System;
using System.Drawing;
using System.Windows.Forms;

namespace MySnakeAI
{
    public class Fruit
    {
        public int X;
        public int Y;
        public PictureBox Body;
        public Fruit(Snake snake, Map map)
        { 
            Body = new PictureBox();
            Body.Size = new Size(map.ElementSize, map.ElementSize);
            Body.BackColor = Color.Green;
            Spawn(snake, map);
        }
        public void Spawn(Snake snake, Map map)
        {
            var rnd = new Random();
            X = rnd.Next(0, map.Width - 1);
            Y = rnd.Next(0, map.Height - 1);
            for (int i = 0; i < snake.Body.Count; i++)
            {
                if (new Point(X * map.ElementSize, Y * map.ElementSize) == snake.Body[i].Location)
                {
                    X = rnd.Next(0, map.Width - 1);
                    Y = rnd.Next(0, map.Height - 1);
                    i = 0; 
                }
            }
            Body.Location = new Point(X * map.ElementSize, Y * map.ElementSize);
        }
    }
}

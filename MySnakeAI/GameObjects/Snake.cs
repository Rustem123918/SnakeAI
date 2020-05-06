using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MySnakeAI
{
    public enum SnakeDirection
    {
        STOP = 0, 
        UP = 1, 
        RIGHT = 2, 
        DOWN = 3, 
        LEFT = 4
    }
    public class Snake
    {
        public int X;
        public int Y;
        public SnakeDirection Dir;
        public List<PictureBox> Body;
        public Snake(Map map)
        {
            Body = new List<PictureBox>();
            Body.Add(new PictureBox());
            Body[0].Size = new Size(map.ElementSize - 1, map.ElementSize - 1);
            Body[0].BackColor = Color.Black;
            Spawn(map);
        }
        public void Spawn(Map map)
        {
            if(Body.Count>1)
                Body.RemoveRange(1, Body.Count - 1);
            Dir = SnakeDirection.STOP;
            X = map.Width / 2;
            Y = map.Height / 2;
            Body[0].Location = new Point(X * map.ElementSize, Y * map.ElementSize);
        }
        public void Move(Map map)
        {
            if (Body.Count >= 2)
            {
                Point prev = Body[1].Location;
                Body[1].Location = Body[0].Location;
                Point prev2;
                for (int i = 2; i < Body.Count; i++)
                {
                    prev2 = Body[i].Location;
                    Body[i].Location = prev;
                    prev = prev2;
                }
            }

            switch(Dir)
            {
                case SnakeDirection.UP:
                    Y--;
                    break;
                case SnakeDirection.RIGHT:
                    X++;
                    break;
                case SnakeDirection.DOWN:
                    Y++;
                    break;
                case SnakeDirection.LEFT:
                    X--;
                    break;
            }
            Body[0].Location = new Point(X * map.ElementSize, Y * map.ElementSize);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MySnakeAI
{
    public class Fruit
    {
        private const int ElementSize = GameProcess.ElementSize;
        private const int MapWidth = GameProcess.MapWidth;
        private const int MapHeight = GameProcess.MapHeight;

        private int currentX;
        private int currentY;
        public int rX;
        public int rY;
        public PictureBox Body;
        public Fruit()
        {
            GenerateFruit();
            Body = new PictureBox();
            Body.Size = new Size(ElementSize, ElementSize);
            Body.Location = new Point(rX, rY);
            Body.BackColor = Color.Green;
        }
        public void Move(Snake snake)
        {
            GenerateFruit();
            for (int i = 0; i<=GameProcess.Score; i++)
            {
                if (new Point(rX, rY) == snake.Body[i].Location || (rX == currentX && rY == currentY)) { GenerateFruit(); i = 0; }
            }
            currentX = rX;
            currentY = rY;
            Body.Location = new Point(rX, rY);
        }
        private void GenerateFruit()
        {
            Random rnd = new Random();
            rX = rnd.Next(0, MapWidth-1) * ElementSize;
            rY = rnd.Next(0, MapWidth-1) * ElementSize;
        }
    }
}

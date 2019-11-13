﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MySnakeAI
{
    public class Snake
    {
        private const int ElementSize = GameProcess.ElementSize;
        private const int MapWidth = GameProcess.MapWidth;
        private const int MapHeight = GameProcess.MapHeight;
        
        public bool IsDead;
        public int dirX;
        public int dirY;
        public int LastdirX;
        public int LastdirY;
        public PictureBox[] Body = new PictureBox[MapHeight * MapWidth];
        private PictureBox head;
        public Snake()
        {
            IsDead = false;
            dirX = 1;
            dirY = 0;
            Body[0] = new PictureBox();
            Body[0].Size = new Size(ElementSize-1, ElementSize-1);
            Body[0].Location = new Point(ElementSize*MapWidth/2, ElementSize * MapHeight / 2);
            Body[0].BackColor = Color.Black;
            head = Body[0];
        }
        public void Move()
        {
            if (!CheckOutOfBorders() && !EatItSelf())
            {
                int a = 0;
                for (int i = GameProcess.Score; i >= 1; i--)
                {
                    //Вычисляем направление хвоста
                    if (a == 0 && GameProcess.Score > 2)
                    {
                        LastdirX = (Body[i - 1].Location.X - Body[i].Location.X) / ElementSize;
                        LastdirY = (Body[i - 1].Location.Y - Body[i].Location.Y) / ElementSize;
                        a = 1;
                    }
                    //Вычисляем направление хвоста
                    Body[i].Location = Body[i - 1].Location;
                }
                head.Location = new Point(head.Location.X + dirX * ElementSize,
                   head.Location.Y + dirY * ElementSize);
            }
            else
            {
                IsDead = true;
            }
        }
        private bool CheckOutOfBorders() //true - выход за границы, false - не вышел за границы
        {
            if (head.Location.X + dirX * ElementSize < 0 || 
                head.Location.X + dirX * ElementSize >= MapWidth * ElementSize ||
                head.Location.Y + dirY * ElementSize < 0 || 
                head.Location.Y + dirY * ElementSize >= MapHeight * ElementSize)
                return true;

            if (head.Location.Y == 0) SnakeWindow.up = 1;
            else SnakeWindow.up = 0;
            if (head.Location.Y == (MapHeight - 1) * ElementSize) SnakeWindow.down = 1;
            else SnakeWindow.down = 0;
            if (head.Location.X == 0) SnakeWindow.left = 1;
            else SnakeWindow.left = 0;
            if (head.Location.X == (MapWidth - 1) * ElementSize) SnakeWindow.right = 1;
            else SnakeWindow.right = 0;

            return false;
        }
        private bool EatItSelf()
        {
            for(int i = 1; i<=GameProcess.Score; i++)
            {
                if (i == 1)
                {
                    if (SnakeWindow.up == 1 || head.Location.Y - ElementSize == Body[1].Location.Y) SnakeWindow.up = 1;
                    else SnakeWindow.up = 0;
                    if (SnakeWindow.down == 1 || head.Location.Y + ElementSize == Body[1].Location.Y) SnakeWindow.down = 1;
                    else SnakeWindow.down = 0;
                    if (SnakeWindow.left == 1 || head.Location.X - ElementSize == Body[1].Location.X) SnakeWindow.left = 1;
                    else SnakeWindow.left = 0;
                    if (SnakeWindow.right == 1 || head.Location.X + ElementSize == Body[1].Location.X) SnakeWindow.right = 1;
                    else SnakeWindow.right = 0;
                }

                if (head.Location == Body[i].Location) return true;
            }
            return false;
        }
    }
}

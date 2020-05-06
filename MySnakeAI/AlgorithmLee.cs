using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySnakeAI
{
    public class AlgorithmLee
    {
        public List<int> px; //Координаты ячеек, входящих в путь
        public List<int> py; //Координаты ячеек, входящих в путь
        public AlgorithmLee()
        {
            px = new List<int>();
            py = new List<int>();
        }
        public bool Search(Map map, Snake snake, int ax, int ay, int bx, int by)
        {
            int W = map.Width; //Ширина карты
            int H = map.Height; //Высота карты
            int WALL = -1; //Непроходимая ячейка
            int BLANK = -2; //Свободная непомеченная ячейка
            int len; //Длина пути
            int[,] grid = new int[W, H]; //Рабочее поле

            //Заполняем рабочее поле
            for (var _x = 0; _x < W; _x++)
                for (var _y = 0; _y < H; _y++)
                    for (var i = 0; i < snake.Body.Count; i++)
                    {
                        if (_x * map.ElementSize == snake.Body[i].Location.X
                            && _y * map.ElementSize == snake.Body[i].Location.Y && i != 0) 
                        { 
                            grid[_x, _y] = WALL; 
                            break; 
                        }
                        else 
                            grid[_x, _y] = BLANK;
                    }

            //Перед вызовом Lee() массив grid должен быть заполнен значениями WALL и BLANK
            var dx = new int[] { 1, 0, -1, 0 }; //Смещение соответсвующее соседям ячейки
            var dy = new int[] { 0, 1, 0, -1 }; //Справа, снизу, слева, сверху
            int d, x, y, k;
            bool stop;

            if (grid[ax, ay] == WALL || grid[bx, by] == WALL) return false;

            //Распространение волны
            d = 0;
            grid[ax, ay] = 0;
            do
            {
                stop = true;
                for (x = 0; x < W; x++)
                    for (y = 0; y < H; y++)
                    {
                        if (grid[x, y] == d)
                        {
                            for (k = 0; k < 4; k++)
                            {
                                int iy = y + dy[k], ix = x + dx[k];
                                if (iy >= 0 && iy < H && ix >= 0 && ix < W && grid[ix, iy] == BLANK)
                                {
                                    stop = false; //Найдены свободные непомеченные ячейки
                                    grid[ix, iy] = d + 1; //Распространяем волну
                                }
                            }
                        }
                    }
                d++;
            } while (!stop && grid[bx, by] == BLANK);

            if (grid[bx, by] == BLANK) return false; //Путь не найден

            //Восстановление пути
            len = grid[bx, by];
            d = len;
            x = bx;
            y = by;

            while (d > 0)
            {
                px.Add(x);//px[d] = x;
                py.Add(y);//py[d] = y;
                d--;
                for (k = 0; k < 4; k++)
                {
                    int iy = y + dy[k], ix = x + dx[k];
                    if (iy >= 0 && iy < H && ix >= 0 && ix < W && grid[ix, iy] == d)
                    {
                        x = ix;
                        y = iy;
                        break;
                    }
                }
            }
            px.Add(ax);//px[0] = ax;
            py.Add(ay);//py[0] = ay;
            px.Reverse();
            py.Reverse();
            return true;
        }
    }
}

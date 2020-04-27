using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MySnakeAI
{
    public class GameProcess
    {
        public const int ElementSize = 30;
        public const int MapWidth = 20;
        public const int MapHeight = 20;
        public static int Score;
        public static int HumanRecord;
        public static int LeeRecord;
        public static int AIRecord;
        public static bool FruitWasEaten(Snake snake, Fruit fruit)
        {
            if (snake.Body[0].Location.X == fruit.Body.Location.X &&
                snake.Body[0].Location.Y == fruit.Body.Location.Y)
            {
                Score++;
                //Вычисляем направление хвоста
                int dirX; int dirY;
                if(Score > 2) { dirX = snake.LastdirX; dirY = snake.LastdirY; }
                else { dirX = snake.dirX; dirY = snake.dirY; }
                //Вычисляем направление хвоста
                snake.Body[Score] = new PictureBox();
                snake.Body[Score].Size = new Size(ElementSize-1, ElementSize-1);
                snake.Body[Score].Location = new Point(snake.Body[Score - 1].Location.X - ElementSize * dirX,
                    snake.Body[Score - 1].Location.Y - ElementSize * dirY);
                snake.Body[Score].BackColor = Color.Blue;
                return true;
            }
            return false;
        }

        //В этом блоке кода будет алгоритм Ли
        public static List<int> px = new List<int>(); //Координаты ячеек, входящих в путь
        public static List<int> py = new List<int>(); //Координаты ячеек, входящих в путь
        public static bool AlgorithmLee(Snake snake, int ax, int ay, int bx, int by)
        {
            int W = MapWidth; //Ширина карты
            int H = MapHeight; //Высота карты
            int WALL = -1; //Непроходимая ячейка
            int BLANK = -2; //Свободная непомеченная ячейка
            int len; //Длина пути
            int[,] grid = new int[W, H]; //Рабочее поле

            //Заполняем рабочее поле
            for (var _x = 0; _x < W; _x++)
                for (var _y = 0; _y < H; _y++)
                    for (var i = 0; i <= Score; i++)
                    {
                        if (_x * ElementSize == snake.Body[i].Location.X
                            && _y * ElementSize == snake.Body[i].Location.Y && i != 0) { grid[_x, _y] = WALL; break; }
                        else grid[_x, _y] = BLANK;
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
        //В этом блоке кода будет алгоритм Ли

        //В этом блоке кода будет алгоритм Q-Learning
        public static double GAMMA = 0.75; //0.75 влияние будущей награды
        public static double ALPHA = 0.01; // - скорость обучения
        public static double EPSILON = 0.9;
        public static double[] rewards = { 10, 1, -1, -100 }; //10 - съел фрукт, 1 - приблизился к фрукту, -1 - удалился от фрукта, -100 - умер
        public static State[] states = new State[144];
        public static State currentState;
        public static Dictionary<State, Action> Q = new Dictionary<State, Action>();
        public static void InitStates()
        {
            int j = 0;
            for (int i = 0; i < 8; i++)
            {
                states[i] = new State();
                states[i].fruitLoc[i] = 1;
            }
            j = 0;
            for (int i = 8; i < 16; i++)
            {
                states[i] = new State();
                states[i].obst[0] = 1;
                states[i].fruitLoc[j] = 1;
                j++;
            }
            j = 0;
            for (int i = 16; i < 24; i++)
            {
                states[i] = new State();
                states[i].obst[1] = 1;
                states[i].fruitLoc[j] = 1;
                j++;
            }
            j = 0;
            for (int i = 24; i < 32; i++)
            {
                states[i] = new State();
                states[i].obst[0] = 1;
                states[i].obst[1] = 1;
                states[i].fruitLoc[j] = 1;
                j++;
            }
            j = 0;
            for (int i = 32; i < 40; i++)
            {
                states[i] = new State();
                states[i].obst[2] = 1;
                states[i].fruitLoc[j] = 1;
                j++;
            }
            j = 0;
            for (int i = 40; i < 48; i++)
            {
                states[i] = new State();
                states[i].obst[0] = 1;
                states[i].obst[2] = 1;
                states[i].fruitLoc[j] = 1;
                j++;
            }
            j = 0;
            for (int i = 48; i < 56; i++)
            {
                states[i] = new State();
                states[i].obst[1] = 1;
                states[i].obst[2] = 1;
                states[i].fruitLoc[j] = 1;
                j++;
            }
            j = 0;
            for (int i = 56; i < 64; i++)
            {
                states[i] = new State();
                states[i].obst[0] = 1;
                states[i].obst[1] = 1;
                states[i].obst[2] = 1;
                states[i].fruitLoc[j] = 1;
                j++;
            }
            j = 0;
            for (int i = 64; i < 72; i++)
            {
                states[i] = new State();
                states[i].obst[3] = 1;
                states[i].fruitLoc[j] = 1;
                j++;
            }
            j = 0;
            for (int i = 72; i < 80; i++)
            {
                states[i] = new State();
                states[i].obst[0] = 1;
                states[i].obst[3] = 1;
                states[i].fruitLoc[j] = 1;
                j++;
            }
            j = 0;
            for (int i = 80; i < 88; i++)
            {
                states[i] = new State();
                states[i].obst[1] = 1;
                states[i].obst[3] = 1;
                states[i].fruitLoc[j] = 1;
                j++;
            }
            j = 0;
            for (int i = 88; i < 96; i++)
            {
                states[i] = new State();
                states[i].obst[0] = 1;
                states[i].obst[1] = 1;
                states[i].obst[3] = 1;
                states[i].fruitLoc[j] = 1;
                j++;
            }
            j = 0;
            for (int i = 96; i < 104; i++)
            {
                states[i] = new State();
                states[i].obst[2] = 1;
                states[i].obst[3] = 1;
                states[i].fruitLoc[j] = 1;
                j++;
            }
            j = 0;
            for (int i = 104; i < 112; i++)
            {
                states[i] = new State();
                states[i].obst[0] = 1;
                states[i].obst[2] = 1;
                states[i].obst[3] = 1;
                states[i].fruitLoc[j] = 1;
                j++;
            }
            j = 0;
            for (int i = 112; i < 120; i++)
            {
                states[i] = new State();
                states[i].obst[1] = 1;
                states[i].obst[2] = 1;
                states[i].obst[3] = 1;
                states[i].fruitLoc[j] = 1;
                j++;
            }
            j = 0;
            for (int i = 120; i < 128; i++)
            {
                states[i] = new State();
                states[i].obst[0] = 1;
                states[i].obst[1] = 1;
                states[i].obst[2] = 1;
                states[i].obst[3] = 1;
                states[i].fruitLoc[j] = 1;
                j++;
            }
            states[128] = new State(); //Частный случай

            states[129] = new State();
            states[129].obst[0] = 1;

            states[130] = new State();
            states[130].obst[1] = 1;

            states[131] = new State();
            states[131].obst[0] = 1;
            states[131].obst[1] = 1;

            states[132] = new State();
            states[132].obst[2] = 1;

            states[133] = new State();
            states[133].obst[0] = 1;
            states[133].obst[2] = 1;

            states[134] = new State();
            states[134].obst[1] = 1;
            states[134].obst[2] = 1;

            states[135] = new State();
            states[135].obst[0] = 1;
            states[135].obst[1] = 1;
            states[135].obst[2] = 1;

            states[136] = new State();
            states[136].obst[3] = 1;

            states[137] = new State();
            states[137].obst[0] = 1;
            states[137].obst[3] = 1;

            states[138] = new State();
            states[138].obst[1] = 1;
            states[138].obst[3] = 1;

            states[139] = new State();
            states[139].obst[0] = 1;
            states[139].obst[1] = 1;
            states[139].obst[3] = 1;

            states[140] = new State();
            states[140].obst[2] = 1;
            states[140].obst[3] = 1;

            states[141] = new State();
            states[141].obst[0] = 1;
            states[141].obst[2] = 1;
            states[141].obst[3] = 1;

            states[142] = new State();
            states[142].obst[1] = 1;
            states[142].obst[2] = 1;
            states[142].obst[3] = 1;

            states[143] = new State();
            states[143].obst[0] = 1;
            states[143].obst[1] = 1;
            states[143].obst[2] = 1;
            states[143].obst[3] = 1;
        }
        public static void InitQ()
        {
            for (int i = 0; i < 144; i++)
            {
                Q.Add(states[i], new Action());
            }
        }
        public static void Step(Snake snake)
        {
            currentState = new State();
            currentState.obst[0] = SnakeWindow.up;
            currentState.obst[1] = SnakeWindow.right;
            currentState.obst[2] = SnakeWindow.down;
            currentState.obst[3] = SnakeWindow.left;
            currentState.fruitLoc[0] = SnakeWindow.ul;
            currentState.fruitLoc[1] = SnakeWindow.u;
            currentState.fruitLoc[2] = SnakeWindow.ur;
            currentState.fruitLoc[3] = SnakeWindow.r;
            currentState.fruitLoc[4] = SnakeWindow.dr;
            currentState.fruitLoc[5] = SnakeWindow.d;
            currentState.fruitLoc[6] = SnakeWindow.dl;
            currentState.fruitLoc[7] = SnakeWindow.l;

            //Какому состоянию из словаря соответствует currentState 
            bool obstTrue = true;
            bool fruitTrue = true;
            for (int i = 0; i < 129; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (states[i].obst[j] != currentState.obst[j]) { obstTrue = false; break; }
                }
                for (int t = 0; t < 8; t++)
                {
                    if (states[i].fruitLoc[t] != currentState.fruitLoc[t]) { fruitTrue = false; break; }
                }
                if (obstTrue && fruitTrue) { currentState = states[i]; break; }
                obstTrue = true;
                fruitTrue = true;
            }

            var r = AgentDecision();
            if (r == 0) //Up
            { 
                snake.dirX = 0;
                if (snake.dirY != 1)
                    snake.dirY = -1;
            } 
            if (r == 1) //Right
            { 
                snake.dirY = 0;
                if (snake.dirX != -1)
                    snake.dirX = 1;
            } 
            if (r == 2) //Down
            { 
                snake.dirX = 0;
                if (snake.dirY != -1)
                    snake.dirY = 1;
            } //Left
            if (r == 3) 
            { 
                snake.dirY = 0;
                if (snake.dirX != 1)
                    snake.dirX = -1;
            } 
        }
        public static int AgentDecision()
        {
            var rnd = new Random();
            var check = (double)rnd.Next(0, 100) / 100;
            var r = rnd.Next(0, 4);
            if(check<EPSILON)
            {
                return r; //RandomAction();
            }
            else
            {
                return BestAction();
            }
        }
        public static int BestAction()
        {
            Action act = Q[currentState];
            double max = act.actions[0];
            int r = 0;
            for (int i = 0; i < 4; i++)
            {
                if (act.actions[i] > max) { max = act.actions[i]; r = i; }
            }
            return r;
        }
        public static void Update(Snake snake, double reward)
        {
            int index = 0;
            if (snake.dirX == 0 && snake.dirY == -1) index = 0; //Up
            if (snake.dirX == 1 && snake.dirY == 0) index = 1; //Right
            if (snake.dirX == 0 && snake.dirY == 1) index = 2; //Down
            if (snake.dirX == -1 && snake.dirY == 0) index = 3; //Left
            double currScore = Q[currentState].actions[index];

            currScore = currScore + ALPHA * (reward + GAMMA * Q[currentState].actions[BestAction()] - currScore);
            Q[currentState].actions[index] = currScore;
        }
        public static double GetReward(double distanceAfter, double distanceBefore)
        {
            var d2 = distanceAfter;
            var d1 = distanceBefore;
            if (d2 > d1) return rewards[2];
            else return rewards[1];
        }
        //В этом блоке кода будет алгоритм Q-Learning
    }
}

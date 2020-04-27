using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MySnakeAI
{
    public partial class SnakeWindow : Form
    {
        #region
        private const int ElementSize = GameProcess.ElementSize;
        private const int MapWidth = GameProcess.MapWidth;
        private const int MapHeight = GameProcess.MapHeight;

        public static int up;
        public static int right;
        public static int down;
        public static int left;

        public static int ul;
        public static int u;
        public static int ur;
        public static int r;
        public static int dr;
        public static int d;
        public static int dl;
        public static int l;

        private GameProcess gameProcess;
        private Fruit fruit;
        private Snake snake;
        private PictureBox borderLine;
        private Label labelScore;
        private Label labelRecord;
        private Label labelHuman;
        private Label labelLee;
        private Label labelAI;
        private Label Epsilon;
        private Label Generation;
        private static int gen = 1;
        private Timer timer;
        private int time_delay = 50;
        private int who = 2; //1-human; 2-lee; 3-AI
        private PictureBox[] call; //с какой стороны находится препятсвие
        private PictureBox[] ball; //положение фрукта относительно головы змейки
                                   //private Dictionary<string, Bitmap> images = new Dictionary<string, Bitmap>();
                                   //private int tickCount;
        #endregion //Переменные 
        public SnakeWindow()
        {
            GameProcess.InitStates();
            GameProcess.InitQ();
            InitializeWindow();
            gameProcess = new GameProcess();
            fruit = new Fruit();
            snake = new Snake();
            this.Controls.Add(fruit.Body);
            this.Controls.Add(snake.Body[0]);
            /*
            var imagesDirectory = new DirectoryInfo("Images");
            foreach (var file in imagesDirectory.GetFiles("*.png"))
                images[file.Name] = (Bitmap)Image.FromFile(file.FullName);
                */

            call = new PictureBox[4];
            for (int i = 0; i < 4; i++)
            {
                call[i] = new PictureBox();
                call[i].BackColor = Color.Black;
                call[i].Size = new Size(ElementSize, ElementSize);
            }
            call[0].Location = new Point(MapWidth * ElementSize + 4 * ElementSize, 6 * ElementSize);
            call[1].Location = new Point(MapWidth * ElementSize + 5 * ElementSize, 7 * ElementSize);
            call[2].Location = new Point(MapWidth * ElementSize + 4 * ElementSize, 8 * ElementSize);
            call[3].Location = new Point(MapWidth * ElementSize + 3 * ElementSize, 7 * ElementSize);
            for (int i = 0; i < 4; i++)
            {
                this.Controls.Add(call[i]);
            }

            ball = new PictureBox[8];
            for (int i = 0; i < 8; i++)
            {
                ball[i] = new PictureBox();
                ball[i].BackColor = Color.Black;
                ball[i].Size = new Size(ElementSize, ElementSize);
            }
            ball[0].Location = new Point(MapWidth * ElementSize + 3 * ElementSize, 10 * ElementSize);
            ball[1].Location = new Point(MapWidth * ElementSize + 4 * ElementSize, 10 * ElementSize);
            ball[2].Location = new Point(MapWidth * ElementSize + 5 * ElementSize, 10 * ElementSize);
            ball[3].Location = new Point(MapWidth * ElementSize + 5 * ElementSize, 11 * ElementSize);
            ball[4].Location = new Point(MapWidth * ElementSize + 5 * ElementSize, 12 * ElementSize);
            ball[5].Location = new Point(MapWidth * ElementSize + 4 * ElementSize, 12 * ElementSize);
            ball[6].Location = new Point(MapWidth * ElementSize + 3 * ElementSize, 12 * ElementSize);
            ball[7].Location = new Point(MapWidth * ElementSize + 3 * ElementSize, 11 * ElementSize);
            for (int i = 0; i < 8; i++)
            {
                this.Controls.Add(ball[i]);
            }

            timer = new Timer();
            timer.Interval = time_delay;
            timer.Tick += TimerTick;
            timer.Start();
        }

        private void GameRestart()
        {
            for(int i = 0; i <= GameProcess.Score; i++)
            {
                this.Controls.Remove(snake.Body[i]);
            }
            this.Controls.Remove(fruit.Body);
            fruit = new Fruit();
            snake = new Snake();
            this.Controls.Add(fruit.Body);
            this.Controls.Add(snake.Body[0]);
            GameProcess.Score = 0;
        }
        private void InitializeWindow()
        {
            labelScore = new Label();
            labelScore.Size = new Size(500, ElementSize);
            labelScore.Location = new Point(MapWidth * ElementSize + ElementSize, 0);
            labelScore.Text = "Max score: 900   Current score: " + GameProcess.Score.ToString();
            labelScore.Font = new Font("Arial", 16);
            this.Controls.Add(labelScore);

            labelRecord = new Label();
            labelRecord.Size = new Size(200, ElementSize);
            labelRecord.Location = new Point(MapWidth * ElementSize + ElementSize, 2*ElementSize);
            labelRecord.Text = "Records:";
            labelRecord.Font = new Font("Arial", 16);
            this.Controls.Add(labelRecord);

            string recHumanText = File.ReadAllText("Records\\Human.txt");
            GameProcess.HumanRecord = int.Parse(recHumanText);
            labelHuman = new Label();
            labelHuman.Size = new Size(200, ElementSize);
            labelHuman.Location = new Point(MapWidth * ElementSize + 2*ElementSize, 3*ElementSize);
            labelHuman.Text = "Human: " + GameProcess.HumanRecord;
            labelHuman.Font = new Font("Arial", 16);
            this.Controls.Add(labelHuman);

            string recLeeText = File.ReadAllText("Records\\AlgorithmLee.txt");
            GameProcess.LeeRecord = int.Parse(recLeeText);
            labelLee = new Label();
            labelLee.Size = new Size(200, ElementSize);
            labelLee.Location = new Point(MapWidth * ElementSize + 2 * ElementSize, 4 * ElementSize);
            labelLee.Text = "Algorithm Lee: " + GameProcess.LeeRecord;
            labelLee.Font = new Font("Arial", 16);
            this.Controls.Add(labelLee);

            string recAIText = File.ReadAllText("Records\\AI.txt");
            GameProcess.AIRecord = int.Parse(recAIText);
            labelAI = new Label();
            labelAI.Size = new Size(200, ElementSize);
            labelAI.Location = new Point(MapWidth * ElementSize + 2 * ElementSize, 5 * ElementSize);
            labelAI.Text = "AI: " + GameProcess.AIRecord;
            labelAI.Font = new Font("Arial", 16);
            this.Controls.Add(labelAI);

            
            Epsilon = new Label();
            Epsilon.Size = new Size(200, ElementSize);
            Epsilon.Location = new Point(MapWidth * ElementSize + 3 * ElementSize, 14 * ElementSize);
            Epsilon.Text = "Epsilon: " + GameProcess.EPSILON;
            Epsilon.Font = new Font("Arial", 16);
            this.Controls.Add(Epsilon);

            Generation = new Label();
            Generation.Size = new Size(200, ElementSize);
            Generation.Location = new Point(MapWidth * ElementSize + 3 * ElementSize, 15 * ElementSize);
            Generation.Text = "Generation: " + gen;
            Generation.Font = new Font("Arial", 16);
            this.Controls.Add(Generation);

            borderLine = new PictureBox();
            borderLine.Size = new Size(1, MapHeight * ElementSize);
            borderLine.BackColor = Color.Black;
            borderLine.Location = new Point(MapWidth * ElementSize, 0);
            this.Controls.Add(borderLine);

            this.Text = "SnakeAI";
            ClientSize = new Size(MapWidth * ElementSize + 600,
                MapHeight * ElementSize);
            FormBorderStyle = FormBorderStyle.FixedDialog;

        }
        private void ActionWithKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode.ToString())
            {
                case "Right":
                    if (snake.dirX != -1)
                        snake.dirX = 1;
                    snake.dirY = 0;
                    break;
                case "Left":
                    if (snake.dirX != 1)
                        snake.dirX = -1;
                    snake.dirY = 0;
                    break;
                case "Up":
                    snake.dirX = 0;
                    if (snake.dirY != 1)
                        snake.dirY = -1;
                    break;
                case "Down":
                    snake.dirX = 0;
                    if (snake.dirY != -1)
                        snake.dirY = 1;
                    break;
            }
        }
        private void TimerTick(object sender, EventArgs args)
        {
            ul = 0;
            u = 0;
            ur = 0;
            r = 0;
            dr = 0;
            d = 0;
            dl = 0;
            l = 0;

            up = 0;
            right = 0;
            down = 0;
            left = 0;

            if (snake.Body[0].Location.X > fruit.Body.Location.X && snake.Body[0].Location.Y > fruit.Body.Location.Y) ul = 1;
            else ul = 0;
            if (snake.Body[0].Location.X == fruit.Body.Location.X && snake.Body[0].Location.Y > fruit.Body.Location.Y) u = 1;
            else u = 0;
            if (snake.Body[0].Location.X < fruit.Body.Location.X && snake.Body[0].Location.Y > fruit.Body.Location.Y) ur = 1;
            else ur = 0;
            if (snake.Body[0].Location.X < fruit.Body.Location.X && snake.Body[0].Location.Y == fruit.Body.Location.Y) r = 1;
            else r = 0;
            if (snake.Body[0].Location.X < fruit.Body.Location.X && snake.Body[0].Location.Y < fruit.Body.Location.Y) dr = 1;
            else dr = 0;
            if (snake.Body[0].Location.X == fruit.Body.Location.X && snake.Body[0].Location.Y < fruit.Body.Location.Y) d = 1;
            else d = 0;
            if (snake.Body[0].Location.X > fruit.Body.Location.X && snake.Body[0].Location.Y < fruit.Body.Location.Y) dl = 1;
            else dl = 0;
            if (snake.Body[0].Location.X > fruit.Body.Location.X && snake.Body[0].Location.Y == fruit.Body.Location.Y) l = 1;
            else l = 0;

            if (snake.Body[0].Location.Y == 0) up = 1;
            else up = 0;
            if (snake.Body[0].Location.Y == (MapHeight - 1) * ElementSize) down = 1;
            else down = 0;
            if (snake.Body[0].Location.X == 0) left = 1;
            else left = 0;
            if (snake.Body[0].Location.X == (MapWidth - 1) * ElementSize) right = 1;
            else right = 0;

            for (int i = 1; i <= GameProcess.Score; i++)
            {
                int deltaY = snake.Body[i].Location.Y - snake.Body[0].Location.Y;
                int deltaX = snake.Body[i].Location.X - snake.Body[0].Location.X;
                if (up == 0)
                {
                    if (deltaY == -ElementSize && deltaX == 0) up = 1;
                    else up = 0;
                }
                if (down == 0)
                {
                    if (deltaY == ElementSize && deltaX == 0) down = 1;
                    else down = 0;
                }
                if (left == 0)
                {
                    if (deltaX == -ElementSize && deltaY == 0) left = 1;
                    else left = 0;
                }
                if (right == 0)
                {
                    if (deltaX == ElementSize && deltaY == 0) right = 1;
                    else right = 0;
                }
            }

            if (up == 1) call[0].BackColor = Color.Gray;
            else call[0].BackColor = Color.Black;
            if (right == 1) call[1].BackColor = Color.Gray;
            else call[1].BackColor = Color.Black;
            if (down == 1) call[2].BackColor = Color.Gray;
            else call[2].BackColor = Color.Black;
            if (left == 1) call[3].BackColor = Color.Gray;
            else call[3].BackColor = Color.Black;

            if (ul == 1) ball[0].BackColor = Color.Gray;
            else ball[0].BackColor = Color.Black;
            if (u == 1) ball[1].BackColor = Color.Gray;
            else ball[1].BackColor = Color.Black;
            if (ur == 1) ball[2].BackColor = Color.Gray;
            else ball[2].BackColor = Color.Black;
            if (r == 1) ball[3].BackColor = Color.Gray;
            else ball[3].BackColor = Color.Black;
            if (dr == 1) ball[4].BackColor = Color.Gray;
            else ball[4].BackColor = Color.Black;
            if (d == 1) ball[5].BackColor = Color.Gray;
            else ball[5].BackColor = Color.Black;
            if (dl == 1) ball[6].BackColor = Color.Gray;
            else ball[6].BackColor = Color.Black;
            if (l == 1) ball[7].BackColor = Color.Gray;
            else ball[7].BackColor = Color.Black;

            if (who == 1) PlayHuman();
            else if (who == 2) PlayLee();
            else if (who == 3) PlayAI();
            //if (GameProcess.EPSILON > 0.005)
            //    PlayAI();
            //else
            //{
            //    if (GameProcess.AlgorithmLee(snake, snake.Body[0].Location.X / ElementSize, snake.Body[0].Location.Y / ElementSize,
            //                                    fruit.Body.Location.X / ElementSize, fruit.Body.Location.Y / ElementSize))
            //    {
            //        PlayLee();
            //    }
            //    else
            //        PlayAI();
            //}
            //Invalidate();
        }
        private void PlayHuman()
        {
            
            this.KeyDown += ActionWithKeyDown;
            if (snake.IsDead)
            {
                if (GameProcess.Score > GameProcess.HumanRecord)
                {
                    GameProcess.HumanRecord = GameProcess.Score;
                    File.WriteAllText("Records\\Human.txt", GameProcess.HumanRecord.ToString());
                    labelHuman.Text = "Human: " + GameProcess.HumanRecord;
                }
                GameRestart();
            }
            else
            {
                snake.Move();
                if (GameProcess.FruitWasEaten(snake, fruit))
                {
                    fruit.Move(snake);
                    this.Controls.Add(snake.Body[GameProcess.Score]);
                }
                labelScore.Text = "Max score: 900   Current score: " + GameProcess.Score.ToString();
            }
        }
        private void PlayLee()
        {
            if (snake.IsDead)
            {
                if (GameProcess.Score > GameProcess.LeeRecord)
                {
                    GameProcess.LeeRecord = GameProcess.Score;
                    File.WriteAllText("Records\\AlgorithmLee.txt", GameProcess.LeeRecord.ToString());
                    labelLee.Text = "Algorithm Lee: " + GameProcess.LeeRecord;
                }
                GameRestart();
            }
            else
            {
                var canMove = GameProcess.AlgorithmLee(snake, snake.Body[0].Location.X / ElementSize, snake.Body[0].Location.Y / ElementSize,
                                                fruit.Body.Location.X / ElementSize, fruit.Body.Location.Y / ElementSize);
                if (canMove)
                {
                    snake.dirX = GameProcess.px[1] - GameProcess.px[0];
                    snake.dirY = GameProcess.py[1] - GameProcess.py[0];
                }
                snake.Move();
                if (GameProcess.FruitWasEaten(snake, fruit))
                {
                    fruit.Move(snake);
                    this.Controls.Add(snake.Body[GameProcess.Score]);
                }
                labelScore.Text = "Max score: 900   Current score: " + GameProcess.Score.ToString();
            }
        }
        private void PlayAI()
        {
            if (snake.IsDead)
            {
                if (GameProcess.Score > GameProcess.AIRecord)
                {
                    GameProcess.AIRecord = GameProcess.Score;
                    File.WriteAllText("Records\\AI.txt", GameProcess.AIRecord.ToString());
                    labelAI.Text = "AI: " + GameProcess.AIRecord;
                }
                gen++;
                
                if (GameProcess.EPSILON > 0.005) GameProcess.EPSILON -= 0.005;
                GameRestart();
            }
            double reward = 0;
            var distBefore = Math.Sqrt(Math.Pow(snake.Body[0].Location.X - fruit.Body.Location.X, 2) + 
                Math.Pow(snake.Body[0].Location.Y - fruit.Body.Location.Y, 2));

            GameProcess.Step(snake);
            snake.Move();

            var distAfter = Math.Sqrt(Math.Pow(snake.Body[0].Location.X - fruit.Body.Location.X, 2) +
                Math.Pow(snake.Body[0].Location.Y - fruit.Body.Location.Y, 2));

            if (snake.IsDead) reward = GameProcess.rewards[3];
            else if (GameProcess.FruitWasEaten(snake, fruit))
            {
                reward = GameProcess.rewards[0];
                fruit.Move(snake);
                this.Controls.Add(snake.Body[GameProcess.Score]);
            }
            else reward = GameProcess.GetReward(distAfter, distBefore);
            labelScore.Text = "Max score: 900   Current score: " + GameProcess.Score.ToString();

            GameProcess.Update(snake, reward);
            Epsilon.Text = "Epsilon: " + GameProcess.EPSILON;
            Generation.Text = "Generation: " + gen;
        }
    }
}

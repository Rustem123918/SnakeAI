using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MySnakeAI
{
    public partial class SnakeWindow : Form
    {
        private int MapWidth = 20;
        private int MapHeight = 20;
        private int ElementSize = 30;

        public int Who = 4; //1 - человек //2 - Ли //3 - QLearning //4 - NeuralNet
        public bool GameOver;
        public Map map;
        public Snake snake;
        public Fruit fruit;
        public AlgorithmLee alg_lee;
        public QLearning q_learn;
        public NeuralNetwork.NeuralNetwork neuralNetwork;

        public int Score;
        public int MaxScore;
        public int HumanRecord;
        public int LeeRecord;
        public int QRecord;

        private PictureBox borderLine;
        private Label labelScore;
        private Label labelRecord;
        private Label labelHuman;
        private Label labelLee;
        private Label labelQ;
        private Label Epsilon;
        //private Label Generation;
        private PictureBox[] call; //с какой стороны находится препятсвие
        private PictureBox[] ball; //положение фрукта относительно головы змейки

        private Timer timer;
        private int time_delay = 50;
        //private Dictionary<string, Bitmap> images = new Dictionary<string, Bitmap>();
        //private int tickCount;
        public SnakeWindow()
        {
            GameOver = false;
            map = new Map(MapWidth, MapHeight, ElementSize);
            snake = new Snake(map);
            fruit = new Fruit(snake, map);

            if (Who == 2)
                alg_lee = new AlgorithmLee();
            else if (Who == 3)
                q_learn = new QLearning();
            else if (Who == 4)
            {
                q_learn = new QLearning();
                var topology = new NeuralNetwork.Topology(12, 4, 0.1, 6);
                neuralNetwork = new NeuralNetwork.NeuralNetwork(topology);
                var dataSet = new List<Tuple<List<double>, List<double>>>();

                var strs = File.ReadLines("Records\\DataSet2.txt").ToList();
                char[] param = { '{', ' ', ',', '}' };
                foreach (var s in strs)
                {
                    var list = new List<double>();
                    var inputs = new List<double>();
                    var outputs = new List<double>();
                    foreach (var c in s.Split(param))
                    {
                        if(c.Length == 1)
                            list.Add(int.Parse(c));
                    }
                    inputs = list.GetRange(0, 12);
                    outputs = list.GetRange(12, 4);
                    var tup = new Tuple<List<double>, List<double>>(inputs, outputs);
                    dataSet.Add(tup);
                }

                neuralNetwork.Learn(dataSet, 10000);
            }

            InitializeWindow();

            timer = new Timer();
            timer.Interval = time_delay;
            timer.Tick += TimerTick;
            timer.Start();
        }
        private void InitializeWindow()
        {
            labelScore = new Label();
            labelScore.Size = new Size(500, ElementSize);
            labelScore.Location = new Point(MapWidth * ElementSize + ElementSize, 0);
            labelScore.Text = "Max score: 900   Current score: " + Score.ToString();
            labelScore.Font = new Font("Arial", 16);
            this.Controls.Add(labelScore);

            labelRecord = new Label();
            labelRecord.Size = new Size(200, ElementSize);
            labelRecord.Location = new Point(MapWidth * ElementSize + ElementSize, 2*ElementSize);
            labelRecord.Text = "Records:";
            labelRecord.Font = new Font("Arial", 16);
            this.Controls.Add(labelRecord);

            string recHumanText = File.ReadAllText("Records\\Human.txt");
            HumanRecord = int.Parse(recHumanText);
            labelHuman = new Label();
            labelHuman.Size = new Size(200, ElementSize);
            labelHuman.Location = new Point(MapWidth * ElementSize + 2*ElementSize, 3*ElementSize);
            labelHuman.Text = "Human: " + HumanRecord;
            labelHuman.Font = new Font("Arial", 16);
            this.Controls.Add(labelHuman);

            string recLeeText = File.ReadAllText("Records\\AlgorithmLee.txt");
            LeeRecord = int.Parse(recLeeText);
            labelLee = new Label();
            labelLee.Size = new Size(200, ElementSize);
            labelLee.Location = new Point(MapWidth * ElementSize + 2 * ElementSize, 4 * ElementSize);
            labelLee.Text = "Algorithm Lee: " + LeeRecord;
            labelLee.Font = new Font("Arial", 16);
            this.Controls.Add(labelLee);

            string recQText = File.ReadAllText("Records\\Q.txt");
            QRecord = int.Parse(recQText);
            labelQ = new Label();
            labelQ.Size = new Size(200, ElementSize);
            labelQ.Location = new Point(MapWidth * ElementSize + 2 * ElementSize, 5 * ElementSize);
            labelQ.Text = "Q: " + QRecord;
            labelQ.Font = new Font("Arial", 16);
            this.Controls.Add(labelQ);

            Epsilon = new Label();
            Epsilon.Size = new Size(200, ElementSize);
            Epsilon.Location = new Point(MapWidth * ElementSize + 3 * ElementSize, 14 * ElementSize);
            Epsilon.Text = "Eps: " + q_learn.EPSILON;
            Epsilon.Font = new Font("Arial", 16);
            this.Controls.Add(Epsilon);

            //Generation = new Label();
            //Generation.Size = new Size(200, ElementSize);
            //Generation.Location = new Point(MapWidth * ElementSize + 3 * ElementSize, 15 * ElementSize);
            //Generation.Text = "Generation: " + q_learn.gen;
            //Generation.Font = new Font("Arial", 16);
            //this.Controls.Add(Generation);

            borderLine = new PictureBox();
            borderLine.Size = new Size(1, MapHeight * ElementSize);
            borderLine.BackColor = Color.Black;
            borderLine.Location = new Point(MapWidth * ElementSize, 0);
            this.Controls.Add(borderLine);

            this.Text = "SnakeAI";
            ClientSize = new Size(MapWidth * ElementSize + 600,
                MapHeight * ElementSize);
            FormBorderStyle = FormBorderStyle.FixedDialog;


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

        }
        private void TimerTick(object sender, EventArgs args)
        {
            switch(Who)
            {
                case 1:
                    PlayHuman();
                    break;
                case 2:
                    PlayLee();
                    break;
                case 3:
                    PlayQ();
                    break;
                case 4:
                    PlayNeuralNet();
                    break;
            }

            if (GameOver)
            {
                SaveRecords();
                GameRestart();
            }

            //Invalidate();
        }
        private void ActionWithKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode.ToString())
            {
                case "D":
                    if (snake.Dir != SnakeDirection.LEFT)
                        snake.Dir = SnakeDirection.RIGHT;
                    break;
                case "A":
                    if (snake.Dir != SnakeDirection.RIGHT)
                        snake.Dir = SnakeDirection.LEFT;
                    break;
                case "W":
                    if (snake.Dir != SnakeDirection.DOWN)
                        snake.Dir = SnakeDirection.UP;
                    break;
                case "S":
                    if (snake.Dir != SnakeDirection.UP)
                        snake.Dir = SnakeDirection.DOWN;
                    break;
            }
        }
        private bool CheckGameOver() //true - выход за границы, false - не вышел за границы
        {
            if (snake.X < 0 || snake.X >= MapWidth ||
                snake.Y < 0 || snake.Y >= MapHeight)
                return true;

            for (int i = 1; i < snake.Body.Count; i++)
            {
                if (snake.Body[0].Location == snake.Body[i].Location)
                    return true;
            }

            return false;
        }
        private bool CheckEatFruit()
        {
            if (snake.Body[0].Location == fruit.Body.Location)
            {
                Score++;
                var positionLastElement = snake.Body.Last().Location;
                snake.Body.Add(new PictureBox());
                snake.Body.Last().Size = new Size(ElementSize - 1, ElementSize - 1);
                snake.Body.Last().Location = positionLastElement;
                snake.Body[Score].BackColor = Color.Blue;
                fruit.Spawn(snake, map);
                this.Controls.Add(snake.Body.Last());
                return true;
            }
            return false;
        }
        private void CheckState()
        {
            q_learn.up = 0;
            q_learn.down = 0;
            q_learn.right = 0;
            q_learn.left = 0;
            q_learn.ul = 0;
            q_learn.u = 0;
            q_learn.ur = 0;
            q_learn.r = 0;
            q_learn.dr = 0;
            q_learn.d = 0;
            q_learn.dl = 0;
            q_learn.l = 0;

            if (snake.Y == 0) q_learn.up = 1;
            else q_learn.up = 0;
            if (snake.Y == MapHeight - 1) q_learn.down = 1;
            else q_learn.down = 0;
            if (snake.X == 1) q_learn.left = 1;
            else q_learn.left = 0;
            if (snake.X == MapWidth - 1) q_learn.right = 1;
            else q_learn.right = 0;

            for (int i = 1; i < snake.Body.Count; i++)
            {
                int deltaX = snake.Body[i].Location.X - snake.Body[0].Location.X;
                int deltaY = snake.Body[i].Location.Y - snake.Body[0].Location.Y;
                if (q_learn.up == 0)
                {
                    if (deltaX == 0 && deltaY == -ElementSize) q_learn.up = 1;
                    else q_learn.up = 0;
                }
                if (q_learn.right == 0)
                {
                    if (deltaX == ElementSize && deltaY == 0) q_learn.right = 1;
                    else q_learn.right = 0;
                }
                if (q_learn.down == 0)
                {
                    if (deltaX == 0 && deltaY == ElementSize) q_learn.down = 1;
                    else q_learn.down = 0;
                }
                if (q_learn.left == 0)
                {
                    if (deltaX == -ElementSize && deltaY == 0) q_learn.left = 1;
                    else q_learn.left = 0;
                }
            }

            if (snake.X > fruit.X && snake.Y > fruit.Y) q_learn.ul = 1;
            else q_learn.ul = 0;
            if (snake.X == fruit.X && snake.Y > fruit.Y) q_learn.u = 1;
            else q_learn.u = 0;
            if (snake.X < fruit.X && snake.Y > fruit.Y) q_learn.ur = 1;
            else q_learn.ur = 0;
            if (snake.X < fruit.X && snake.Y == fruit.Y) q_learn.r = 1;
            else q_learn.r = 0;
            if (snake.X < fruit.X && snake.Y < fruit.Y) q_learn.dr = 1;
            else q_learn.dr = 0;
            if (snake.X == fruit.X && snake.Y < fruit.Y) q_learn.d = 1;
            else q_learn.d = 0;
            if (snake.X > fruit.X && snake.Y < fruit.Y) q_learn.dl = 1;
            else q_learn.dl = 0;
            if (snake.X > fruit.X && snake.Y == fruit.Y) q_learn.l = 1;
            else q_learn.l = 0;

            if (q_learn.up == 1) call[0].BackColor = Color.Gray;
            else call[0].BackColor = Color.Black;
            if (q_learn.right == 1) call[1].BackColor = Color.Gray;
            else call[1].BackColor = Color.Black;
            if (q_learn.down == 1) call[2].BackColor = Color.Gray;
            else call[2].BackColor = Color.Black;
            if (q_learn.left == 1) call[3].BackColor = Color.Gray;
            else call[3].BackColor = Color.Black;

            if (q_learn.ul == 1) ball[0].BackColor = Color.Gray;
            else ball[0].BackColor = Color.Black;
            if (q_learn.u == 1) ball[1].BackColor = Color.Gray;
            else ball[1].BackColor = Color.Black;
            if (q_learn.ur == 1) ball[2].BackColor = Color.Gray;
            else ball[2].BackColor = Color.Black;
            if (q_learn.r == 1) ball[3].BackColor = Color.Gray;
            else ball[3].BackColor = Color.Black;
            if (q_learn.dr == 1) ball[4].BackColor = Color.Gray;
            else ball[4].BackColor = Color.Black;
            if (q_learn.d == 1) ball[5].BackColor = Color.Gray;
            else ball[5].BackColor = Color.Black;
            if (q_learn.dl == 1) ball[6].BackColor = Color.Gray;
            else ball[6].BackColor = Color.Black;
            if (q_learn.l == 1) ball[7].BackColor = Color.Gray;
            else ball[7].BackColor = Color.Black;
        }
        private void PlayHuman()
        {
            this.KeyDown += ActionWithKeyDown;
            snake.Move(map);
            GameOver = CheckGameOver();
            CheckEatFruit();
            labelScore.Text = "Max score: 900   Current score: " + Score.ToString();
        }
        private void PlayLee()
        {
            var canMove = alg_lee.Search(map, snake, snake.Body[0].Location.X / ElementSize, snake.Body[0].Location.Y / ElementSize,
                                                fruit.Body.Location.X / ElementSize, fruit.Body.Location.Y / ElementSize);
            if (canMove)
            {
                int dirX = alg_lee.px[1] - alg_lee.px[0];
                int dirY = alg_lee.py[1] - alg_lee.py[0];
                switch (dirX)
                {
                    case 1:
                        snake.Dir = SnakeDirection.RIGHT;
                        break;
                    case -1:
                        snake.Dir = SnakeDirection.LEFT;
                        break;
                }
                switch (dirY)
                {
                    case 1:
                        snake.Dir = SnakeDirection.DOWN;
                        break;
                    case -1:
                        snake.Dir = SnakeDirection.UP;
                        break;
                }
            }
            snake.Move(map);
            GameOver = CheckGameOver();
            CheckEatFruit();
            labelScore.Text = "Max score: 900   Current score: " + Score.ToString();
        }
        private void PlayQ()
        {
            CheckState();

            double reward = 0;
            var distBefore = Math.Sqrt(Math.Pow(snake.Body[0].Location.X - fruit.Body.Location.X, 2) +
                Math.Pow(snake.Body[0].Location.Y - fruit.Body.Location.Y, 2));

            q_learn.Step(snake);
            snake.Move(map);
            GameOver = CheckGameOver();
            bool eatFruit = CheckEatFruit();

            var distAfter = Math.Sqrt(Math.Pow(snake.Body[0].Location.X - fruit.Body.Location.X, 2) +
                Math.Pow(snake.Body[0].Location.Y - fruit.Body.Location.Y, 2));

            if (GameOver) 
                reward = q_learn.rewards[3];
            else if (eatFruit)
                reward = q_learn.rewards[0];
            else 
                reward = q_learn.GetReward(distAfter, distBefore);

            labelScore.Text = "Max score: 900   Current score: " + Score.ToString();

            q_learn.Update(snake, reward);
            Epsilon.Text = "Eps: " + Math.Round(q_learn.EPSILON, 3);
            //Generation.Text = "Generation: " + gen;
        }
        private void PlayNeuralNet()
        {
            CheckState();
            q_learn.InstallCurrentState();
            var inputSignals = new List<double>();
            for(int i = 0; i < 4; i++)
            {
                inputSignals.Add(q_learn.currentState.obst[i]);
            }
            for (int i = 0; i < 8; i++)
            {
                inputSignals.Add(q_learn.currentState.fruitLoc[i]);
            }
            var neurons = neuralNetwork.FeedForward(inputSignals);
            int index = 0;
            int k = 0;
            var max = neurons[0].Output;
            foreach(var n in neurons)
            {
                if (n.Output > max)
                {
                    max = n.Output;
                    index = k;
                }
                k++;
            }
            switch (index)
            {
                case 0:
                    if (snake.Dir != SnakeDirection.DOWN)
                        snake.Dir = SnakeDirection.UP;
                    break;
                case 1:
                    if (snake.Dir != SnakeDirection.LEFT)
                        snake.Dir = SnakeDirection.RIGHT;
                    break;
                case 2:
                    if (snake.Dir != SnakeDirection.UP)
                        snake.Dir = SnakeDirection.DOWN;
                    break;
                case 3:
                    if (snake.Dir != SnakeDirection.RIGHT)
                        snake.Dir = SnakeDirection.LEFT;
                    break;
            }
            snake.Move(map);
            GameOver = CheckGameOver();
            bool eatFruit = CheckEatFruit();
        }
        private void GameRestart()
        {
            GameOver = false;
            for (int i = 1; i < snake.Body.Count; i++)
            {
                this.Controls.Remove(snake.Body[i]);
            }
            snake.Spawn(map);
            fruit.Spawn(snake, map);
            Score = 0;

            if(Who == 3)
            {
                //gen++;
                if (q_learn.EPSILON > 0.005) 
                    q_learn.EPSILON -= 0.005;
            }

            //Для записи матрицы Q в файл
            var str = "";
            foreach(var pair in q_learn.Q)
            {
                str += "{";
                for(int i = 0; i < 4; i++)
                {
                    str += pair.Key.obst[i] +", ";
                }
                for(int i = 0; i < 8; i++)
                {
                    str += pair.Key.fruitLoc[i] + ", ";
                }
                str = str.Remove(str.Length - 2);
                str += "} {";
                double max = pair.Value.actions[0];
                for(int i = 1; i < 4; i++)
                {
                    if(pair.Value.actions[i]>max)
                        max = pair.Value.actions[i];
                }
                for(int i = 0; i < 4; i++)
                {
                    if (pair.Value.actions[i] < max)
                        str += 0 + ", ";
                    else
                        str += 1 + ", ";
                    //str += pair.Value.actions[i] + ", ";
                }
                str = str.Remove(str.Length - 2);
                str += "}\n";
            }
            File.WriteAllText("Records\\DataSet.txt", str);
        }
        private void SaveRecords()
        {
            switch(Who)
            {
                case 1:
                    if (Score > HumanRecord)
                    {
                        HumanRecord = Score;
                        File.WriteAllText("Records\\Human.txt", HumanRecord.ToString());
                        labelHuman.Text = "Human: " + HumanRecord;
                    }
                    break;
                case 2:
                    if (Score > LeeRecord)
                    {
                        LeeRecord = Score;
                        File.WriteAllText("Records\\AlgorithmLee.txt", LeeRecord.ToString());
                        labelLee.Text = "Algorithm Lee: " + LeeRecord;
                    }
                    break;
                case 3:
                    if (Score > QRecord)
                    {
                        QRecord = Score;
                        File.WriteAllText("Records\\Q.txt", QRecord.ToString());
                        labelQ.Text = "Q: " + QRecord;
                    }
                    break;
                    //case 4:
                    //    break;

            }
        }
    }
}

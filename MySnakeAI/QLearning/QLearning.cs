using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySnakeAI
{
    public class QLearning
    {
        public double GAMMA; //влияние будущей награды
        public double ALPHA; //скорость обучения
        public double EPSILON;
        public double[] rewards; //10 - съел фрукт, 1 - приблизился к фрукту, -1 - удалился от фрукта, -100 - умер
        public State[] states;
        public State currentState;
        public Dictionary<State, Action> Q = new Dictionary<State, Action>();

        public int up;
        public int right;
        public int down;
        public int left;

        public int ul;
        public int u;
        public int ur;
        public int r;
        public int dr;
        public int d;
        public int dl;
        public int l;

        public QLearning()
        {
            GAMMA = 0.75;
            ALPHA = 0.01;
            EPSILON = 0.9;
            rewards = new double[4];
            rewards[0] = 5;
            rewards[1] = 1;
            rewards[2] = -1;
            rewards[3] = -100;
            states = new State[144];
            Q = new Dictionary<State, Action>();

            InitStates();
            InitQ();
        }
        public void InitStates()
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
        public void InitQ()
        {
            for (int i = 0; i < 144; i++)
            {
                Q.Add(states[i], new Action());
            }
        }
        public void InstallCurrentState()
        {
            currentState = new State();
            currentState.obst[0] = up;
            currentState.obst[1] = right;
            currentState.obst[2] = down;
            currentState.obst[3] = left;
            currentState.fruitLoc[0] = ul;
            currentState.fruitLoc[1] = u;
            currentState.fruitLoc[2] = ur;
            currentState.fruitLoc[3] = r;
            currentState.fruitLoc[4] = dr;
            currentState.fruitLoc[5] = d;
            currentState.fruitLoc[6] = dl;
            currentState.fruitLoc[7] = l;

            //Какому состоянию из словаря соответствует currentState 
            bool obstTrue = true;
            bool fruitTrue = true;
            for (int i = 0; i < 144; i++)
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
        }
        public void Step(Snake snake)
        {
            InstallCurrentState();
            var index = AgentDecision();
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
        }
        public int AgentDecision()
        {
            var rnd = new Random();
            var check = (double)rnd.Next(0, 100) / 100;
            var index = rnd.Next(0, 4);
            if (check < EPSILON)
            {
                return index; //RandomAction();
            }
            else
            {
                return BestAction();
            }
        }
        public int BestAction()
        {
            Action act = Q[currentState];
            double max = act.actions[0];
            int index = 0;
            for (int i = 0; i < 4; i++)
            {
                if (act.actions[i] > max) 
                {
                    max = act.actions[i]; 
                    index = i; 
                }
            }
            return index;
        }
        public void Update(Snake snake, double reward)
        {
            int index = 0;
            switch(snake.Dir)
            {
                case SnakeDirection.UP:
                    index = 0;
                    break;
                case SnakeDirection.RIGHT:
                    index = 1;
                    break;
                case SnakeDirection.DOWN:
                    index = 2;
                    break;
                case SnakeDirection.LEFT:
                    index = 3;
                    break;
            }
            double currScore = Q[currentState].actions[index];

            double newScore = currScore + ALPHA * (reward + GAMMA * Q[currentState].actions[BestAction()] - currScore);
            Q[currentState].actions[index] = newScore;
        }
        public double GetReward(double distanceAfter, double distanceBefore)
        {
            var d2 = distanceAfter;
            var d1 = distanceBefore;
            if (d2 > d1) 
                return rewards[2];
            else 
                return rewards[1];
        }
    }
}

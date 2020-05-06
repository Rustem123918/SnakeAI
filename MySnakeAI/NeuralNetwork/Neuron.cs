using System;
using System.Collections.Generic;

namespace MySnakeAI
{
    public class Neuron
    {
        public List<double> Weights { get; }
        public List<double> Inputs { get; }
        public NeuronType NeuronType { get; }
        public double Output { get; private set; }
        public double Delta { get; private set; }
        public Neuron(int inputCount,  NeuronType type = NeuronType.Normal)
        {
            NeuronType = type;
            Weights = new List<double>();
            Inputs = new List<double>();

            InitWeightsRandomValue(inputCount);
        }
        private void InitWeightsRandomValue(int inputCount)
        {
            var rnd = new Random();
            for (int i = 0; i < inputCount; i++)
            {
                if(NeuronType == NeuronType.Input)
                    Weights.Add(1);
                else
                    Weights.Add(rnd.NextDouble()); //от 0 до 1
                Inputs.Add(0);
            }
        }
        public double FeedForward(List<double> inputs)
        {
            if (inputs.Count != Weights.Count)
                throw new Exception("Количество весов и входных сигналов не совпадает!");
            
            for(int i = 0; i<inputs.Count; i++)
            {
                Inputs[i] = inputs[i];
            }

            var sum = 0.0;
            for(int i = 0; i<inputs.Count; i++)
            {
                sum += inputs[i] * Weights[i];
            }

            if (NeuronType == NeuronType.Input)
                Output = sum;
            else
                Output = Sigmoid(sum);

            return Output;
        }
        private double Sigmoid(double x)
        {
            var result = 1.0 / (1.0 + Math.Exp(-x));
            return result;
        }
        private double SigmoidDx(double x)
        {
            var sigm = Sigmoid(x);
            var result = sigm * (1 - sigm);
            return result;
        }

        public void Learn(double error, double learnRate)
        {
            if (NeuronType == NeuronType.Input)
                return;

            Delta = error * SigmoidDx(Output);

            for(int i = 0; i < Weights.Count; i++)
            {
                var weight = Weights[i];
                var input = Inputs[i];

                var newWeight = weight - input * Delta * learnRate;
                Weights[i] = newWeight;
            }
        }
        public override string ToString()
        {
            return Output.ToString();
        }
    }
}

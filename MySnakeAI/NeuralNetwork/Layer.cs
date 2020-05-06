using System;
using System.Collections.Generic;

namespace MySnakeAI
{
    public class Layer
    {
        public List<Neuron> Neurons { get; }
        public int NeuronsCount => Neurons?.Count ?? 0;

        public NeuronType Type;
        public Layer(List<Neuron> neurons, NeuronType type = NeuronType.Normal)
        {
            foreach (var neuron in neurons)
                if (neuron.NeuronType != type)
                    throw new Exception("Тип нейрона не соответствует типу слоя");

            Neurons = neurons;
            Type = type;
        }

        public List<double> GetSignals()
        {
            var result = new List<double>();
            foreach(var neuron in Neurons)
            {
                result.Add(neuron.Output);
            }
            return result;
        }

        public override string ToString()
        {
            return Type.ToString();
        }
    }
}

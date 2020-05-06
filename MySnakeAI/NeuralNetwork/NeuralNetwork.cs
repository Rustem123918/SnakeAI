using System;
using System.Collections.Generic;
using System.Linq;

namespace MySnakeAI
{
    public class NeuralNetwork
    {
        public Topology Topology { get; }
        public List<Layer> Layers { get; }
        public NeuralNetwork(Topology topology)
        {
            Topology = topology;
            Layers = new List<Layer>();

            CreateInputLayer();
            CreateHiddenLayers();
            CreateOutputLayer();
        }
        public List<Neuron> FeedForward(List<double> inputSignals)
        {
            if (inputSignals.Count != Topology.InputCount)
                throw new Exception("Количество входных сигналов не соответсвует количество входных нейронов!");

            SendSignalsToInputNeurons(inputSignals);
            FeedForwardAllLayersAfterInput();

            return Layers.Last().Neurons;
        }
        public void Learn(List<Tuple<List<double>, List<double>>> dataset, int epoch) // сделал  void
        {
            //var error = 0.0;

            for(int i = 0; i<epoch; i++)
            {
                foreach(var data in dataset)
                {
                    //error += Backpropagation(data.Item1, data.Item2);
                    Backpropagation(data.Item1, data.Item2);
                }
            }

            //var result = error / epoch;
            //return result;
        }
        private void Backpropagation(List<double> inputs, List<double> expected) // сделал void, поменял местами входные и ожидаемые 
        {
            var actual = new List<double>();
            var lastNeurons = FeedForward(inputs);
            foreach (var n in lastNeurons)
                actual.Add(n.Output);

            int ind = 0;
            foreach(var neuron in Layers.Last().Neurons)
            {
                var difference = actual[ind] - expected[ind];
                neuron.Learn(difference, Topology.LearningRate);
                ind++;
            }

            for(int j = Layers.Count-2; j >= 0; j--)
            {
                var layer = Layers[j];
                if (layer.Type == NeuronType.Input)
                    break;
                var previousLayer = Layers[j + 1];

                for(int i = 0; i<layer.NeuronsCount; i++)
                {
                    var neuron = layer.Neurons[i];

                    for(int k = 0; k<previousLayer.NeuronsCount; k++)
                    {
                        var previousNeuron = previousLayer.Neurons[k];

                        var error = previousNeuron.Weights[i] * previousNeuron.Delta;
                        neuron.Learn(error, Topology.LearningRate);
                    }
                }
            }

            //var result = difference * difference;
            //return result;
        }
        private void FeedForwardAllLayersAfterInput()
        {
            for (int i = 1; i < Layers.Count; i++)
            {
                var layer = Layers[i];
                var previousLayerSignals = Layers[i - 1].GetSignals();

                foreach (var neuron in layer.Neurons)
                    neuron.FeedForward(previousLayerSignals);
            }
        }
        private void SendSignalsToInputNeurons(List<double> inputSignals)
        {
            for (int i = 0; i < inputSignals.Count; i++)
            {
                var signal = new List<double>() { inputSignals[i] };
                var neuron = Layers[0].Neurons[i];
                neuron.FeedForward(signal);
            }
        }
        private void CreateOutputLayer()
        {
            var neurons = new List<Neuron>();
            for(int i = 0; i<Topology.OutputCount; i++)
            {
                var neuron = new Neuron(Layers.Last().NeuronsCount, NeuronType.Output);
                neurons.Add(neuron);
            }
            var layer = new Layer(neurons, NeuronType.Output);
            Layers.Add(layer);

        }
        private void CreateHiddenLayers()
        {
            for (int i = 0; i < Topology.HiddenLayers.Count; i++)
            {
                var neurons = new List<Neuron>();
                for(int j = 0; j<Topology.HiddenLayers[i]; j++)
                {
                    var neuron = new Neuron(Layers.Last().NeuronsCount);
                    neurons.Add(neuron);
                }
                var layer = new Layer(neurons);
                Layers.Add(layer);
            }
        }
        private void CreateInputLayer()
        {
            var inputNeurons = new List<Neuron>();
            for(int i = 0; i<Topology.InputCount; i++)
            {
                var neuron = new Neuron(1, NeuronType.Input);
                inputNeurons.Add(neuron);
            }
            var inputLayer = new Layer(inputNeurons, NeuronType.Input);
            Layers.Add(inputLayer);
        }
    }
}

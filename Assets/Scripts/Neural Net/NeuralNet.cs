using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNet{

	public int inputCount;
	public int outputCount;
	public int hiddenLayerCount;
	public int neuronCount;
	public double alpha; //learning rate
	List<Layer> layers = new List<Layer>();

	public NeuralNet(int inputCount, int outputCount, int hiddenLayerCount, int neuronCountPerHiddenLayer, double alpha)
	{
		this.inputCount = inputCount;
		this.outputCount = outputCount;
		this.hiddenLayerCount = hiddenLayerCount;
		this.neuronCount = neuronCountPerHiddenLayer;
		this.alpha = alpha;

		if(this.hiddenLayerCount > 0)
		{
			layers.Add(new Layer(neuronCount, this.inputCount));

			for(int i = 0; i < this.hiddenLayerCount-1; i++)
			{
				layers.Add(new Layer(neuronCount, neuronCount));
			}

			layers.Add(new Layer(this.outputCount, neuronCount));
		}
		else
		{
			layers.Add(new Layer(this.outputCount, this.inputCount));
		}
	}

	public List<double> Train(List<double> inputValues, List<double> desiredOutput)
	{
		List<double> outputValues = new List<double>();
		outputValues = Predict(inputValues, desiredOutput);
		UpdateWeights(outputValues, desiredOutput);
		return outputValues;
	}

	public List<double> Predict(List<double> inputValues, List<double> desiredOutput)
	{
		List<double> inputs = new List<double>();
		List<double> outputValues = new List<double>();
		int currentInput = 0;

		if(inputValues.Count != inputCount)
		{
			Debug.Log("ERROR: Number of Inputs must be " + inputCount);
			return outputValues;
		}

		inputs = new List<double>(inputValues);
		for(int i = 0; i < hiddenLayerCount + 1; i++)
		{
				if(i > 0)
				{
					inputs = new List<double>(outputValues);
				}
				outputValues.Clear();

				for(int j = 0; j < layers[i].numNeurons; j++)
				{
					double N = 0;
					layers[i].neurons[j].inputs.Clear();

					for(int k = 0; k < layers[i].neurons[j].numInputs; k++)
					{
					    layers[i].neurons[j].inputs.Add(inputs[currentInput]);
						N += layers[i].neurons[j].weights[k] * inputs[currentInput];
						currentInput++;
					}

					N -= layers[i].neurons[j].bias;

					if(i == hiddenLayerCount)
						layers[i].neurons[j].output = ActivationFunctionO(N);
					else
						layers[i].neurons[j].output = ActivationFunction(N);
					
					outputValues.Add(layers[i].neurons[j].output);
					currentInput = 0;
				}
		}
		return outputValues;
	}

	public string PrintWeights()
	{
		string weightStr = "";
		foreach(Layer l in layers)
		{
			foreach(Neuron n in l.neurons)
			{
				foreach(double w in n.weights)
				{
					weightStr += w + ",";
				}
				weightStr += n.bias + ",";
			}
		}
		return weightStr;
	}

	public void LoadWeights(string weightStr)
	{
		if(weightStr == "") return;
		string[] weightValues = weightStr.Split(',');
		int w = 0;
		foreach(Layer l in layers)
		{
			foreach(Neuron n in l.neurons)
			{
				for(int i = 0; i < n.weights.Count; i++)
				{
					n.weights[i] = System.Convert.ToDouble(weightValues[w]);
					w++;
				}
				n.bias = System.Convert.ToDouble(weightValues[w]);
				w++;
			}
		}
	}
	
	void UpdateWeights(List<double> outputs, List<double> desiredOutput)
	{
		double error;
		for(int i = hiddenLayerCount; i >= 0; i--)
		{
			for(int j = 0; j < layers[i].numNeurons; j++)
			{
				if(i == hiddenLayerCount)
				{
					error = desiredOutput[j] - outputs[j];
					layers[i].neurons[j].errorGradient = outputs[j] * (1-outputs[j]) * error;
				}
				else
				{
					layers[i].neurons[j].errorGradient = layers[i].neurons[j].output * (1-layers[i].neurons[j].output);
					double errorGradSum = 0;
					for(int p = 0; p < layers[i+1].numNeurons; p++)
					{
						errorGradSum += layers[i+1].neurons[p].errorGradient * layers[i+1].neurons[p].weights[j];
					}
					layers[i].neurons[j].errorGradient *= errorGradSum;
				}	
				for(int k = 0; k < layers[i].neurons[j].numInputs; k++)
				{
					if(i == hiddenLayerCount)
					{
						error = desiredOutput[j] - outputs[j];
						layers[i].neurons[j].weights[k] += alpha * layers[i].neurons[j].inputs[k] * error;
					}
					else
					{
						layers[i].neurons[j].weights[k] += alpha * layers[i].neurons[j].inputs[k] * layers[i].neurons[j].errorGradient;
					}
				}
				layers[i].neurons[j].bias += alpha * -1 * layers[i].neurons[j].errorGradient;
			}

		}

	}

	double ActivationFunction(double value)
	{
		return TanH(value);
	}

	double ActivationFunctionO(double value)
	{
		return TanH(value);
	}

	double TanH(double value)
	{
		double k = (double) System.Math.Exp(-2*value);
    	return 2 / (1.0f + k) - 1;
	}

	double ReLu(double value)
	{
		if(value > 0) return value;
		else return 0;
	}

	double LeakyReLu(double value)
	{
		if(value < 0) return 0.01*value;
   		else return value;
	}

	double Sigmoid(double value) 
	{
    	double k = (double) System.Math.Exp(value);
    	return k / (1.0f + k);
	}
}

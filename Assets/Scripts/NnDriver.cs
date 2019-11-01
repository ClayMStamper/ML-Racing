using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NnDriver : Driver {

    public int epochs = 1000;
    public float learningRate = 0.5f;

    private NeuralNet net;
    private bool trainingDone = false;
    private float trainingProgress;
    private double sumSquareError;
    private double lastSumSquareError;
    private List<double> outputs = new List<double>(2);

    private List<double> prediction;

    private void Start() {
        //rule of thumb: neruonCount = 2 x Inputs
        net = new NeuralNet(5, 2, 1, 10, learningRate);
        StartCoroutine(LoadTrainingSet());
    }

    private void Update() {
        
        if (!trainingDone)
            return;

        ScanMap();
        GetUserInput();
        Move();
        
    }

    protected override void GetUserInput() {

        outputs = net.Predict(inputs, new List<double> {0, 0});

        translationIn = (float)outputs[0];
        rotationIn = (float) outputs[1];

    }
    
    IEnumerator LoadTrainingSet() {
        
        string path = Application.dataPath + "/trainingData.txt";
        if (File.Exists(path)) {
            for (int e = 0; e < epochs; e++) {
                string[][] rawInputs = new StreamReader(path).ReadToEnd().Split2D('\n', ',');

                for (int i = 0; i < rawInputs.Length; i++) {
                    for (int j = 0; j < rawInputs[i].Length; j++) {

                        Debug.Log("Attempting to parse: " + rawInputs[i][j]);
                        float value;
                        if (!float.TryParse(rawInputs[i][j], out value)) {
                            Debug.LogError("Failed to parse: " + rawInputs[i][j]);
                            Debug.Break();
                        }

                        if (j < net.outputCount - 1) { //first elements are output, not input
                            outputs.Add(value);
                        } else {
                            inputs.Add(value);
                        }

                        prediction = net.Train(inputs, outputs);
                    
                        double sqError = 0;
                        for (int k = 0; k < outputs.Count; k++) {
                            sqError += Math.Pow(outputs[k] - prediction[k], 2);
                        }
                        sumSquareError += sqError;
                    }
                }
                sumSquareError /= rawInputs.Length; // divide by line count
                lastSumSquareError = sumSquareError;
                yield return null;
            }
        } else {
            Debug.LogError("File: " + path + ", not found");
        }

        trainingDone = true;

    }
    
    private void OnGUI() {
        GUI.Label(new Rect(25, 25, 250, 30), "Error: " + lastSumSquareError);
        GUI.Label(new Rect(25, 40, 250, 30), "Alpha: " + net.alpha);
        GUI.Label(new Rect(25, 55, 250, 30), "Training Progress: " + trainingProgress);
    }
    
}

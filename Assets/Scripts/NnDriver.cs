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

        List<double> prediction = net.Predict(mapInputData);

        translationIn = (float)prediction[0];
        rotationIn = (float) prediction[1];

    }
    
    IEnumerator LoadTrainingSet() {
        
        string path = Application.dataPath + "/trainingData.txt";
        if (File.Exists(path)) {
            for (int e = 0; e < epochs; e++) {
                string[][] rawInputs = new StreamReader(path).ReadToEnd().Split2D('\n', ',');

                for (int i = 0; i < rawInputs.Length; i++) {
                    
                    List<double> trainedOutputs = new List<double>();
                    mapInputData.Clear();
                    
                    for (int j = 0; j < rawInputs[i].Length; j++) {

                        string rawDataSplit = rawInputs[i][j];
                        if (string.IsNullOrEmpty(rawDataSplit))
                            continue;
                        
//                        Debug.Log("Attempting to parse: " + rawInputs[i][j]);
                        float value;
                        if (!float.TryParse(rawDataSplit, out value)) {
                            Debug.LogError("Failed to parse: " + rawDataSplit);
                            continue;
                        }

                        if (j < net.outputCount) { //first elements are output, not input
                            //    print("Adding output: " + value + ", at index " + j);
                            trainedOutputs.Add(value);
                        } else {
                            //    print("Adding input: " + value + ", at index " + j);
                            mapInputData.Add(value);
                        }
                    }
                    List<double> predictions = net.Train(mapInputData, trainedOutputs);
                    
                    double sqError = 0;
                    for (int k = 0; k < trainedOutputs.Count; k++) {
                        sqError += Math.Pow(trainedOutputs[k] - predictions[k], 2);
                    }
                    sumSquareError += sqError;
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

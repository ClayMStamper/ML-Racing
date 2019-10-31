using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NnDriver : Driver {

    public int epochs;
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

    private void OnGUI() {
        GUI.Label(new Rect(25, 25, 250, 30), "Error: " + lastSumSquareError);
        GUI.Label(new Rect(25, 40, 250, 30), "Alpha: " + net.alpha);
        GUI.Label(new Rect(25, 55, 250, 30), "Training Progress: " + trainingProgress);
    }

    [ContextMenu("Test")]
    void Test() {
        string path = Application.dataPath + "/trainingData.txt";
        string[][] rawInputs = new StreamReader(path).ReadToEnd().Split2D('\n', ',');

        for (int i = 0; i < rawInputs.Length; i++) {
            for (int j = 0; j < rawInputs[i].Length; j++) {
                Debug.Log(rawInputs[i][j]);
            }
        }
    }

    IEnumerator LoadTrainingSet() {
        
        string path = Application.dataPath + "/trainingData.txt";
        if (File.Exists(path)) {
            
            string[][] rawInputs = new StreamReader(path).ReadToEnd().Split2D('\n', ',');

            for (int i = 0; i < epochs; i++) {
                
            }
            
        } else {
            Debug.LogError("File: " + path + ", not found");
        }

        yield return null;

    }
    
}

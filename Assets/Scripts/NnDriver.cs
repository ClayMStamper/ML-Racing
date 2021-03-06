﻿using System;
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

    private Vector3 startPos;
    
    private void Start() {
        startPos = transform.position;
        //rule of thumb: neruonCount = 2 x Inputs
        net = new NeuralNet(5, 2, 1, 10, learningRate);
        StartCoroutine(LoadTrainingSet());
    }

    private void Update() {

        if (Input.GetKey(KeyCode.Space)) {
            transform.position = startPos;
        }

        if (!trainingDone)
            return;

        ScanMap();
       // Print("Map view: ", mapInputData);
        GetUserInput(); //AI input in this case
        Move();
        
    }

    protected override void GetUserInput() {

       // Print("Lidar data", mapInputData);
        List<double> prediction = net.Predict(mapInputData);

        translationIn = (float)prediction[0];
        rotationIn = ((float) prediction[1]).Remap(0, 1, -1, 1);

    }
    
    protected override void ScanMap() {
        
        eyes.localRotation = Quaternion.Euler(startLook * 90);
        mapInputData = new List<double>();
        
        for (int i = 0; i < net.inputCount; i++) {
            
            Ray ray = new Ray(eyes.position, eyes.forward);
            //look distance normalized and inverted for proper neuron firing
            double mapData = Physics.Raycast(ray, out RaycastHit hit, lookDist) ? 
                Math.Round(1 - (hit.distance / lookDist), decimalPrecision) : 0;
            mapInputData.Add(mapData);
            Debug.DrawRay(ray.origin, ray.direction * (1 - (float)mapInputData[i]), Color.red);
            eyes.Rotate(Vector3.up * scanStep);
            // print("Recording map data " + i + " = " + mapInputData[i]);
        }
        
    }
    
    IEnumerator LoadTrainingSet() {
        
        string path = Application.dataPath + "/trainingData.txt";
        if (File.Exists(path)) {
            string[][] rawInputs = new StreamReader(path).ReadToEnd().Split2D('\n', ',');
            for (int e = 0; e < epochs; e++) {
                for (int i = 0; i < rawInputs.Length; i++) {
                    
                    List<double> recordedUserInput = new List<double>();
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
                            recordedUserInput.Add(value);
                        } else {
                            //    print("Adding input: " + value + ", at index " + j);
                            mapInputData.Add(value);
                        }
                    }

                    if (mapInputData.Count > 0) {
                        List<double> predictions = net.Train(mapInputData, recordedUserInput);

                        double sqError = 0;
                        for (int k = 0; k < recordedUserInput.Count; k++) {
                            sqError += Math.Pow(recordedUserInput[k] - predictions[k], 2);
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
    
    void Print(string name, List<double> vals) {

        string str = "";
		
        str += name + " = \n";
        foreach (double val in vals) {
            str += val;
            str += "\n";
        }
		
        Debug.Log(str + '\n');
		
    }
    
}

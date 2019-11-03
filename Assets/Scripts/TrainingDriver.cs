using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TrainingDriver : Driver {

    private List<string> trainingData = new List<string>();
    private StreamWriter writer;
    
    private void OnApplicationQuit() {
        
        writer = File.CreateText(path);
        List<string> duplicates = new List<string>();

        foreach (string str in trainingData) {
            if (!duplicates.Contains(str)) {
                writer.WriteLine(str);
                duplicates.Add(str);
            }
        }
        writer.Close();
    }

    void Update()
    {
        GetUserInput();
        Move();
        ScanMap();
        StoreScanData();

    }

    protected override void Move() {

        mapInputData[0] = Math.Round(translationIn.Remap(-1, 1, 0, 1), decimalPrecision);
        mapInputData[1] = Math.Round(rotationIn.Remap(-1, 1, 0, 1), decimalPrecision);
        
        base.Move();
        
    }

    private void StoreScanData() {

        string dataString = "";

        //add entries with commas
        for (int i = 0; i < mapInputData.Count - 1; i++) {
            dataString += mapInputData[i] + ",";
           // print("Data string at index " + i + " = " + dataString);
        }

        //add final entry without comma
        dataString += mapInputData[mapInputData.Count - 1];
        trainingData.Add(dataString);
        
    }
    
}

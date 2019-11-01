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

        foreach (string str in trainingData) {
            writer.WriteLine(str);
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

        inputs[0] = Math.Round(translationIn.Remap(-1, 1, 0, 1), 2);
        inputs[1] = Math.Round(rotationIn.Remap(-1, 1, 0, 1), 2);
        
        base.Move();
        
    }

    private void StoreScanData() {

        string dataString = "";

        //add entries with commas
        for (int i = 0; i < inputs.Count - 2; i++) {
            dataString += inputs[i] + ",";
        }

        //add final entry without comma
        dataString += inputs[inputs.Count - 1];
        
        if (!trainingData.Contains(dataString)) //no duplicates
            trainingData.Add(dataString);
        
    }
    
}

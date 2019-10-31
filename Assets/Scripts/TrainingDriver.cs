using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TrainingDriver : Driver {



    private List<string> trainingData = new List<string>();
    private StreamWriter writer;

    private void Start() {
        string path = Application.dataPath + "/trainingData.txt";
        writer = File.CreateText(path);
    }

    private void OnApplicationQuit() {
        foreach (string str in trainingData) {
            writer.WriteLine(str);
        }
        writer.Close();
    }

    void Update()
    {
        Move();
        ScanMap();
    }

    private void Move() {
        
        float translationIn = Input.GetAxis("Vertical");
        float rotationIn = Input.GetAxis("Horizontal") ;

        inputs[0] = Math.Round(translationIn, 2);
        inputs[1] = Math.Round(rotationIn, 2);
        
        float translation = translationIn * speed * Time.deltaTime;
        float rotation = rotationIn* rotationSpeed * Time.deltaTime;

        transform.Translate(0, 0, translation );
        transform.Rotate(0, rotation, 0);
        
    }

    private void ScanMap() {

        eyes.localRotation = Quaternion.Euler(startLook * 90);
        
        for (int i = 2; i < inputs.Count; i++) {
            
            Ray ray = new Ray(eyes.position, eyes.forward);
            inputs[i] = Physics.Raycast(ray, out RaycastHit hit, lookDist) ? 
                Math.Round(1 - (hit.distance / lookDist), 2) : 
                0;
            Debug.DrawRay(ray.origin, ray.direction * (1 - (float)inputs[i]), Color.red);
            eyes.Rotate(Vector3.up * scanStep);
            
        }
        
        ParseData();
        
    }

    private void ParseData() {

        string dataString = "";

        //add entries with commas
        for (int i = 0; i < inputs.Count - 2; i++) {
            dataString += inputs[i] + ",";
        }

        //add final entry without comma
        dataString += inputs[inputs.Count - 1];
        
        trainingData.Add(dataString);
        
    }
    
}

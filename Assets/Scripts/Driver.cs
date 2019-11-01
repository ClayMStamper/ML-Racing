using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Driver : MonoBehaviour {

    public float speed = 8.0f;
    public float rotationSpeed = 200.0f;
    public float lookDist = 25f;
    public float scanStep = 45;
    public Vector3 startLook = -Vector3.up; //negative on y axis is left - cover fan left to right
    public int decimalPrecision = 0;
    
    [SerializeField]
    protected Transform eyes;
    [SerializeField]
    protected List<double> mapInputData;
    
    protected string path;
    protected float translationIn;
    protected float rotationIn;

    private void Awake() {
       path = Application.dataPath + "/trainingData.txt";
    }

    protected virtual void GetUserInput() {
        
        translationIn = Input.GetAxis("Vertical");
        rotationIn = Input.GetAxis("Horizontal") ;
        
    }
    
    protected virtual void ScanMap() {
        
        eyes.localRotation = Quaternion.Euler(startLook * 90);
        
        for (int i = 2; i < mapInputData.Count; i++) {
            
            Ray ray = new Ray(eyes.position, eyes.forward);
            //look distance normalized and inverted for proper neuron firing
            mapInputData[i] = Physics.Raycast(ray, out RaycastHit hit, lookDist) ? 
                Math.Round(1 - (hit.distance / lookDist), decimalPrecision) : 0;
            Debug.DrawRay(ray.origin, ray.direction * (1 - (float)mapInputData[i]), Color.red);
            eyes.Rotate(Vector3.up * scanStep);
            
           // print("Recording map data " + i + " = " + mapInputData[i]);
            
            
        }
        
    }

    protected virtual void Move() {
        
        float translation = translationIn * speed * Time.deltaTime;
        float rotation = rotationIn* rotationSpeed * Time.deltaTime;

        transform.Translate(0, 0, translation );
        transform.Rotate(0, rotation, 0);
        
    }


    
}

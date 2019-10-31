using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Driver : MonoBehaviour {

    public float speed = 100.0f;
    public float rotationSpeed = 100.0f;
    public float lookDist = 75f;
    public float scanStep = 45;
    public int numberOfScans = 5;
    public Vector3 startLook = -Vector3.up; //negative on y axis is left - cover fan left to right
    
    [SerializeField]
    protected Transform eyes;
    [SerializeField]
    protected List<double> inputs;
    
}

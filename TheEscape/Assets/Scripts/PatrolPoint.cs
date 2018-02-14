using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPoint : MonoBehaviour {

    public Vector3 position;
    public float waitTimeOut;
    // Use this for initialization
    void Start()
    {
        position = transform.position;        
    }
}

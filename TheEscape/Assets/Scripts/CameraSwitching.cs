using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitching : MonoBehaviour
{

    public Camera mainCamera;
    // public Camera alternateCamera;
    private int origMask = -1;
    private int seeThroughMask = -5;

    void Start()
    {
        // alternateCamera.enabled = false;
        mainCamera.enabled = true;
    }
    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 9)
        {
            mainCamera.cullingMask = seeThroughMask;
            // mainCamera.enabled = false;
            // alternateCamera.enabled = true;
        }        
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            mainCamera.cullingMask = origMask;
            // alternateCamera.enabled = false;
            // mainCamera.enabled = true;
        }          
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            if (mainCamera.enabled)
            {
                // mainCamera.enabled = false;
                // alternateCamera.enabled = true;
            }
        }
    }
}
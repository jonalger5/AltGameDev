using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitching : MonoBehaviour
{

    public Camera mainCamera;
    public Camera alternateCamera;

    public void OnTriggerEnter(Collider other)
    {
        mainCamera.enabled = false;
        alternateCamera.enabled = true;
    }

    public void OnTriggerExit(Collider other)
    {
        alternateCamera.enabled = false;
        mainCamera.enabled = true;
    }

    public void OnTriggerStay(Collider other)
    {
        if (mainCamera.enabled)
        {
            mainCamera.enabled = false;
            alternateCamera.enabled = true;
        }
    }
}
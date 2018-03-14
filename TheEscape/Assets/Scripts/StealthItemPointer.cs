using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class StealthItemPointer : MonoBehaviour {

    public GameObject player;
    public GameObject item;
    private float prevHeadAngle;
    private float prevAngle;

    // Use this for initialization
    void Start () {
        prevHeadAngle = 360;
        prevAngle = 0;
    }

    void UpdateAngle ()
    {
        var playerPos = player.transform.position;
        var itemPosition = item.transform.position;
        RectTransform rectTransform = GetComponent<RectTransform>();

        float directZ = playerPos.z - itemPosition.z;
        float directX = playerPos.x - itemPosition.x;
        float angle = Mathf.Atan2(directZ, directX) * Mathf.Rad2Deg;

        Vector3 forward = player.transform.forward;
        forward.y = 0;
        float headingAngle = Quaternion.LookRotation(forward).eulerAngles.y;
        headingAngle = headingAngle - 270;
        print("headingAngle: " + headingAngle);
        print("prevAngle: " + prevHeadAngle);
        print("angle: " + angle);
        float angleToRotate = (angle - prevAngle);
        if (angle <= 180)
        {
            angleToRotate -= (prevHeadAngle - headingAngle);
        }
        else
        {
            angleToRotate += (prevHeadAngle - headingAngle);
        }
        rectTransform.Rotate(new Vector3(0, 0, angleToRotate)); 
        prevHeadAngle = headingAngle;
        prevAngle = angle;
    }
	
	// Update is called once per frame
	void Update () {
        UpdateAngle();
    }
}

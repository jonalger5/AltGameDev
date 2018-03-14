using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthItemPointer : MonoBehaviour {

    public GameObject player;
    private PlayerController _player;
    public GameObject item;

    // Use this for initialization
    void Start () {
        _player = GameObject.Find("MainCharacter").GetComponent<PlayerController>();
    }
	
	// Update is called once per frame
	void Update () {
        var playerPos = player.transform.position;
        var itemPosition = item.transform.position;

        Vector3 dir = playerPos - itemPosition;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // Vector3 forward = player.transform.forward;
        //forward.y = 0;
        // float headingAngle = Quaternion.LookRotation(forward).eulerAngles.y;

        //RectTransform rectTransform = GetComponent<RectTransform>();
        //float correctionAngle = rectTransform.eulerAngles.z - angle;
        //rectTransform.Rotate(new Vector3(0, 0, angle));
        

        //Quaternion newAngle = Quaternion.FromToRotation(playerPos, itemPosition);
        //rectTransform.SetPositionAndRotation(playerPos, newAngle);
        //rectTransform.SetPositionAndRotation(itemPosition, angle);
       
        // new Quaternion(rotation.x, 0, rotation.z, rotation.w)

        Quaternion rotation = Quaternion.LookRotation(_player.transform.position - item.transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 10);
    }
}

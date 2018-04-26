using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimConditions : MonoBehaviour
{
    Animator anim;
    public bool isStealth;

    // Use this for initialization
    void Start ()
    {
        anim = GetComponent<Animator>();
        
        if (isStealth)
        {
            anim.SetBool("isStealth", true);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        // button presses
		if (Input.GetKeyDown(KeyCode.W))
        {
            anim.SetBool("isWalking", true);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            anim.SetBool("isSneaking", true);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            anim.SetBool("movingLeft", true);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            anim.SetBool("movingRight", true);
        }

        // button releases
        if (Input.GetKeyUp(KeyCode.W))
        {
            anim.SetBool("isWalking", false);
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
             anim.SetBool("isSneaking", false);
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            anim.SetBool("movingLeft", false);
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            anim.SetBool("movingRight", false);
        }
    }
}

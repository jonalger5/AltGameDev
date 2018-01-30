using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    [SerializeField]
    public float speed;
    [SerializeField]
    public float health;
    private Text healthUI;

    private Rigidbody rb;
    private Renderer renderer;

    // Use this for initialization
    void Start () {
        healthUI = GameObject.Find("/HealthUI/Health").GetComponent<Text>();
        rb = GetComponent<Rigidbody>();
        renderer = GetComponent<Renderer>();
        renderer.material.color = Color.yellow;
	}
	
	// Update is called once per frame
	void Update () {

        //Used for Movement
        transform.Translate(Vector3.forward * Time.deltaTime * Input.GetAxis("Vertical") * speed);
        transform.Translate(Vector3.right * Time.deltaTime * Input.GetAxis("Horizontal") * speed);

        //Updating HealthUI
        healthUI.text = health.ToString();

    }
}

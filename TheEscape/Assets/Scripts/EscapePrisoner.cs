using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapePrisoner : MonoBehaviour {

    private Animator _anim;
	// Use this for initialization
	void Start () {
		_anim = GetComponent<Animator>();
        StartCoroutine(PlayDeath());
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator PlayDeath()
    {
        yield return new WaitForSeconds(8f);
        _anim.Play("Dying");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour {

    private List<string> storytext;
    private int index;
    private Text text;

	// Use this for initialization
	void Start () {
        index = 0;
        storytext = new List<string>();
        storytext.Add("On July 31, 1941, Nazi leader Hermann Goering authorized SS General Reinhard Heydrich to make preparations for the implementation of a \"complete solution of the Jewish question.\"");
        storytext.Add("As part of Operation Reinhard, Nazi leaders established three extermination camps in Poland—Belzec, Sobibor, and Treblinka—with the sole purpose of the mass murder of Jews.");
        storytext.Add("The prisoners of these camps were forced to sort through the belongings of those killed in the gas chambers.");
        storytext.Add("In times of desperation and against unfavorable odds, some prisoners were willing to try and escape or die trying.");
        text = GameObject.Find("/Canvas/Text").GetComponent<Text>();
        text.text = storytext[index];
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.anyKeyDown)
        {
            index++;
            if (index == storytext.Count)
            {
                //GameManager.gm.AdvanceScene();
                SceneManager.LoadScene(2);
                
            }
            else
            {
                text.text = storytext[index];

            }
        }
	}
}

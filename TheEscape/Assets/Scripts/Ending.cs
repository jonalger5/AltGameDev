using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Ending : MonoBehaviour {

    private List<string> storytext;
    private int index;
    private Text text;
    private Canvas credits;
    private bool showingCredits = false;

    // Use this for initialization
    void Start()
    {
        index = 0;
        storytext = new List<string>();
        storytext.Add("By 1945, the Germans and their collaborators killed nearly two out of every three European Jews as part of the \"Final Solution\".");
        storytext.Add("At the extermination camp of Sobibor, nearly all escape attempts resulted death.");
        storytext.Add("Of the hundreds of thousands of people who were sent to Sobibor, only 50 people were able to survive the war.");
        text = GameObject.Find("/Canvas/Text").GetComponent<Text>();
        text.text = storytext[index];
        credits = GameObject.Find("Credits").GetComponent<Canvas>();
        credits.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.anyKeyDown)
        {
            if(showingCredits)
                SceneManager.LoadScene(0);
            else
            {
                index++;
                if (index == storytext.Count)
                {
                    credits.gameObject.SetActive(true);
                    showingCredits = true;
                }

                else
                    text.text = storytext[index];
            }           
        }
    }
}

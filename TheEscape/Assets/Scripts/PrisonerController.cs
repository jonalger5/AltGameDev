using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrisonerController : MonoBehaviour {

    [SerializeField]
    public int id;
    public List<Quest> Quests = new List<Quest>();
    
	// Use this for initialization
	void Start () {
		QuestDatabase qd = GameObject.Find("Quest Database").GetComponent<QuestDatabase>();
        foreach(Quest q in qd.Quests)
        {
            if (q.prisonerID == id)
                Quests.Add(q);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

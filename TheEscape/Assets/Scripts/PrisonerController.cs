using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrisonerController : MonoBehaviour {

    [SerializeField]
    public int id;
    public List<Quest> Quests = new List<Quest>();
    public Quest activeQuest;
    public bool questInProgress = false;
    private QuestDatabase qd;
    
	// Use this for initialization
	void Start ()
    {
		qd = GameObject.Find("Quest Database").GetComponent<QuestDatabase>();
        foreach(Quest q in qd.Quests)
        {
            if (q.prisonerID == id)
                Quests.Add(q);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public Quest GetNextQuest()
    {
        foreach(Quest q in Quests)
        {
            if (!q.complete)
            {
                activeQuest = q;
                questInProgress = true;
                return q;
            }
        }
        return null;
    }

    public void ReturnQuest()
    {
        qd.Quests[activeQuest.questID].complete = true;
        Quests[activeQuest.questID].complete = true;
        activeQuest = null;
        questInProgress = false;
    }
}

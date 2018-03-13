using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrisonerController : MonoBehaviour {

    [SerializeField]
    public int id;
    [SerializeField]
    private float lookDistance;
    [SerializeField]
    private float dampling;
    private Quaternion forwardRotation;

    public List<Quest> Quests = new List<Quest>();
    public Quest activeQuest;
    public bool questInProgress = false;
    private QuestDatabase qd;

    private PlayerController _player;

	// Use this for initialization
	void Start ()
    {
        forwardRotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, transform.rotation.z);
		qd = GameObject.Find("Quest Database").GetComponent<QuestDatabase>();
        foreach(Quest q in qd.Quests)
        {
            if (q.prisonerID == id)
                Quests.Add(q);
        }

        _player = GameObject.Find("MainCharacter").GetComponent<PlayerController>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        float targetDistance = Vector3.Distance(_player.transform.position, transform.position);

        if (targetDistance < lookDistance)
            LookAtPlayer();
        else
            FaceFoward();
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

        //Update Quests
        Quests = new List<Quest>();
        foreach (Quest q in qd.Quests)
        {
            if (q.prisonerID == id)
                Quests.Add(q);
        }
        activeQuest = null;
        questInProgress = false;
    }

    private void LookAtPlayer()
    {
        Quaternion rotation = Quaternion.LookRotation(_player.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * dampling);
    }

    private void FaceFoward()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, forwardRotation, Time.deltaTime * dampling);
    }
}

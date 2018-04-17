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
    private Vector3 forwardRotation;

    public List<Quest> Quests = new List<Quest>();
    public Quest activeQuest;
    public bool hasReturnedQuest = false;

    private PlayerController _player;
    [SerializeField]
    private GameObject _complete;
    [SerializeField]
    private GameObject _standby;
    [SerializeField]
    private GameObject _incomplete;

	// Use this for initialization
	void Start ()
    {
        //forwardRotation = new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z);
        //forwardRotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, transform.rotation.z);

        foreach(Quest q in GameManager.gm.qdInstance.Quests)
        {
            if (q.prisonerID == id)
                Quests.Add(q);
        }

        foreach(Quest q in Quests)
        {
            if (q.inProgress)
            {
                activeQuest = q;
                break;
            }
        }
        _player = GameObject.Find("MainCharacter").GetComponent<PlayerController>();
        _complete.SetActive(false);
        _standby.SetActive(false);
        _incomplete.SetActive(false);      
    }
	
	// Update is called once per frame
	void Update ()
    {
        float targetDistance = Vector3.Distance(_player.transform.position, transform.position);

        if (targetDistance < lookDistance)
            LookAtPlayer();
        //else
        //    FaceFoward();

        if (activeQuest.questItem.name == "")
            activeQuest = GetNextQuest();

        if (activeQuest.questItem.name != "")
        {
            if (_player.CheckQuest(activeQuest))
            {
                _complete.SetActive(true);
                _standby.SetActive(false);
                _incomplete.SetActive(false);
            }
            else if (activeQuest.inProgress)
            {
                _complete.SetActive(false);
                _standby.SetActive(true);
                _incomplete.SetActive(false);
            }
            else if (!activeQuest.inProgress)
            {
                _complete.SetActive(false);
                _standby.SetActive(false);
                _incomplete.SetActive(true);
            }
        }

        if(hasReturnedQuest)
        {
            _complete.SetActive(false);
            _standby.SetActive(false);
            _incomplete.SetActive(false);
        }
    }

    public Quest GetNextQuest()
    {
        foreach(Quest q in Quests)
        {
            if (!q.complete && !q.inProgress)
            {
                activeQuest = q;
                //GameManager.gm.qdInstance.Quests[q.questID].inProgress = true;
                return q;
            }
        }
        return null;
    }

    public void ReturnQuest()
    {
        GameManager.gm.qdInstance.Quests[activeQuest.questID].complete = true;
        GameManager.gm.qdInstance.Quests[activeQuest.questID].inProgress = false;

        //Update Quests
        Quests = new List<Quest>();
        foreach (Quest q in GameManager.gm.qdInstance.Quests)
        {
            if (q.prisonerID == id)
                Quests.Add(q);
        }
        activeQuest = null;
        hasReturnedQuest = true;
    }

    private void LookAtPlayer()
    {
        Quaternion rotation = Quaternion.LookRotation(_player.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * dampling);
    }

    private void FaceFoward()
    {
        Quaternion rotation = Quaternion.Euler(forwardRotation);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * dampling);
    }
}

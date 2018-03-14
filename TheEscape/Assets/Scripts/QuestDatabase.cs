using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestDatabase : MonoBehaviour{

    public List<Quest> Quests = new List<Quest>();

    void Start()
    {
        for (int i = 0; i < Quests.Count; i++)
            Quests[i].questID = i;
    }
}

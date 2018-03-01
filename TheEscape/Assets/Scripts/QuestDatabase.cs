using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestDatabase : MonoBehaviour {

    //Key is Prisoner ID
    //Quests for that Prisoner
    public Dictionary<int, List<Quest>> Quests = new Dictionary<int, List<Quest>>();
}

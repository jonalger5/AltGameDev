using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest{

    public int prisonerID;
    public int questID;
    public Item questItem;
    public List<string> acceptDialogue;
    public List<string> returnDialogue;
    public List<string> standbyDialogue;
    public string desc;
    public bool inProgress;
    public bool complete;
}

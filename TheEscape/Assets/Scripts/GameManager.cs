using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public int sceneIndex;
    public float playerHealth;
    public float playerStartingHealth;
    public List<int> currentQuests;
    public List<Item> inventory;
    public QuestDatabase questDatabase;
    public QuestDatabaseInstance qdInstance;
    public GameObject rollCallPoint;
    public bool hasReceivedQuest;

    public static GameManager gm;

	// Use this for initialization
	void Awake () {
		if(gm == null)
        {
            gm = this;
            InitializeValues();
            DontDestroyOnLoad(gameObject);
        }
    }
	
    void InitializeValues()
    {
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
        playerHealth = 100f;
        playerStartingHealth = playerHealth;
        currentQuests = new List<int>();
        inventory = new List<Item>();     
        questDatabase = GameObject.Find("Quest Database").GetComponent<QuestDatabase>();
        qdInstance = new QuestDatabaseInstance();
        qdInstance.Quests = questDatabase.Quests;
        hasReceivedQuest = false;
    }

    public void AdvanceScene()
    {
        playerStartingHealth = playerHealth;
        sceneIndex++;
        if (sceneIndex == SceneManager.sceneCountInBuildSettings)
            sceneIndex = 0;
        SceneManager.LoadScene(sceneIndex);
    }

    public void ReloadScene()
    {
        playerHealth = playerStartingHealth;
        SceneManager.LoadScene(sceneIndex);
    }

    public IEnumerator RollCall(Canvas textUI, Text text)
    {
        yield return new WaitForSeconds(3f);
        textUI.gameObject.SetActive(true);
        text.text = "* Whistle *";
        yield return new WaitForSeconds(2f);
        text.text = "Guard: Everyone fall in!";
        yield return new WaitForSeconds(2f);
        text.text = "Report for Roll Call";
    }
}

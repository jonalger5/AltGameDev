using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public int sceneIndex;
    public float playerHealth;
    public List<int> currentQuests;
    public ItemDatabase itemDatabase;
    public QuestDatabase questDatabase;
    public QuestDatabaseInstance qdInstance;

    public static GameManager gm;

	// Use this for initialization
	void Awake () {
		if(gm == null)
        {
            gm = this;
            InitializeValues();
            DontDestroyOnLoad(gameObject);
        }
        itemDatabase = GameObject.Find("Item Database").GetComponent<ItemDatabase>();
    }
	
    void InitializeValues()
    {
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
        playerHealth = 100f;
        currentQuests = new List<int>();        
        questDatabase = GameObject.Find("Quest Database").GetComponent<QuestDatabase>();
        qdInstance = new QuestDatabaseInstance();
        qdInstance.Quests = questDatabase.Quests;
    }

    public void AdvanceScene()
    {
        sceneIndex++;
        SceneManager.LoadScene("CharacterScene");
    }
}

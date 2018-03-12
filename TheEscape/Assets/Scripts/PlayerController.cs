using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerController : MonoBehaviour {


    private enum DialogueType
    {
        accepting,
        returning, 
        standby
    }

    [SerializeField]
    public float speed;
    [SerializeField]
    public float sensitivity;
    [SerializeField]
    public float health;
    private Text healthUI;
    private Canvas deathScreenUI;
    private Canvas PauseScreenUI;

    //Dialogue Variables
    private Canvas dialogueUI;
    private Text dialogueText;
    private int dialogueIndex;
    private bool isTalking;
    private DialogueType dialogueType;

    //Quest Variables
    private QuestDatabase qd;
    public List<int> currentQuests = new List<int>();
    private int currentQuestIndex = 0;
    private Canvas questUI;

    private new Renderer renderer;

    public List<Item> inventory;
    private int slotX = 2, slotY = 3;
    private ItemDatabase database;
    private bool showInventory = false;
    private bool showItem = false;
    private string ItemDetails;
    public Texture2D InventoryBackground;
    public Texture2D EmptySlot;
    public GUISkin guiSkin;

    public List<Item> StealItems;
    private int itemnum;

    private int deleteloc;
    private bool del = false;
    [SerializeField]
    private float _stealTimeout;
    public float stealTimer = 0;
    public bool isStealing = false;

    public int depositQuota;
    public float timer;
    public Text timerText;
    public Text quotaText;

    private bool isPaused;
    private float timerdecrement;

    private int NumOfItems ;
    private int Position;
    private int Position1;
    private bool CanAccess = true;

    private Canvas VictoryScreenUI;
    private Canvas EndScreen;
    private Canvas EndScreen1;

    private Dictionary<int, double> percentages;

    // Use this for initialization
    void Start () {

        healthUI = GameObject.Find("/HealthUI/Health").GetComponent<Text>();
        dialogueUI = GameObject.Find("DialogueUI").GetComponent<Canvas>();
        dialogueText = GameObject.Find("/DialogueUI/DialogueText").GetComponent<Text>();
        dialogueUI.gameObject.SetActive(false);
        questUI = GameObject.Find("QuestUI").GetComponent<Canvas>();
        questUI.gameObject.SetActive(false);

        deathScreenUI = GameObject.Find("DeathScreenUI").GetComponent<Canvas>();
        deathScreenUI.gameObject.SetActive(false);

        PauseScreenUI = GameObject.Find("PauseScreen").GetComponent<Canvas>();
        PauseScreenUI.gameObject.SetActive(false);
        isPaused = false;

        VictoryScreenUI = GameObject.Find("VictoryScreen").GetComponent<Canvas>();
        VictoryScreenUI.gameObject.SetActive(false);

        EndScreen = GameObject.Find("LoseScreen1").GetComponent<Canvas>();
        EndScreen.gameObject.SetActive(false);

        EndScreen1 = GameObject.Find("LoseScreen2").GetComponent<Canvas>();
        EndScreen1.gameObject.SetActive(false);

        renderer = GetComponent<Renderer>();
        renderer.material.color = Color.yellow;
        inventory = new List<Item>();
        database = GameObject.Find("Item Database").GetComponent<ItemDatabase>();
        qd = GameObject.Find("Quest Database").GetComponent<QuestDatabase>();

        StealItems = new List<Item>();
        Time.timeScale = 1;

        deathScreenUI.gameObject.SetActive(false);
        PauseScreenUI.gameObject.SetActive(false);
        VictoryScreenUI.gameObject.SetActive(false);
        EndScreen.gameObject.SetActive(false);
        EndScreen1.gameObject.SetActive(false);

        timer = 60;
        depositQuota = 51;
        UpdateTimerText();
        UpdateQuotaText();

        Cursor.visible = false;
        timerdecrement = 0;
        timerdecrement = Time.fixedUnscaledDeltaTime;


        NumOfItems = 6;
        percentages = new Dictionary<int, double>();
        percentages.Add(0, 0);
        percentages.Add(1, .05);
        percentages.Add(2, .10);
        percentages.Add(3, .25);
        percentages.Add(4, .50);
        percentages.Add(5, .75);
        percentages.Add(6, .95);
    }

    void UpdateTimerText()
    {
        timerText.text = "Time Left: " + timer.ToString("F2");
    }

    void UpdateQuotaText()
    {
        depositQuota--;
        if (depositQuota < 0)
        {
            depositQuota = 0;
        }
        quotaText.text = "Remaining Items: " + depositQuota.ToString();
    }
    // Update is called once per frame
    void Update ()
    {
        if (timer > 0.0f)
        {
            timer -= timerdecrement;
            if (timer < 0.0f)
            {
                DisplayEndScreen();
                timer = 0.0f;
            }
            UpdateTimerText();
        }

        
        //Activate Death Screen
        if (health <= 0)
        {
            healthUI.text = "0";
            deathScreenUI.gameObject.SetActive(true);
            Time.timeScale = 0;
        }
        //Updating HealthUI
        else
            healthUI.text = health.ToString();

        //Used for Movement
        if (!isTalking)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * Input.GetAxis("Vertical") * speed);
            transform.Translate(Vector3.right * Time.deltaTime * Input.GetAxis("Horizontal") * speed);
        }

        if (!showInventory && !isTalking)
            transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivity, 0);

        if (Input.GetKeyDown(KeyCode.I))
        {
            showInventory = !showInventory;
            Cursor.visible = !Cursor.visible;
            if (!showInventory)
                questUI.gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            showInventory = !showInventory;
            if (isPaused)
            {
                timerdecrement = 0;
                Time.timeScale = 0;
                transform.Rotate(0, 0, 0);
            }
            else if (!isPaused)
            {
                timerdecrement = Time.fixedUnscaledDeltaTime;
                Time.timeScale = 1;
                transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivity, 0);
            }
            Cursor.visible = !Cursor.visible;
            
            PauseScreenUI.gameObject.SetActive(isPaused);
        }

        if (Input.GetKeyDown(KeyCode.Space) && isTalking)
        {
            switch (dialogueType)
            {
                case DialogueType.accepting:
                    
                    if (dialogueIndex != qd.Quests[currentQuestIndex].acceptDialogue.Count - 1)
                    {
                        dialogueIndex++;
                        dialogueText.text = qd.Quests[currentQuestIndex].acceptDialogue[dialogueIndex];
                    }
                    else
                    {
                        dialogueUI.gameObject.SetActive(false);
                        isTalking = false;
                    }
                        
                    break;

                case DialogueType.returning:
                    if (dialogueIndex != qd.Quests[currentQuestIndex].returnDialogue.Count - 1)
                    {
                        dialogueIndex++;
                        dialogueText.text = qd.Quests[currentQuestIndex].returnDialogue[dialogueIndex];
                    }
                    else
                    {
                        dialogueUI.gameObject.SetActive(false);
                        isTalking = false;
                    }

                    break;

                case DialogueType.standby:
                    if (dialogueIndex != qd.Quests[currentQuestIndex].standbyDialogue.Count - 1)
                    {
                        dialogueIndex++;
                        dialogueText.text = qd.Quests[currentQuestIndex].standbyDialogue[dialogueIndex];
                    }
                    else
                    {
                        dialogueUI.gameObject.SetActive(false);
                        isTalking = false;
                    }

                    break;

                default:
                    break;
            }
        }

        if (isStealing)
        {
            stealTimer += Time.deltaTime;
            if (stealTimer >= _stealTimeout)
            {
                isStealing = false;
                stealTimer = 0;
            }
        }        
    }
    public IEnumerator LetsWait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        CanAccess = !CanAccess;
    }

    private void Remove(int position)
    {
        inventory.RemoveAt(position);
        StartCoroutine(LetsWait((float)0.1));
    }
    public void RemoveSteal(int position)
    {
        StealItems.RemoveAt(position);
        StartCoroutine(LetsWait((float)0.1));
    }
    
    void OnGUI()
    {
        GUI.skin = guiSkin;
        ItemDetails = "";
        
        if (showInventory)
        {
            
            //GUI.Box(new Rect(10, 50, 240, 240), InventoryBackground);
            for (int x = 0; x < slotX; x++)
            {
                for (int y = 0; y < slotY; y++)
                {
                    Position = x*slotY + y;
                    if (Position < inventory.Count)
                    {
                        Rect slot = new Rect(Screen.width/100 + y * 60, Screen.height / 10 + x * 60, 50, 50);
                        GUI.Box(slot, inventory[Position].icon);
                        if (slot.Contains(Event.current.mousePosition))
                        {
                            ItemDetails = ShowItem(inventory[Position]);
                            showItem = true;
                            

                            if(Input.GetButtonUp("space") && inventory[Position].type == Item.ItemType.Consumable && health < 100 && CanAccess)
                            {
                                CanAccess = !CanAccess;
                                health += inventory[Position].value;
                                Remove(Position);
                                showItem = false;
                            }


                            if (Input.GetMouseButtonDown(0) && CanAccess)
                            {
                                CanAccess = !CanAccess;
                                isStealing = true;
                                StealItems.Add(inventory[Position]);
                                Remove(Position);
                                NumOfItems--;
                                showItem = false;

                            }
                        }

                        if (ItemDetails == "")
                            showItem = false;
                    }
                    else
                    {
                        Rect slot = new Rect(Screen.width/100 + y * 60, Screen.height/10 + x * 60, 50, 50);
                        GUI.Box(slot, EmptySlot);
                    }                  
                }
            }
            for (int i = 0; i < slotX; i++)
            {
                for (int j = 0; j < slotY; j++)
                {

                    Position1 = i * slotY + j;
                    if (Position1 < StealItems.Count)
                    {
                        Rect slot = new Rect(Screen.width / 100 + j * 60, Screen.height / 3 + i * 60, 50, 50);
                        GUI.Box(slot, StealItems[Position1].icon);
                        if (slot.Contains(Event.current.mousePosition))
                        {
                            ItemDetails = ShowItem(StealItems[Position1]);
                            showItem = true;

                            if (Input.GetMouseButtonDown(0) && CanAccess)
                            {
                                CanAccess = !CanAccess;
                                inventory.Add(StealItems[Position1]);
                                RemoveSteal(Position1);
                                NumOfItems++;
                                showItem = false;

                            }
                        }

                        if (ItemDetails == "")
                            showItem = false;
                    }
                    else
                    {
                        Rect slot = new Rect(Screen.width / 100 + j * 60, Screen.height / 3 + i * 60, 50, 50);
                        GUI.Box(slot, EmptySlot);
                    }
                }
            }
            questUI.gameObject.SetActive(true);
            for(int i = 0; i < currentQuests.Count; i++)
            {
                if(CheckQuest(qd.Quests[currentQuests[i]]))
                    GUI.Label(new Rect(4 * Screen.width / 5, Screen.height / 5 + i * 20, 150, 50), qd.Quests[currentQuests[i]].desc, GUI.skin.customStyles[1]);

                else
                    GUI.Label(new Rect(4 * Screen.width / 5, Screen.height / 5 + i * 20, 150, 50), qd.Quests[currentQuests[i]].desc, GUI.skin.customStyles[2]);
            } 
        }

        if (showItem)
        {
            GUI.Box(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 150, 50), ItemDetails);
        }
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "PileOfItems")
        {
            int PickupNum = NumOfItems - inventory.Count;
            for (int i = 0;i < PickupNum; i++)
            {
                itemnum = Random.Range(0, 4);
                inventory.Add(database.Items[itemnum]);
                
            }

        }

        if(other.gameObject.name == "RedPile")
        {
            for(int i = 0;i < inventory.Count; i++)
            {
                if(inventory[i].name == "Red")
                {
                    inventory.RemoveAt(i);
                    i -= 1;
                    UpdateQuotaText();
                }
            }
        }

        if (other.gameObject.name == "BluePile")
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i].name == "Blue")
                {
                    inventory.RemoveAt(i);
                    i -= 1;
                    UpdateQuotaText();
                }
            }
        }

        if (other.gameObject.name == "YellowPile" )
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i].name == "Yellow")
                {
                    inventory.RemoveAt(i);
                    i -= 1;
                    UpdateQuotaText();
                }
            }
        }

        if (other.gameObject.name == "GreenPile" )
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i].name == "Green")
                {
                    inventory.RemoveAt(i);
                    i -= 1;
                    UpdateQuotaText();
                }
            }
        }

        if (other.gameObject.CompareTag("Prisoner"))
        {
            PrisonerController prisoner = other.gameObject.GetComponent<PrisonerController>();

            if (!prisoner.questInProgress)
            {
                Quest q = prisoner.GetNextQuest();

                if (q != null)
                {
                    currentQuests.Add(q.questID);
                    dialogueUI.gameObject.SetActive(true);
                    dialogueText.text = q.acceptDialogue[0];
                    dialogueIndex = 0;
                    isTalking = true;
                    dialogueType = DialogueType.accepting;
                    currentQuestIndex = q.questID;
                }
                    
            }
            else if (CheckQuest(prisoner.activeQuest))
            {
                dialogueUI.gameObject.SetActive(true);
                dialogueText.text = prisoner.activeQuest.returnDialogue[0];
                dialogueIndex = 0;
                isTalking = true;
                dialogueType = DialogueType.returning;
                currentQuestIndex = prisoner.activeQuest.questID;
                currentQuests.Remove(prisoner.activeQuest.questID);
                RemoveQuestItem(prisoner.activeQuest.questItem);
                prisoner.ReturnQuest();
            }
            else if (!CheckQuest(prisoner.activeQuest))
            {
                dialogueUI.gameObject.SetActive(true);
                dialogueText.text = prisoner.activeQuest.standbyDialogue[0];
                dialogueIndex = 0;
                isTalking = true;
                dialogueType = DialogueType.standby;
            }
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Guard"))
        {
            health = 0;
        } 
    }

    private string ShowItem(Item item)
    {
        if (item.type == Item.ItemType.Valuable)
            ItemDetails = item.name + "\n" + "Type: Valuable\n Value: " + item.value;   

        if (item.type == Item.ItemType.Consumable)
            ItemDetails = item.name + "\n" + "Type: Consumable\n Health: " + item.value;

        return ItemDetails;
    }

    private bool CheckQuest(Quest quest)
    {
        if (currentQuests.Contains(quest.questID) && CheckInventory(quest.questItem))
            return true;
        else
            return false;
    }

    private void RemoveQuestItem(Item item)
    {
        for(int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].name == item.name)
            {
                inventory.RemoveAt(i);
                break;
            }   
        }
    }

    private bool CheckInventory(Item item)
    {
        foreach(Item i in inventory)
        {
            if (i.name == item.name)
                return true;
        }
        return false;
    }
    public void DisplayEndScreen()
    {
        showInventory = !showInventory;
        timerdecrement = 0;
        Time.timeScale = 0;
        transform.Rotate(0, 0, 0);
        Cursor.visible = true;
        

        if (depositQuota == 0)
        {
            if(Random.value > percentages[StealItems.Count])
            {
                VictoryScreenUI.gameObject.SetActive(true);
            }
            else
            {
                EndScreen1.gameObject.SetActive(true);
            }
        }
        else
        {
            EndScreen.gameObject.SetActive(true);
        }
    }
    public void OnRestart()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }

    public void OnMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void OnContinue()
    {
        isPaused = !isPaused;
        showInventory = !showInventory;
        timerdecrement = Time.fixedUnscaledDeltaTime;
        Time.timeScale = 1;
        transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivity, 0);
        
        Cursor.visible = !Cursor.visible;

        PauseScreenUI.gameObject.SetActive(isPaused);
    }


    public void OnQuit()
    {
#if UNITY_EDITOR
        if (Application.isEditor)
        {
            EditorApplication.isPlaying = false;
            return;
        }
#endif
        Application.Quit();
    }
}

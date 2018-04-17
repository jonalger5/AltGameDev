using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
//quick comment
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
    private float sprint;
    private bool isSprinting = false;
    private float sprintTimer;
    [SerializeField]
    private float sprintTimeOut;
    private bool sprintCoolDown = false;
    [SerializeField]
    public float sensitivity;
    private Text healthUI;

    internal static void PickUpStealthItem(Item item)
    {
        GameManager.gm.inventory.Add(item);
    }

    private Canvas deathScreenUI;
    private Canvas PauseScreenUI;

    //Dialogue Variables
    private Canvas dialogueUI;
    private Text dialogueText;
    private int dialogueIndex;
    private bool isTalking;
    private DialogueType dialogueType;

    //Quest Variables
    private int currentQuestIndex = 0;
    private Canvas questUI;
    private GameObject rollCallPoint;

    private new Renderer renderer;
    //public static List<Item> inventory;
    private ItemDatabase itemDatabase;
    private int slotX = 2, slotY = 3;
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

    [SerializeField]
    public bool isSortingGame;
    [SerializeField]
    private int depositQuota;
    [SerializeField]
    private float timer;
    private Text timerText;
    private Text quotaText;

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

    private Animator _anim;

    private bool bluecontact = false;
    private bool greencontact = false;
    private bool yellowcontact = false;

    // Use this for initialization
    void Start () {
        itemDatabase = GameObject.Find("Item Database").GetComponent<ItemDatabase>();
        healthUI = GameObject.Find("/HealthUI/Health").GetComponent<Text>();
        timerText = GameObject.Find("/HealthUI/Timer").GetComponent<Text>();
        quotaText = GameObject.Find("/HealthUI/Quota").GetComponent<Text>();
        dialogueUI = GameObject.Find("DialogueUI").GetComponent<Canvas>();
        dialogueText = GameObject.Find("/DialogueUI/DialogueText").GetComponent<Text>();
        dialogueUI.gameObject.SetActive(false);
        questUI = GameObject.Find("QuestUI").GetComponent<Canvas>();
        questUI.gameObject.SetActive(false);

        //deathScreenUI = GameObject.Find("DeathScreenUI").GetComponent<Canvas>();
        //deathScreenUI.gameObject.SetActive(false);

        //PauseScreenUI = GameObject.Find("PauseScreen").GetComponent<Canvas>();
        //PauseScreenUI.gameObject.SetActive(false);
        //isPaused = false;

        //VictoryScreenUI = GameObject.Find("VictoryScreen").GetComponent<Canvas>();
        //VictoryScreenUI.gameObject.SetActive(false);

        //EndScreen = GameObject.Find("LoseScreen1").GetComponent<Canvas>();
        //EndScreen.gameObject.SetActive(false);

        //EndScreen1 = GameObject.Find("LoseScreen2").GetComponent<Canvas>();
        //EndScreen1.gameObject.SetActive(false);

        //inventory = new List<Item>();

        StealItems = new List<Item>();
        Time.timeScale = 1;

        //deathScreenUI.gameObject.SetActive(false);
        //PauseScreenUI.gameObject.SetActive(false);
        //VictoryScreenUI.gameObject.SetActive(false);
        //EndScreen.gameObject.SetActive(false);
        //EndScreen1.gameObject.SetActive(false);

        if (isSortingGame)
        {            
            UpdateTimerText();
            UpdateQuotaText();
            timerdecrement = 0;
            timerdecrement = Time.fixedUnscaledDeltaTime;
        }
        else
        {
            timerText.gameObject.SetActive(false);
            quotaText.gameObject.SetActive(false);
            rollCallPoint = GameObject.Find("RollCallPoint");
            rollCallPoint.gameObject.SetActive(false);
        }

        Cursor.visible = false;

        NumOfItems = 6;
        percentages = new Dictionary<int, double>();
        percentages.Add(0, 0);
        percentages.Add(1, .05);
        percentages.Add(2, .10);
        percentages.Add(3, .25);
        percentages.Add(4, .50);
        percentages.Add(5, .75);
        percentages.Add(6, .95);

        _anim = GetComponent<Animator>();
    }

    void UpdateTimerText()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("SortingGame"))
        {
            Debug.Log("we did it");
            timerText.text = "Time Left: " + timer.ToString("F2");
        }
        else if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("StealthMiniGame"))
        {
            timerText.text = "Find Item";
        }
        else
        {
            timerText.text = "";
        }
    }

    void UpdateQuotaText()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("SortingGame"))
        {
            depositQuota--;
            if (depositQuota < 0)
            {
                depositQuota = 0;
            }
            quotaText.text = "Remaining Items: " + depositQuota.ToString();
        }
        else if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("StealthMiniGame"))
        {
            quotaText.text = "Don't Get Caught";
        }
        else
        {
            quotaText.text = "";
        }
    }
    // Update is called once per frame
    void Update ()
    {
        if (isSortingGame)
        {
            if (timer > 0.0f)
            {
                timer -= timerdecrement;
                if (timer < 0.0f)
                {
                    // DisplayEndScreen();
                    GameManager.gm.inventory.AddRange(StealItems);
                    GameManager.gm.AdvanceScene();
                    timer = 0.0f;
                }
                UpdateTimerText();
            }
        }
     
        //Activate Death Screen
        if (GameManager.gm.playerHealth <= 0)
        {
            healthUI.text = "0";
            //deathScreenUI.gameObject.SetActive(true);
            SceneManager.LoadScene(0);
            Time.timeScale = 0;
            Cursor.visible = !Cursor.visible;
        }
        //Updating HealthUI
        else
            healthUI.text = GameManager.gm.playerHealth.ToString();

        //Used for Movement
        if (!isTalking)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");           
            
            if(Input.GetKeyDown(KeyCode.V) && sprintTimer < sprintTimeOut)
            {
                isSprinting = true;
                sprintCoolDown = false;                              
            }
            if(Input.GetKeyUp(KeyCode.V))
            {
                isSprinting = false;
                sprintCoolDown = true;
            }

            if (isSprinting)
            {
                sprintTimer += Time.deltaTime;

                if (sprintTimer > sprintTimeOut)
                {
                    isSprinting = false;                   
                }

                transform.Translate(Vector3.forward * Time.deltaTime * v * sprint);
                transform.Translate(Vector3.right * Time.deltaTime * h * sprint);
            }
            else
            {              
                if (sprintTimer > 0 && sprintCoolDown)
                {
                    sprintTimer -= Time.deltaTime;
                }                   
                else if(sprintTimer < 0 && sprintCoolDown) 
                {
                    sprintCoolDown = false;
                    sprintTimer = 0;
                }

                transform.Translate(Vector3.forward * Time.deltaTime * v * speed);
                transform.Translate(Vector3.right * Time.deltaTime * h * speed);
                _anim.SetFloat("Walk", v);
            }            
        }
        else
            _anim.SetFloat("Walk", 0);

        if (!showInventory && !isTalking && !(bluecontact || greencontact || yellowcontact))
        {
            transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivity, 0);
            Cursor.visible = false;
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            showInventory = !showInventory;
            Cursor.visible = !Cursor.visible;
            if (!showInventory)
                questUI.gameObject.SetActive(false);
        }

        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    isPaused = !isPaused;
        //    showInventory = !showInventory;
        //    if (isPaused)
        //    {
        //        timerdecrement = 0;
        //        Time.timeScale = 0;
        //        transform.Rotate(0, 0, 0);
        //    }
        //    else if (!isPaused)
        //    {
        //        timerdecrement = Time.fixedUnscaledDeltaTime;
        //        Time.timeScale = 1;
        //        transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivity, 0);
        //    }
        //    Cursor.visible = !Cursor.visible;
            
        //    //PauseScreenUI.gameObject.SetActive(isPaused);
        //}
        if(bluecontact || greencontact || yellowcontact)
        {
            transform.Rotate(0, 0, 0);
            Cursor.visible = true;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isTalking)
        {
            switch (dialogueType)
            {
                case DialogueType.accepting:
                    
                    if (dialogueIndex != GameManager.gm.qdInstance.Quests[currentQuestIndex].acceptDialogue.Count - 1)
                    {
                        dialogueIndex++;
                        dialogueText.text = GameManager.gm.qdInstance.Quests[currentQuestIndex].acceptDialogue[dialogueIndex];
                    }
                    else
                    {
                        dialogueUI.gameObject.SetActive(false);
                        isTalking = false;
                        rollCallPoint.gameObject.SetActive(true);
                    }
                        
                    break;

                case DialogueType.returning:
                    if (dialogueIndex != GameManager.gm.qdInstance.Quests[currentQuestIndex].returnDialogue.Count - 1)
                    {
                        dialogueIndex++;
                        dialogueText.text = GameManager.gm.qdInstance.Quests[currentQuestIndex].returnDialogue[dialogueIndex];
                    }
                    else
                    {
                        dialogueUI.gameObject.SetActive(false);
                        isTalking = false;
                    }

                    break;

                case DialogueType.standby:
                    if (dialogueIndex != GameManager.gm.qdInstance.Quests[currentQuestIndex].standbyDialogue.Count - 1)
                    {
                        dialogueIndex++;
                        dialogueText.text = GameManager.gm.qdInstance.Quests[currentQuestIndex].standbyDialogue[dialogueIndex];
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
        GameManager.gm.inventory.RemoveAt(position);
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

        
        if (showInventory || isSortingGame)
        {
            //GUI.Box(new Rect(10, 50, 240, 240), InventoryBackground);
            for (int x = 0; x < slotX; x++)
            {
                for (int y = 0; y < slotY; y++)
                {
                    Position = x * slotY + y;
                    if (Position < GameManager.gm.inventory.Count)
                    {
                        Rect slot = new Rect(Screen.width / 100 + y * 60, Screen.height / 10 + x * 60, 50, 50);
                        GUI.Box(slot, GameManager.gm.inventory[Position].icon);
                        if (slot.Contains(Event.current.mousePosition))
                        {
                            ItemDetails = ShowItem(GameManager.gm.inventory[Position]);
                            showItem = true;


                            if (Input.GetMouseButtonDown(0) && CanAccess && bluecontact)
                            {
                                CanAccess = !CanAccess;

                                if (GameManager.gm.inventory[Position].type.ToString() == "Consumable")
                                {
                                    Remove(Position);
                                    UpdateQuotaText();
                                }
                                showItem = false;

                            }
                            if (Input.GetMouseButtonDown(0) && CanAccess && yellowcontact)
                            {
                                CanAccess = !CanAccess;
                                if (GameManager.gm.inventory[Position].type.ToString() == "Valuable")
                                {
                                    Remove(Position);
                                    UpdateQuotaText();
                                }
                                showItem = false;

                            }
                            if (Input.GetMouseButtonDown(0) && CanAccess && greencontact)
                            {
                                CanAccess = !CanAccess;
                                if (GameManager.gm.inventory[Position].type.ToString() == "Other")
                                {
                                    Remove(Position);
                                    UpdateQuotaText();
                                }
                                showItem = false;

                            }



                            if (Input.GetMouseButtonDown(0) && CanAccess && CanAccess && !greencontact && !yellowcontact && !bluecontact)
                            {
                                CanAccess = !CanAccess;
                                isStealing = true;
                                StealItems.Add(GameManager.gm.inventory[Position]);
                                Remove(Position);
                                NumOfItems--;
                                showItem = false;

                            }

                            if (Input.GetButtonUp("space") && GameManager.gm.inventory[Position].type == Item.ItemType.Consumable && GameManager.gm.playerHealth < 100 && CanAccess)
                            {
                                CanAccess = !CanAccess;
                                GameManager.gm.playerHealth += GameManager.gm.inventory[Position].value;
                                Remove(Position);
                                showItem = false;
                            }
                        }

                        if (ItemDetails == "")
                            showItem = false;
                    }
                    else
                    {
                        Rect slot = new Rect(Screen.width / 100 + y * 60, Screen.height / 10 + x * 60, 50, 50);
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

                            if (Input.GetMouseButtonDown(0))
                            {
                                CanAccess = !CanAccess;
                                GameManager.gm.inventory.Add(StealItems[Position1]);
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
        }

        if (showInventory)
        {
            questUI.gameObject.SetActive(true);
            GUILayout.BeginArea(new Rect(4 * Screen.width / 5, Screen.height / 5, 170, 300));
            for (int i = 0; i < GameManager.gm.currentQuests.Count; i++)
            {

                if (CheckQuest(GameManager.gm.qdInstance.Quests[GameManager.gm.currentQuests[i]]))
                {
                    GUILayout.Label(GameManager.gm.qdInstance.Quests[GameManager.gm.currentQuests[i]].desc, GUI.skin.customStyles[1], GUILayout.MaxWidth(170));
                }

                else
                {
                    GUILayout.Label(GameManager.gm.qdInstance.Quests[GameManager.gm.currentQuests[i]].desc, GUI.skin.customStyles[2], GUILayout.MaxWidth(170));
                }
            }
            GUILayout.EndArea();
        }

        if (showItem)
        {
            GUI.Box(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 150, 50), ItemDetails);
        }
        
    }
    void OnTriggerExit(Collider other)
    {


        if (other.gameObject.name == "BluePile")
        {

            Cursor.visible = false;
            bluecontact = false;
            /*
            for (int i = 0; i < GameManager.gm.inventory.Count; i++)
            {
                if (GameManager.gm.inventory[i].type.ToString() == "Consumable")
                {
                    GameManager.gm.inventory.RemoveAt(i);
                    i -= 1;
                    UpdateQuotaText();
                }
            }
            */
        }

        if (other.gameObject.name == "YellowPile")
        {
            Cursor.visible = false;
            yellowcontact = false;
        }

        if (other.gameObject.name == "GreenPile")
        {
            Cursor.visible = false;
            greencontact = false;
        }

    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "PileOfItems")
        {
            int PickupNum = NumOfItems - GameManager.gm.inventory.Count;
            for (int i = 0;i < PickupNum; i++)
            {
                itemnum = UnityEngine.Random.Range(0, 5);
                GameManager.gm.inventory.Add(itemDatabase.Items[itemnum]);
            }

        }
        

        if (other.gameObject.name == "BluePile")
        {

            Cursor.visible = true;
            bluecontact = true;
            /*
            for (int i = 0; i < GameManager.gm.inventory.Count; i++)
            {
                if (GameManager.gm.inventory[i].type.ToString() == "Consumable")
                {
                    GameManager.gm.inventory.RemoveAt(i);
                    i -= 1;
                    UpdateQuotaText();
                }
            }
            */
        }

        if (other.gameObject.name == "YellowPile" )
        {
            Cursor.visible = true;
            yellowcontact = true;
        }

        if (other.gameObject.name == "GreenPile" )
        {
            Cursor.visible = true;
            greencontact = true;
        }

        if (other.gameObject.CompareTag("Prisoner"))
        {
            PrisonerController prisoner = other.gameObject.GetComponent<PrisonerController>();

            if (!prisoner.hasReturnedQuest)
            {
                if (prisoner.activeQuest.questItem.name == "")
                {
                    Quest q = prisoner.GetNextQuest();

                    if (q != null)
                    {
                        GameManager.gm.currentQuests.Add(q.questID);
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
                    GameManager.gm.currentQuests.Remove(prisoner.activeQuest.questID);
                    RemoveQuestItem(prisoner.activeQuest.questItem);
                    prisoner.ReturnQuest();
                }
                else if (!CheckQuest(prisoner.activeQuest) && !prisoner.activeQuest.complete)
                {
                    dialogueUI.gameObject.SetActive(true);
                    dialogueText.text = prisoner.activeQuest.standbyDialogue[0];
                    dialogueIndex = 0;
                    isTalking = true;
                    dialogueType = DialogueType.standby;
                }
            }
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Guard"))
        {
            GameManager.gm.playerHealth = 0;
        } 

        if (other.gameObject.CompareTag("StealthItem"))
        {
            GameManager.gm.AdvanceScene();
            //Destroy(other.collider.gameObject);
        }
        if (other.gameObject.CompareTag("RollCallPoint"))
        {
            GameManager.gm.AdvanceScene();

        }
    }

    private string ShowItem(Item item)
    {
        switch (item.type)
        {
            case Item.ItemType.Clothing:
                ItemDetails = item.name + "\n" + "Type: Clothing";
                break;

            case Item.ItemType.Consumable:
                ItemDetails = item.name + "\n" + "Type: Consumable\n Health: " + item.value;
                break;

            case Item.ItemType.Documents:
                ItemDetails = item.name + "\n" + "Type: Documents";
                break;

            case Item.ItemType.EscapePlan:
                ItemDetails = item.name + "\n" + "Type: Escape Plan";
                break;

            case Item.ItemType.Valuable:
                ItemDetails = item.name + "\n" + "Type: Valuable\n Value: " + item.value;
                break;

            case Item.ItemType.Other:
                ItemDetails = item.name + "\n" + "Type: Other";
                break;

            default:
                break;

        }

        return ItemDetails;
    }

    private bool CheckQuest(Quest quest)
    {
        if (GameManager.gm.currentQuests.Contains(quest.questID) && CheckInventory(quest.questItem))
            return true;
        else
            return false;
    }

    private void RemoveQuestItem(Item item)
    {
        for(int i = 0; i < GameManager.gm.inventory.Count; i++)
        {
            if (GameManager.gm.inventory[i].name == item.name)
            {
                GameManager.gm.inventory.RemoveAt(i);
                break;
            }   
        }
    }

    private bool CheckInventory(Item item)
    {
        foreach(Item i in GameManager.gm.inventory)
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
            if(UnityEngine.Random.value > percentages[StealItems.Count])
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
        GameManager.gm.playerHealth = 100f;
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

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
    private Image sprintMeter;
    private Image healthMeter;
    [SerializeField]
    public float sensitivity;
    

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
    public Text[] questUIText;

    //Support/Tutorial Variables
    private GameObject rollCallPoint;
    private GameObject rollCall;
    private Canvas supportUI;
    private Text supportText;

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
    public GameObject actionMeterFull;
    private Image actionMeter;

    [SerializeField]
    public bool isSortingGame;
    [SerializeField]
    public bool isStealthGame;
    [SerializeField]
    private int depositQuota;
    [SerializeField]
    private float timer;
    private Text timerText;
    private Text quotaText;

    private bool isPaused;
    private float timerdecrement;

    private int NumOfItems = 6;
    private int Position;
    private int Position1;
    private bool CanAccess = true;

    private Canvas VictoryScreenUI;
    private Canvas EndScreen;
    private Canvas EndScreen1;

    private Dictionary<int, double> percentages;

    private Animator _anim;

    private bool otherContact = false;
    private bool valuableContact = false;
    private bool clothingcontact = false;
    private bool Consumablecontact = false;
    private bool Documentcontact = false;

    private bool firstSorting;

    // Use this for initialization
    void Start () {
        itemDatabase = GameObject.Find("Item Database").GetComponent<ItemDatabase>();
        healthMeter = GameObject.Find("/HealthUI/Health").GetComponent<Image>();
        sprintMeter = GameObject.Find("/HealthUI/Sprint").GetComponent<Image>();
        actionMeterFull = GameObject.Find("/HealthUI/ActionMeter");
        actionMeter = GameObject.Find("/HealthUI/ActionMeter/Action").GetComponent<Image>();
        actionMeterFull.SetActive(false);
        timerText = GameObject.Find("/HealthUI/Timer").GetComponent<Text>();
        quotaText = GameObject.Find("/HealthUI/Quota").GetComponent<Text>();
        
        dialogueUI = GameObject.Find("DialogueUI").GetComponent<Canvas>();
        dialogueText = GameObject.Find("/DialogueUI/DialogueText").GetComponent<Text>();
        dialogueUI.gameObject.SetActive(false);
        questUI = GameObject.Find("QuestUI").GetComponent<Canvas>();
        questUIText = GameObject.Find("/QuestUI/Quest Text").GetComponentsInChildren<Text>();
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

        StealItems = new List<Item>();
        Time.timeScale = 1;

        firstSorting = true;
        Cursor.visible = false;

        if (isSortingGame)
        {            
            UpdateTimerText();
            UpdateQuotaText();
            timerdecrement = 0;
            timerdecrement = Time.fixedUnscaledDeltaTime;
            GameManager.gm.hasReceivedQuest = false;
            Cursor.visible = false;
        }
        else if(isStealthGame)
        {
            UpdateTimerText();
            UpdateQuotaText();
            rollCallPoint = GameObject.Find("RollCallPoint");
            rollCallPoint.gameObject.SetActive(false);
            GameManager.gm.hasReceivedQuest = false;
        }
        else
        {
            rollCall = GameObject.Find("RollCall - PI");
            rollCall.SetActive(false);
            timerText.gameObject.SetActive(false);
            quotaText.gameObject.SetActive(false);
            supportUI = GameObject.Find("SupportUI").GetComponent<Canvas>();
            supportText = GameObject.Find("/SupportUI/Support Text").GetComponent<Text>();
            supportUI.gameObject.SetActive(false);

            if (GameManager.gm.hasReceivedQuest)
                rollCall.SetActive(true);
        }

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
        if(isSortingGame && firstSorting)
        {
            timerText.text = "Grab items from large pile and sort items by item type";
            LetsWait(10);
            firstSorting = false;
        }
        if (isSortingGame)
        {
            timerText.text = "Time Left: " + timer.ToString("F2");
        }
        else if (isStealthGame)
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
        if (isSortingGame)
        {
            depositQuota--;
            if (depositQuota < 0)
            {
                depositQuota = 0;
            }
            quotaText.text = "Remaining Items: " + depositQuota.ToString();
        }
        else if (isStealthGame)
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
            /*
            if (firstSorting)
            {
                firstSorting = false;
                dialogueUI.gameObject.SetActive(true);
                dialogueText.text = "test";

            }
            */
        }
        
     
        //if(depositQuota == 0)
        //{
            
        //    DisplayEndScreen();
        //}
        //Activate Death Screen
        if (GameManager.gm.playerHealth <= 0)
        {
            GameManager.gm.playerHealth = 100;
            //deathScreenUI.gameObject.SetActive(true);
            GameManager.gm.ReloadScene();
            Time.timeScale = 0;
            Cursor.visible = true;
        }

        //Setting Health and Sprint Meter
        healthMeter.rectTransform.localScale = new Vector3(GameManager.gm.playerHealth / 100, 1, 1);
        sprintMeter.rectTransform.localScale = new Vector3((sprintTimeOut - sprintTimer) / sprintTimeOut, 1, 1);

        //Used for Movement
        if (!isTalking)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");            
            
            if(Input.GetKeyDown(KeyCode.LeftShift) && sprintTimer < sprintTimeOut)
            {
                isSprinting = true;
                sprintCoolDown = false;                              
            }
            if(Input.GetKeyUp(KeyCode.LeftShift))
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
                //_anim.SetFloat("Walk", v);
            }            
        }
        else
            _anim.SetFloat("Walk", 0);

        /*
        if (!showInventory && !isTalking && !(Consumablecontact || otherContact || valuableContact || clothingcontact || Documentcontact ))
        {
            transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivity, 0);
            //Cursor.visible = false;
        }
        */

        if (Input.GetKeyDown(KeyCode.I))
        {
            showInventory = !showInventory;
            //Cursor.visible = !Cursor.visible;

            //Shows Quest Log
            if (showInventory)
            {
                questUI.gameObject.SetActive(true);
                ResetQuestUIText();

                for (int i = 0; i < GameManager.gm.currentQuests.Count; i++)
                {

                    if (CheckQuest(GameManager.gm.qdInstance.Quests[GameManager.gm.currentQuests[i]]))
                    {
                        questUIText[i].text = GameManager.gm.qdInstance.Quests[GameManager.gm.currentQuests[i]].desc;
                        questUIText[i].color = Color.green;
                    }

                    else
                    {
                        questUIText[i].text = GameManager.gm.qdInstance.Quests[GameManager.gm.currentQuests[i]].desc;
                        questUIText[i].color = Color.red;
                    }
                }
            }
            else
            {
                questUI.gameObject.SetActive(false);
                showItem = false;
            }            
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
                Cursor.visible = true;                
            }
            else if (!isPaused)
            {
                timerdecrement = Time.fixedUnscaledDeltaTime;
                Time.timeScale = 1;
                transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivity, 0);
                Cursor.visible = false;
            }
            PauseScreenUI.gameObject.SetActive(isPaused);
        }
        if (Input.GetMouseButtonDown(1))
        {
            Cursor.visible = !Cursor.visible;
        }
        if (Cursor.visible)
        {
            transform.Rotate(0, 0, 0);

        }
        if (!Cursor.visible)
        {
            transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivity, 0);
            timerdecrement = Time.fixedUnscaledDeltaTime;
            Time.timeScale = 1;
            showInventory = false;
            isPaused = false;
        }
        
        if (Consumablecontact || otherContact || valuableContact || clothingcontact || Documentcontact )
        {
            transform.Rotate(0, 0, 0);
            //Cursor.visible = true;
        }
        

        if (Input.GetKeyDown(KeyCode.Space) && isTalking)
        {
            sprintTimer = 0;
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
                        rollCall.gameObject.SetActive(true);
                        GameManager.gm.hasReceivedQuest = true;
                        StartCoroutine(GameManager.gm.RollCall(supportUI, supportText));
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
            actionMeterFull.gameObject.SetActive(true);
            actionMeter.rectTransform.localScale = new Vector3((_stealTimeout - stealTimer) / _stealTimeout, 1, 1);
            stealTimer += Time.deltaTime;
            if (stealTimer >= _stealTimeout)
            {
                actionMeterFull.gameObject.SetActive(false);
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

        //Shows both Inventory and Sorting Items
        if (isSortingGame)
        {
            //GUI.Box(new Rect(10, 50, 240, 240), InventoryBackground);
            for (int x = 0; x < slotX; x++)
            {
                for (int y = 0; y < slotY; y++)
                {
                    Position = x * slotY + y;
                    if (Position < GameManager.gm.inventory.Count)
                    {
                        Rect slot = new Rect(Screen.width / 100 + y * 60, Screen.height / 3 + x * 60, 50, 50);
                        GUI.Box(slot, GameManager.gm.inventory[Position].icon);
                        if (slot.Contains(Event.current.mousePosition))
                        {
                            ItemDetails = ShowItem(GameManager.gm.inventory[Position]);
                            showItem = true;


                            if (Input.GetMouseButtonDown(0) && CanAccess && Consumablecontact)
                            {
                                CanAccess = !CanAccess;

                                if (GameManager.gm.inventory[Position].type.ToString() == "Consumable")
                                {
                                    Remove(Position);
                                    LetsWait(1);
                                    UpdateQuotaText();
                                    showItem = false;
                                }
                                else
                                {
                                    GameManager.gm.playerHealth -= 5;
                                    CanAccess = true;

                                }

                            }
                            if (Input.GetMouseButtonDown(0) && CanAccess && valuableContact)
                            {
                                CanAccess = !CanAccess;
                                if (GameManager.gm.inventory[Position].type.ToString() == "Valuable")
                                {
                                    Remove(Position);
                                    LetsWait(1);
                                    UpdateQuotaText();
                                }
                                else
                                {
                                    GameManager.gm.playerHealth -= 5;
                                    CanAccess = true;
                                }
                                showItem = false;

                            }
                            if (Input.GetMouseButtonDown(0) && CanAccess && otherContact)
                            {
                                CanAccess = !CanAccess;
                                if (GameManager.gm.inventory[Position].type.ToString() == "Other")
                                {
                                    Remove(Position);
                                    LetsWait(1);
                                    UpdateQuotaText();
                                }
                                else
                                {
                                    GameManager.gm.playerHealth -= 5;
                                    CanAccess = true;
                                }
                                showItem = false;

                            }
                            if (Input.GetMouseButtonDown(0) && CanAccess && Documentcontact)
                            {
                                CanAccess = !CanAccess;

                                if (GameManager.gm.inventory[Position].type.ToString() == "Documents")
                                {
                                    Remove(Position);
                                    LetsWait(1);
                                    UpdateQuotaText();
                                }
                                else
                                {
                                    GameManager.gm.playerHealth -= 5;
                                    CanAccess = true;
                                }
                                showItem = false;

                            }
                            if (Input.GetMouseButtonDown(0) && CanAccess && clothingcontact)
                            {
                                CanAccess = !CanAccess;
                                if (GameManager.gm.inventory[Position].type.ToString() == "Clothing")
                                {
                                    Remove(Position);
                                    LetsWait(1);
                                    UpdateQuotaText();
                                }
                                else
                                {
                                    GameManager.gm.playerHealth -= 5;
                                    CanAccess = true;
                                }
                                showItem = false;

                            }


                            if (Input.GetMouseButtonDown(0) && CanAccess && !Consumablecontact && !otherContact && !valuableContact && !clothingcontact && !Documentcontact)
                            {
                                CanAccess = !CanAccess;
                                isStealing = true;
                                stealTimer = 0;
                                StealItems.Add(GameManager.gm.inventory[Position]);
                                Remove(Position);
                                LetsWait(1);
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
                        Rect slot = new Rect(Screen.width / 100 + y * 60, Screen.height / 3 + x * 60, 50, 50);
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
                        Rect slot = new Rect(Screen.width / 100 + j * 60, Screen.height / 10 + i * 60, 50, 50);
                        GUI.Box(slot, StealItems[Position1].icon);
                        if (slot.Contains(Event.current.mousePosition))
                        {
                            ItemDetails = ShowItem(StealItems[Position1]);
                            showItem = true;

                            if (Input.GetMouseButtonDown(0) && CanAccess)
                            {
                                CanAccess = !CanAccess;
                                GameManager.gm.inventory.Add(StealItems[Position1]);
                                RemoveSteal(Position1);
                                LetsWait(1);
                                NumOfItems++;
                                showItem = false;

                            }
                        }

                        if (ItemDetails == "")
                            showItem = false;
                    }
                    else
                    {
                        Rect slot = new Rect(Screen.width / 100 + j * 60, Screen.height / 10 + i * 60, 50, 50);
                        GUI.Box(slot, EmptySlot);
                    }
                }
            }
        }

        if (showInventory && !isSortingGame)
        {
            for (int x = 0; x < slotX; x++)
            {
                for (int y = 0; y < slotY; y++)
                {
                    Position = x * slotY + y;
                    if (Position < GameManager.gm.inventory.Count)
                    {
                        Rect slot = new Rect(Screen.width / 100 + y * 60, Screen.height / 3 + x * 60, 50, 50);
                        GUI.Box(slot, GameManager.gm.inventory[Position].icon);
                        if (slot.Contains(Event.current.mousePosition))
                        {
                            ItemDetails = ShowItem(GameManager.gm.inventory[Position]);
                            showItem = true;

                            if (Input.GetButtonUp("space") && GameManager.gm.inventory[Position].type == Item.ItemType.Consumable && GameManager.gm.playerHealth < 100 && CanAccess)
                            {
                                CanAccess = !CanAccess;
                                GameManager.gm.playerHealth += GameManager.gm.inventory[Position].value;
                                LetsWait(1);
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
        }

        if (showItem)
        {
            GUI.Box(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 150, 50), ItemDetails);
        }
        
    }
    void OnTriggerExit(Collider other)
    {

        if (other.gameObject.name == "ConsumablePile")
        {

            //Cursor.visible = false;
            Consumablecontact = false;
        }

        if (other.gameObject.name == "OtherPile")
        {
            //Cursor.visible = false;
            otherContact = false;
        }

        if (other.gameObject.name == "ValuablePile")
        {
            //Cursor.visible = false;
            valuableContact = false;
        }

        if (other.gameObject.name == "ClothingPile")
        {
            //Cursor.visible = false;
            clothingcontact = false;
        }

        if (other.gameObject.name == "DocumentPile")
        {
            //Cursor.visible = false;
            Documentcontact = false;
        }
        
    }
        void OnTriggerEnter(Collider other)
        {
        if (other.gameObject.name == "PileOfItems")
        {
            //Debug.Log(isSortingGame);
            int PickupNum = NumOfItems - GameManager.gm.inventory.Count;
            for (int i = 0;i < PickupNum; i++)
            {
                itemnum = UnityEngine.Random.Range(0, 25);
                GameManager.gm.inventory.Add(itemDatabase.Items[itemnum]);
            }

        }


        if (other.gameObject.name == "ConsumablePile")
        {

           // Cursor.visible = true;
            Consumablecontact = true;
        }

        if (other.gameObject.name == "OtherPile")
        {
            //Cursor.visible = true;
            otherContact = true;
        }

        if (other.gameObject.name == "ValuablePile")
        {
           // Cursor.visible = true;
            valuableContact = true;
        }

        if (other.gameObject.name == "ClothingPile")
        {

           // Cursor.visible = true;
            clothingcontact = true;
        }

        if (other.gameObject.name == "DocumentPile")
        {
           // Cursor.visible = true;
            Documentcontact = true;
        }
        

        if (other.gameObject.CompareTag("Prisoner"))
        {
            PrisonerController prisoner = other.gameObject.GetComponent<PrisonerController>();

            if (!prisoner.hasReturnedQuest)
            {
                if (prisoner.activeQuest.questItem.name != "" && !prisoner.activeQuest.inProgress)
                {
                    if (prisoner.activeQuest != null && GameManager.gm.currentQuests.Count < 5)
                    {
                        GameManager.gm.currentQuests.Add(prisoner.activeQuest.questID);
                        dialogueUI.gameObject.SetActive(true);
                        dialogueText.text = prisoner.activeQuest.acceptDialogue[0];
                        dialogueIndex = 0;
                        isTalking = true;
                        dialogueType = DialogueType.accepting;
                        currentQuestIndex = prisoner.activeQuest.questID;
                        GameManager.gm.qdInstance.Quests[prisoner.activeQuest.questID].inProgress = true;
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
                    ResetQuestUIText();
                }
                //else if (!CheckQuest(prisoner.activeQuest) && !prisoner.activeQuest.complete)
                //{
                //    dialogueUI.gameObject.SetActive(true);
                //    dialogueText.text = prisoner.activeQuest.standbyDialogue[0];
                //    dialogueIndex = 0;
                //    isTalking = true;
                //    dialogueType = DialogueType.standby;
                //}
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

    public bool CheckQuest(Quest quest)
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

    private void ResetQuestUIText()
    {
        foreach (Text t in questUIText)
            t.text = "";
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
            depositQuota = -1;
            if(UnityEngine.Random.value > percentages[StealItems.Count])
            {
                VictoryScreenUI.gameObject.SetActive(true);
                GameManager.gm.inventory.AddRange(StealItems);
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
        GameManager.gm.playerHealth = 100f;
        GameManager.gm.ReloadScene();
    }

    public void OnMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void onNextScene()
    {
        GameManager.gm.AdvanceScene();
    }
    public void OnContinue()
    {
        isPaused = false;
        showInventory = !showInventory;
        timerdecrement = Time.fixedUnscaledDeltaTime;
        Time.timeScale = 1;
        transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivity, 0);
        Cursor.visible = false;
        PauseScreenUI = GameObject.Find("PauseScreen").GetComponent<Canvas>();
        PauseScreenUI.gameObject.SetActive(false);
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

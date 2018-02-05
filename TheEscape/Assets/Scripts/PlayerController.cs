using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    [SerializeField]
    public float speed;
    [SerializeField]
    public float sensitivity;
    [SerializeField]
    public float health;
    private Text healthUI;

    private Renderer renderer;

    public List<Item> inventory;
    private int slotX = 4, slotY = 4;
    private ItemDatabase database;
    private bool showInventory = false;
    private bool showItem = false;
    private string ItemDetails;
    public Texture2D InventoryBackground;
    public Texture2D EmptySlot;
    public GUISkin slotBackground;

    public List<Item> Pickup;
    private bool onPickup = false;
    private int itemnum;
    private int quantity;
    private bool showPickup = false;
    
    private bool redpickup = false;
    private bool bluepickup = false;
    private bool yellowpickup = false;
    private bool greenpickup = false;

    // Use this for initialization
    void Start () {
        healthUI = GameObject.Find("/HealthUI/Health").GetComponent<Text>();
        renderer = GetComponent<Renderer>();
        renderer.material.color = Color.yellow;
        inventory = new List<Item>();
        database = GameObject.Find("Item Database").GetComponent<ItemDatabase>();

        Pickup = new List<Item>();

        //Used for testing inventory
        inventory.Add(database.Items[0]);
        inventory.Add(database.Items[1]);
        inventory.Add(database.Items[2]);
        inventory.Add(database.Items[3]);
        inventory.Add(database.Items[0]);
        inventory.Add(database.Items[1]);
        inventory.Add(database.Items[2]);
        inventory.Add(database.Items[3]);
        inventory.Add(database.Items[0]);
        inventory.Add(database.Items[1]);
        inventory.Add(database.Items[2]);
    }
	
	// Update is called once per frame
	void Update () {

        //Used for Movement
        transform.Translate(Vector3.forward * Time.deltaTime * Input.GetAxis("Vertical") * speed);
        transform.Translate(Vector3.right * Time.deltaTime * Input.GetAxis("Horizontal") * speed);

        if (!showInventory)
            transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivity, 0);

        //Updating HealthUI
        healthUI.text = health.ToString();

        if (Input.GetKeyDown(KeyCode.I))
        {
            showInventory = !showInventory;
        }
    }

    void OnGUI()
    {
        //GUI.skin = slotBackground;
        ItemDetails = "";
        
        if (showInventory)
        {
            //GUI.Box(new Rect(10, 50, 240, 240), InventoryBackground);
            for (int x = 0; x < slotX; x++)
            {
                for (int y = 0; y < slotY; y++)
                {                    
                    if (x * slotX + y + 1 <= inventory.Count)
                    {
                        Rect slot = new Rect(10 + y * 60, 50 + x * 60, 50, 50);
                        GUI.Box(slot, inventory[x * slotX + y].icon);
                        if (slot.Contains(Event.current.mousePosition))
                        {
                            ItemDetails = ShowItem(inventory[x * slotX + y]);
                            showItem = true;
                        }

                        if (ItemDetails == "")
                            showItem = false;
                    }
                    else
                    {
                        Rect slot = new Rect(10 + y * 60, 50 + x * 60, 50, 50);
                        GUI.Box(slot, EmptySlot);
                    }                  
                }
            }
        }

        if (showItem)
        {
            GUI.Box(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 150, 50), ItemDetails);
        }

        if (onPickup)
        {
            //GUI.Box(new Rect(10, 50, 240, 240), InventoryBackground);
            for (int x = 0; x < 1; x++)
            {
                for (int y = 0; y < quantity; y++)
                {
                    if (x * slotX + y + 1 <= Pickup.Count)
                    {
                        Rect slot = new Rect(900 + y * 60, 20 + x * 60, 50, 50);
                        GUI.Box(slot, Pickup[x * slotX + y].icon);
                        if (slot.Contains(Event.current.mousePosition))
                        {
                            ItemDetails = ShowItem(Pickup[x * slotX + y]);
                            showPickup = true;
                        }

                        if (ItemDetails == "")
                            showPickup = false;
                    }
                    else
                    {
                        Rect slot = new Rect(10 + y * 60, 50 + x * 60, 50, 50);
                        GUI.Box(slot, EmptySlot);
                    }
                }
            }
        }

        if (showPickup)
        {
            GUI.Box(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 150, 50), ItemDetails);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "PileOfItems" && !onPickup)
        {

            itemnum = Random.Range(0, 3);
            quantity = Random.Range(1, 4);
            onPickup = true;

            if(itemnum == 0)
            {
                redpickup = true;
            }

            else if (itemnum == 1)
            {
                yellowpickup = true;
            }

            else if (itemnum == 2)
            {
                greenpickup = true;
            }

            else if (itemnum == 3)
            {
                bluepickup = true;
            }

            for (int i = 0;i <= quantity; i++)
            {
                Pickup.Add(database.Items[itemnum]);
            }

        }

        if(other.gameObject.name == "RedPile" && onPickup && redpickup)
        {
            Pickup.Clear();
            onPickup = false;
        }

        if (other.gameObject.name == "BluePile" && onPickup && bluepickup)
        {
            Pickup.Clear();
            onPickup = false;
        }

        if (other.gameObject.name == "YellowPile" && onPickup && yellowpickup)
        {
            Pickup.Clear();
            onPickup = false;
        }

        if (other.gameObject.name == "GreenPile" && onPickup && greenpickup)
        {
            Pickup.Clear();
            onPickup = false;
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    [SerializeField]
    public float speed;
    [SerializeField]
    public float health;
    private Text healthUI;

    private Renderer renderer;

    public List<Item> inventory;
    private int slotX = 4, slotY = 4;
    //private int[] slots;
    private ItemDatabase database;
    private bool showInventory = false;
    private bool showItem = false;
    private string ItemDetails;

    // Use this for initialization
    void Start () {
        healthUI = GameObject.Find("/HealthUI/Health").GetComponent<Text>();
        renderer = GetComponent<Renderer>();
        renderer.material.color = Color.yellow;
        inventory = new List<Item>();
        //slots = new int[slotX * slotY];
        database = GameObject.Find("Item Database").GetComponent<ItemDatabase>();

        //Used for testing inventory
        //inventory.Add(database.Items[0]);
        //inventory.Add(database.Items[1]);
        //inventory.Add(database.Items[2]);
        //inventory.Add(database.Items[3]);
        //inventory.Add(database.Items[0]);
        //inventory.Add(database.Items[1]);
        //inventory.Add(database.Items[2]);
        //inventory.Add(database.Items[3]);
        //inventory.Add(database.Items[0]);
        //inventory.Add(database.Items[1]);
        //inventory.Add(database.Items[2]);
        //inventory.Add(database.Items[3]);
        //inventory.Add(database.Items[0]);
        //inventory.Add(database.Items[1]);
        //inventory.Add(database.Items[2]);
        //inventory.Add(database.Items[3]);
    }
	
	// Update is called once per frame
	void Update () {

        //Used for Movement
        transform.Translate(Vector3.forward * Time.deltaTime * Input.GetAxis("Vertical") * speed);
        transform.Translate(Vector3.right * Time.deltaTime * Input.GetAxis("Horizontal") * speed);

        //Updating HealthUI
        healthUI.text = health.ToString();

        if (Input.GetKeyDown(KeyCode.I))
        {
            showInventory = !showInventory;
        }
    }

    void OnGUI()
    {
        ItemDetails = "";

        if (showInventory)
        {
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
                }
            }
        }

        if (showItem)
        {
            GUI.Box(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 150, 50), ItemDetails);
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

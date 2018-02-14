﻿using System.Collections;
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

    private int deleteloc;
    private bool del = false;
    [SerializeField]
    private float _stealTimeout;
    public float stealTimer = 0;
    public bool isStealing = false;

    // Use this for initialization
    void Start () {
        healthUI = GameObject.Find("/HealthUI/Health").GetComponent<Text>();
        renderer = GetComponent<Renderer>();
        renderer.material.color = Color.yellow;
        inventory = new List<Item>();
        database = GameObject.Find("Item Database").GetComponent<ItemDatabase>();

        Pickup = new List<Item>();
        
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

        if (quantity == 0)
        {
            onPickup = false;
            showPickup = false;
        }
        if (del)
        {
            inventory.Add(Pickup[deleteloc]);
            Pickup.RemoveAt(deleteloc);
            quantity--;
            del = false;
            isStealing = true;
            stealTimer = 0;
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
                        Rect slot = new Rect(Screen.width/100 + y * 60, Screen.height / 10 + x * 60, 50, 50);
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
                        Rect slot = new Rect(Screen.width/100 + y * 60, Screen.height/10 + x * 60, 50, 50);
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

            for (int y = 0; y < quantity; y++)
            {
                if (y < quantity)
                {
                    Rect slot = new Rect((Screen.width) / 2 + y * 60, Screen.height / 100, 50, 50);
                    GUI.Box(slot, Pickup[y].icon);
                    if (slot.Contains(Event.current.mousePosition))
                    {
                        ItemDetails = ShowItem(Pickup[y]);
                        showPickup = true;

                        if (Input.GetButtonUp("space") && inventory.Count <= 16)
                        {
                            deleteloc = y;
                            del = true;

                        }
                    }

                    if (ItemDetails == "")
                        showPickup = false;
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

            quantity = Random.Range(3, 6);
            onPickup = true;


            for (int i = 0;i <= quantity; i++)
            {
                itemnum = Random.Range(0, 4);
                Pickup.Add(database.Items[itemnum]);
            }

        }

        if(other.gameObject.name == "RedPile" && onPickup)
        {
            for(int i = 0;i < quantity; i++)
            {
                if(Pickup[i].name == "Red")
                {
                    Pickup.RemoveAt(i);
                    i -= 1;
                    quantity--;
                }
            }
        }

        if (other.gameObject.name == "BluePile" && onPickup)
        {
            for (int i = 0; i < quantity; i++)
            {
                if (Pickup[i].name == "Blue")
                {
                    Pickup.RemoveAt(i);
                    i -= 1;
                    quantity--;
                }
            }
        }

        if (other.gameObject.name == "YellowPile" && onPickup)
        {
            for (int i = 0; i < quantity; i++)
            {
                if (Pickup[i].name == "Yellow")
                {
                    Pickup.RemoveAt(i);
                    i -= 1;
                    quantity--;
                }
            }
        }

        if (other.gameObject.name == "GreenPile" && onPickup)
        {
            for (int i = 0; i < quantity; i++)
            {
                if (Pickup[i].name == "Green")
                {
                    Pickup.RemoveAt(i);
                    i -= 1;
                    quantity--;
                }
            }
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

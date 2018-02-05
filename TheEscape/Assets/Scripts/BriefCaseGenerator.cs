using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BriefCaseGenerator : MonoBehaviour {

    private List<Item> inventory;
    private ItemDatabase database;
    private int numberOfItems;
    private const int MAX_ITEMS = 6;
    private const int MIN_ITEMS = 3;

    private bool showInventory;
    private bool showItem;
    private string ItemDetails;
    public Texture2D InventoryBackground;
    public Texture2D EmptySlot;
    public GUISkin slotBackground;

    // Use this for initialization
    void Start ()
    {
        database = GameObject.Find("Item Database").GetComponent<ItemDatabase>();
        showInventory = false;
        showItem = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (numberOfItems == 0)
        {
            showInventory = false;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (!showInventory)
        {
            numberOfItems = Random.Range(MIN_ITEMS, MAX_ITEMS + 1);
            for (int i = 0; i < numberOfItems; i++)
            {
                inventory.Add(database.Items[Random.Range(0, inventory.Count)]);
            }
            showInventory = true;
        }
    }

    void OnGUI()
    {
        ItemDetails = "";

        if (showInventory)
        {
            for (int i = 0; i < MAX_ITEMS; i++)
            {
                if (i * MAX_ITEMS + 1 <= inventory.Count)
                {
                    Rect slot = new Rect(10 * 60, 50 + i * 60, 50, 50);
                    GUI.Box(slot, inventory[i * MAX_ITEMS].icon);
                    if (slot.Contains(Event.current.mousePosition))
                    {
                        ItemDetails = ShowItem(inventory[i * MAX_ITEMS]);
                        showItem = true;
                    }

                    if (ItemDetails == "")
                        showItem = false;
                }
                else
                {
                    Rect slot = new Rect(10 * 60, 50 + i * 60, 50, 50);
                    GUI.Box(slot, EmptySlot);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BriefCaseGenerator : MonoBehaviour {

    private List<Item> inventory;
    private int numberOfItems;
    private ItemDatabase database;
    private const int MAX_ITEMS = 6;
    private const int MIN_ITEMS = 3;
    private bool showInventory;
    private string ItemDetails;

    // Use this for initialization
    void Start () {
        database = GameObject.Find("Item Database").GetComponent<ItemDatabase>();
        showInventory = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (numberOfItems == 0)
        {
            showInventory = false;
        }
	}

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "PileOfItems")
        {
            numberOfItems = Random.Range(MIN_ITEMS, MAX_ITEMS + 1);
            for (int i = 0; i < numberOfItems; i++)
            {
                inventory.Add(database.Items[Random.Range(0, 6)]);
            }
        }
    }

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.name == "PileOfItems")
        {
            showInventory = true;
        }
    }

        void OnGUI()
    {
        // TODO: Show contents
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthItem : MonoBehaviour {

    public Item item;

	// Use this for initialization
	//void Start () {
 //       item.name = "Default";
 //       item.type = Item.ItemType.Other;
 //       item.value = 0;
	//}

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerController.PickUpStealthItem(item);
            // Destroy(other.collider.gameObject);
        }
    }
}

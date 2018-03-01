﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item {

    public enum ItemType
    {
        Consumable,
        Valuable,
        Other
    }

    public string name;
    public ItemType type;
    public float value;
    public Texture2D icon;
}

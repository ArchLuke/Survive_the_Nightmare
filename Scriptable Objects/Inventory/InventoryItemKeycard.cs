using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Inventory System/Items/Keycard")]
public class InventoryItemKeycard: InventoryItem
{
    [SerializeField] private string name = null;
    [SerializeField] private SharedBool _doorCanOpen = null;
    [SerializeField] private SharedString _curName = null;

    public delegate void DoorAction(string name);
    public static DoorAction openDoor;
    // --------------------------------------------------------------------------------------------
    // Name :   Use
    // Desc :   Called when the item is consumed from the inventory
    // --------------------------------------------------------------------------------------------
    public override InventoryItem Use(Vector3 position, bool playAudio = true, Inventory inventory = null)
    {
        
        bool match = (_curName.value == name);
        if (match)
        {
            _doorCanOpen.value = true;
            if (openDoor != null)
                openDoor(name);
        }
        else
            return this;
        return base.Use(position, playAudio? match : false);
    }
}
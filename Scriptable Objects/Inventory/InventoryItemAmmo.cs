using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Inventory System/Items/Ammunition")]
public class InventoryItemAmmo : InventoryItem
{
    // Inspector Assigned
    [SerializeField] private SharedInt _recipient;

    [SerializeField] private int _type;
    // --------------------------------------------------------------------------------------------
    // Name :   Use
    // Desc :   Called when the item is consumed from the inventory
    // --------------------------------------------------------------------------------------------
    public override InventoryItem Use(Vector3 position, bool playAudio = true, Inventory inventory = null)
    {
        if (_type == 0)
            _recipient.value = 12;
        
        else
            _recipient.value = 15;

        // Call base class for default sound processing
        return base.Use(position, playAudio);
    }
}

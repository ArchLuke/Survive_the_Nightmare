using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class InventoryMountInfo
{
}
[System.Serializable]
public class InventoryBackpackMountInfo : InventoryMountInfo
{
    public InventoryItem Item = null;
}

// ------------------------------------------------------------------------------------------------
// Class    :   Inventory
// Desc     :   Base class for implementations of Inventories using this system
// ------------------------------------------------------------------------------------------------
public abstract class Inventory : ScriptableObject
{

    public abstract InventoryBackpackMountInfo GetBackpack(int mountIndex);
    public abstract void                        DropBackpackItem(int mountIndex, bool playAudio = true);
    public abstract bool                        UseBackpackItem (int mountIndex, bool playAudio = true);
    public abstract bool                        AddItem         (CollectableItem collectableItem,bool playAudio = true);


}

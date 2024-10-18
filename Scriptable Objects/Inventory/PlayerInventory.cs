using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Inventory System/Player Inventory")]
public class PlayerInventory : Inventory, ISerializationCallbackReceiver
{
    // Serialized Fields
    [Header("Mount Configuration and Starting Items")]
    [SerializeField] protected List<InventoryBackpackMountInfo> _backpackMounts = new List<InventoryBackpackMountInfo>();

    [Header("Shared Variables")]
    [SerializeField] protected SharedTimedStringQueue   _notificationQueue = null;

    [Header("Shared Variables - Broadcasters")]
    [SerializeField] protected SharedVector3            _playerPosition     = null;
    [SerializeField] protected SharedVector3            _playerDirection    = null;
   
    // Private
    // Runtime Mount Lists
    protected List<InventoryBackpackMountInfo>  _backpack   = new List<InventoryBackpackMountInfo>();

    // ISerializationCallbackReceiver
    public void OnBeforeSerialize() { }

    // --------------------------------------------------------------------------------------------
    // Name :   OnAfterDeserialize()
    // Desc :   We use this function so at runtime we clone the Mounts into copies that we can
    //          mutate without any affecting our original lists as defined in the inpsector. 
    // Note :   This has to be a deep copy as having two seperate lists referencing the same
    //          Mount objects will still mutate the original data.
    // --------------------------------------------------------------------------------------------
    public void OnAfterDeserialize()
    {
        // Clear our runtime lists
        _backpack.Clear();
        
        foreach (InventoryBackpackMountInfo info in _backpackMounts)
        {
            InventoryBackpackMountInfo clone = new InventoryBackpackMountInfo();
            clone.Item = info.Item;
            _backpack.Add(clone);
        }

    }

    public void ClearInventory()
    {
        _backpack.Clear();
        foreach (InventoryBackpackMountInfo info in _backpackMounts)
        {
            InventoryBackpackMountInfo clone = new InventoryBackpackMountInfo();
            clone.Item = info.Item;
            _backpack.Add(clone);
        }
    }
    // --------------------------------------------------------------------------------------------
    // Name :   GetBackpack
    // Desc :   Returns information about the item at the specified mount in the backpack
    // --------------------------------------------------------------------------------------------
    public override InventoryBackpackMountInfo GetBackpack(int mountIndex)
    {
        if (mountIndex < 0 || mountIndex >= _backpack.Count) return null;
        return _backpack[mountIndex];
    }
    public override void DropBackpackItem(int mountIndex, bool playAudio = true)
    {
        if (mountIndex < 0 || mountIndex >= _backpack.Count) return;

        // Chck we have a valid BackPack mount in the inventory
        InventoryBackpackMountInfo itemMount = _backpack[mountIndex];
        if (itemMount == null || itemMount.Item == null) return;

        // Put it in the scene
        Vector3 position = _playerPosition != null ? _playerPosition.value : Vector3.zero;
        position += _playerDirection != null ? _playerDirection.value : Vector3.zero;
        itemMount.Item.Drop( position , playAudio);

        // Nullify the slot so it is empty
        _backpack[mountIndex].Item = null;
    }
    public override bool UseBackpackItem(int mountIndex, bool playAudio = true)
    {
        // Is the selected slot valid to be consumed
        if (mountIndex < 0 || mountIndex >= _backpack.Count) return false;
        // Get weapon mount and return if no weapon assigned
        InventoryBackpackMountInfo backpackMountInfo = _backpack[mountIndex];
        if (backpackMountInfo.Item == null) return false;

        // Get the prefab from the app dictionary for this item
        InventoryItem backpackItem = backpackMountInfo.Item;

        // Tell the item to consume itself
        Vector3 position = _playerPosition != null ? _playerPosition.value : Vector3.zero;
        InventoryItem replacement = backpackItem.Use(position, playAudio);

        // Assign either null or a replacement item to that inventory slot
        _backpack[mountIndex].Item = replacement;

        // Mission Success
        return true;
    }

    public override bool AddItem(CollectableItem collectableItem,bool playAudio = true)
    {
        // Can't add if passed null or the CollectableItem has no associated InventoryItem
        if (collectableItem == null || collectableItem.inventoryItem == null) return false;

        // Determine the item type and call the appropriate function to the perform the
        // add to the inventory and remove the item from the scene.
        InventoryItem invItem = collectableItem.inventoryItem;

        
        return AddBackpackItem(invItem, collectableItem, playAudio);

        return false;    
    }
    // --------------------------------------------------------------------------------------------
    // Name :   AddBackpackItem
    // Desc :   Searches for the first available Backpack Mount and assigns the passed item to it.
    // --------------------------------------------------------------------------------------------
    protected bool AddBackpackItem(InventoryItem inventoryItem, CollectableItem collectableItem, bool playAudio)
    {
        // Search for empty mount in Backpack
        for (int i = 0; i < _backpack.Count; i++)
        {
            // A free mount is one with no item assigned
            if (_backpack[i].Item == null)
            {
                // Store this Item type at the mount
                _backpack[i].Item = inventoryItem;

                // Pickup
                inventoryItem.Pickup(collectableItem.transform.position, playAudio);

                // Broadcast that attempt was successful
                // if (_notificationQueue)
                //     _notificationQueue.Enqueue("Added " + inventoryItem.inventoryName + " to Backpack");

                // Success so return
                return true;
            }
        }

        // Broadcast that attempt was NOT successful
        if (_notificationQueue)
            _notificationQueue.Enqueue("No room in Backpack. Consider dropping.");

        return false;
    }

}

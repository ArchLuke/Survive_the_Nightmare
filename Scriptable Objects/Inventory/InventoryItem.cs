using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Public Enums that catagorize item behaviours

// ------------------------------------------------------------------------------------------------
// CLASS    :   InventoryItem
// DESC     :   The base class for all InventoryItem scriptable objects
// ------------------------------------------------------------------------------------------------
[CreateAssetMenu(menuName = "Scriptable Objects/Inventory System/Items/Base")]
public class InventoryItem : ScriptableObject
{
    [Header("General Properties")]

    [Tooltip("Interactive Text that is display next to the item name.")]
    [SerializeField] protected string _pickupText = "Press 'Use' to Pickup";

    [Tooltip("Name used for displaying this item in the Inventory.")]
    [SerializeField] protected string _inventoryName = null;

    [Tooltip("Sprite used to display this item in the Inventory.")]
    [SerializeField] protected Sprite _inventoryImage = null;

    [Tooltip("The collectable Item that is instantiated in the scene when this item is dropped from the Inventory.")]
    
    [SerializeField] protected CollectableItem _collectableItem = null;

    [Tooltip("Item that replaces the old one after the 'use' button is pressed")] [SerializeField]
    protected InventoryItem _replacementItem = null;
    
    [Tooltip("Audio Collection to use for this Inventory Item.\n\n" +
             "Bank[0] : Pickup Sounds\nBank[1] : Drop Sounds\nBank[2] : Use Sounds")]
    [SerializeField] protected AudioCollection _audio = null;


    // Property Getters
    public string inventoryName { get { return _inventoryName; } }
    public Sprite inventoryImage { get { return _inventoryImage; } }
    public virtual string pickupText { get { return _pickupText; } }
    // --------------------------------------------------------------------------------------------
    // Name : Pickup
    // Desc : Called by Inventory System when item is added to the Inventory
    // --------------------------------------------------------------------------------------------
    public virtual void Pickup(Vector3 position, bool playAudio = true)
    {
        if (_audio != null && AudioManager.instance != null && playAudio)
        {
            // Get pickup audio in first bank
            AudioClip pickupAudio = _audio[0];
            if (pickupAudio != null)
            {
                AudioManager.instance.PlayOneShotSound(_audio.audioGroup,
                                                       pickupAudio,
                                                       position,
                                                       _audio.volume,
                                                       _audio.spatialBlend,
                                                       _audio.priority);
            }


        }
    }

    // --------------------------------------------------------------------------------------------
    // Name : Drop
    // Desc : Called by Inventory System when item is Dropped from the Inventory (using Action Button)
    // --------------------------------------------------------------------------------------------
    public virtual CollectableItem Drop(Vector3 position, bool playAudio = true)
    {
        if (_audio != null && AudioManager.instance != null && playAudio)
        {
            // Get drop audio in 2nd bank
            AudioClip dropAudio = _audio[1];
            if (dropAudio != null)
            {
                AudioManager.instance.PlayOneShotSound(_audio.audioGroup,
                                                        dropAudio,
                                                        position,
                                                        _audio.volume,
                                                        _audio.spatialBlend,
                                                        _audio.priority);

            }
        }

        if (_collectableItem != null)
        {
            CollectableItem go = Instantiate<CollectableItem>(_collectableItem);
            go.transform.position = position;
            return go;
        }

        return null;
    }

    // --------------------------------------------------------------------------------------------
    // Name : Use (Action Button Mapping)
    // Desc : Called when the item is USED via the Inventory screen. In the case of health pills
    //        this would consume the item adding health to player. 
    // --------------------------------------------------------------------------------------------
    public virtual InventoryItem Use(Vector3 position, bool playAudio = true, Inventory inventory = null)
    {
        if (_audio != null && AudioManager.instance != null && playAudio)
        {
            // Get pickup audio in first bank
            AudioClip useAudio = _audio[2];
            if (useAudio != null)
            {
                AudioManager.instance.PlayOneShotSound(_audio.audioGroup,
                                                        useAudio,
                                                        position,
                                                        _audio.volume,
                                                        _audio.spatialBlend,
                                                        _audio.priority);
            }
        }

        // Return the item that should replace this one in the inventory after use
        return _replacementItem;
    }
}

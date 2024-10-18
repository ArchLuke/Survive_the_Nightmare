using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct InventoryUI_ActionButton
{
    public GameObject GameObject;
    public Text ButtonText;
}

// ------------------------------------------------------------------------------------------------
// CLASS    :   PlayerInventoryUI
// DESC     :   Manages the UI used to interact and display the player's inventory
// ------------------------------------------------------------------------------------------------
public class PlayerInventoryUI : MonoBehaviour
{
    [SerializeField]
    protected Inventory _inventory = null;
    // Backpack Mounts
    [Header("Equipment Mount References")]
    [SerializeField]
    protected List<GameObject> _backpackMounts          = new List<GameObject>();
    protected List<Image>      _backpackMountImages     = new List<Image>();
    protected List<Text>       _backpackMountText       = new List<Text>();
    protected List<Image> _backpackMountFrames = new List<Image>();
    
    // Action Button UI References
    [Header("Action Button UI References")]
    [SerializeField] protected InventoryUI_ActionButton _actionButton1;
    [SerializeField] protected InventoryUI_ActionButton _actionButton2;
    public Inventory inventory { get { return _inventory; } set { _inventory = value; } }

    //Counter referencce
    [SerializeField] protected Text _counterText;
    // Internals
    protected int                _selectedMount = -1;
    protected bool               _isInitialized = false;

    public int selectedMount
    {
        get { return _selectedMount; }
        set { _selectedMount = value; }
    }

    // --------------------------------------------------------------------------------------------
    // Name : OnEnable
    // Desc : Called by UNITY everytime the UI is enabled
    // --------------------------------------------------------------------------------------------
    protected virtual void OnEnable()
    {
        // Update the UI to display the current state of Inventory in its
        // reset position
        Invalidate();
    }

    // --------------------------------------------------------------------------------------------
    // Name :   Invalidate
    // Desc :   This function updates the UI so all elements reflect the current state of the
    //          user's inventory. This function also resets the Inventory to an unselected
    //          state.
    // --------------------------------------------------------------------------------------------
    protected virtual void Invalidate()
    {
        // Make sure its initialized before its is rendered for the first time
        if (!_isInitialized)
            Initialize();
        
        // Reset Selections
        _selectedMount = -1;

        // Deactivate the action buttons
        if (_actionButton1.GameObject != null)
            _actionButton1.GameObject.SetActive(false);

        if (_actionButton2.GameObject != null)
            _actionButton2.GameObject.SetActive(false);

        // Iterate over the UI Backpack mounts and set all to empty and unselected
        for (int i = 0; i < _backpackMounts.Count; i++)
        {
            // Clear sprite and deactivate mount
            if (_backpackMountImages[i] != null)
            {
                _backpackMountImages[i].gameObject.SetActive(false);
                _backpackMountImages[i].sprite = null;
            }

            if (_backpackMountText[i] != null)
                _backpackMountText[i].gameObject.SetActive(false);

            // Make all mounts look unselected
            if (_backpackMountFrames[i] != null)
            {
                _backpackMountFrames[i].gameObject.SetActive(false);
            }
        }
        for (int i = 0; i < _backpackMounts.Count; i++)
        {
            if (_backpackMounts[i] != null)
            {
                InventoryBackpackMountInfo backpackMountInfo = _inventory.GetBackpack(i);
                InventoryItem item = null;
                if (backpackMountInfo != null)
                    item = backpackMountInfo.Item;

                if (item != null)
                {
                    // Set sprite and activate mount
                    if (_backpackMountImages[i] != null)
                    {
                        _backpackMountImages[i].gameObject.SetActive(true);
                        _backpackMountImages[i].sprite = item.inventoryImage;
                    }

                    // Disable the text for this slot that says "EMPTY"
                    if (_backpackMountText[i] != null)
                        _backpackMountText[i].gameObject.SetActive(true);
                        _backpackMountText[i].text = item.inventoryName;
                }
            }
        }
    }


    // --------------------------------------------------------------------------------------------
    // Name :   Initialize
    // Desc :   This function is called ONCE the very first time the InventoryUI game object
    //          is enabled to display the inventory. It finds additional UI objects in the
    //          UI hierarchy and caches references to them. This saves us the work of having
    //          to hook up EVERY SINGLE REFERENCE via the inspector.
    // --------------------------------------------------------------------------------------------
    protected virtual void Initialize()
    {
        // This function should only be called once
        if (_isInitialized) return;

        // It has now been called
        _isInitialized = true;

        // Cache the UI references for the Image and Text for each backpack mount
        for (int i = 0; i < _backpackMounts.Count; i++)
        {
            // All empty slots to begin with
            _backpackMountImages.Add(null);
            _backpackMountText.Add(null);
            _backpackMountFrames.Add(null);
            
            // Cache the image and text slot references
            // A slot should have exactly one image game object and one text game object
            if (_backpackMounts[i] != null)
            {
                Transform parent = _backpackMounts[i].transform;
                Transform tmp;
                tmp = parent.Find("Image");
                if (tmp) _backpackMountImages[i] = tmp.GetComponent<Image>();
              
                tmp = parent.Find("Description");
                if (tmp) _backpackMountText[i] = tmp.GetComponent<Text>();

                tmp = parent.Find("frame");
                if (tmp) _backpackMountFrames[i] = tmp.GetComponent<Image>();
            }
        }

     
    }
    public void OnClickBackpackMount(Image image)
    {
        // Get mountfrom name
        int mount;
        if (image == null || !int.TryParse(image.name, out mount))
        {
            Debug.Log("OnClickBackpackError : Could not parse image name as INT");
            return;
        }
        
        // Is this a valid mount that's been clicked
        if (mount >= 0 && mount < _backpackMounts.Count)
        {
            // We are clicking on the selected item so unselect
            if (mount == _selectedMount)
            {
                Invalidate();
            }
            else
            {
                if (_inventory.GetBackpack(mount).Item==null)
                    return;
               
                Invalidate();
                _selectedMount = mount;
                _backpackMountFrames[mount].gameObject.SetActive(true);
                _actionButton1.GameObject.SetActive(true);
                _actionButton2.GameObject.SetActive(true);

            }

        }
    }
    // ---------------------------------------------------------------------------------------------
    // Name :   OnActionButton1
    // Desc :   Called when ActionButton1 is clicked in the UI
    // ---------------------------------------------------------------------------------------------
    public void OnActionButton1()
    {
        if (!_inventory) return;

        _inventory.UseBackpackItem(_selectedMount);
        
        // Repaint Inventory
        Invalidate();
    }

    // ---------------------------------------------------------------------------------------------
    // Name :   OnActionButton2
    // Desc :   Called when ActionButton2 is clicked in the UI
    // ---------------------------------------------------------------------------------------------
    public void OnActionButton2()
    {
        // No Inventory so bail
        if (_inventory == null) return;
        _inventory.DropBackpackItem(_selectedMount);
        // Repaint Inventory to reflect changes
        Invalidate();
    }

}

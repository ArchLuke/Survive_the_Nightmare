using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class InteractiveKeySlidingDoor : InteractiveItem {

    [SerializeField] private string _infoText;
    [SerializeField] private AudioCollection _audio;
    [SerializeField] private GameObject _inventoryUI = null;
    [SerializeField] private GameObject _playerHUD = null;
    [SerializeField] private SharedString _currentDoor = null;
    [SerializeField] private SharedBool _doorCanOpen = null;
    public string name = null;

    private Animator a = null;
    private bool b = false;
    const string OPEN = "open";

    void Start()
    {
        a = GetComponent<Animator>();
        InventoryItemKeycard.openDoor += DelegatePass;

        base.Start();
    }
    public override string GetText()
    {
        return _infoText;
    }

    public void OpenCloseDoor()
    {
        a.SetBool(OPEN, b);
        if (_audio)
        {
            if (b)
                AudioManager.instance.PlayOneShotSound("Scene", 
                    _audio[0], 
                    transform.position, 
                    _audio.volume, 
                    _audio.spatialBlend, 
                    _audio.priority);
            else
                AudioManager.instance.PlayOneShotSound("Scene", 
                    _audio[1], 
                    transform.position, 
                    _audio.volume, 
                    _audio.spatialBlend, 
                    _audio.priority);
        }
    }
    public void DelegatePass(string str)
    {
        if (name != str)
            return;
        _infoText = "Press 'E' to interact";
        Activate(null);
    }
    public override void Activate ( CharacterManager characterManager){
    
        b = a.GetBool(OPEN) ? false : true;
        if (!b)
            OpenCloseDoor();
        

        if (_doorCanOpen.value)
            OpenCloseDoor();
        else
        {
            _inventoryUI.SetActive(true);
            if (_playerHUD) _playerHUD.gameObject.SetActive(false);
            Time.timeScale = 0;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            _currentDoor.value = name;
        }    
        
    }
}

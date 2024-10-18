using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialHandler : MonoBehaviour
{
    [SerializeField] private PlayerHUD			_playerHUD			= null;
    [SerializeField] private TextMeshProUGUI _text;
    
    void Start()
    {
        if (_playerHUD) _playerHUD.gameObject.SetActive(false);

        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ExitGame()
    {
        if (ApplicationManager.instance)
            ApplicationManager.instance.QuitGame();
    }
    
    public void PointerEnter()
    {
        _text.color = Color.red;
    }
    
    public void PointerExit()
    {
        _text.color = Color.white;
    }
    
}

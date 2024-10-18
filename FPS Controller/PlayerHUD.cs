using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ScreenFadeType{ FadeIn, FadeOut }

public class PlayerHUD : MonoBehaviour 
{
	// Inspector Assigned UI References

	[SerializeField] private PlayerInventory _inventory;
    [Header("UI Text")]
    [SerializeField] private TextMeshProUGUI _healthText     = null;
    [SerializeField] private TextMeshProUGUI _ammoText     = null;
    [SerializeField] private GameObject _deathText;
    [SerializeField] private float _deathDuration = 0.1f;
		
    [SerializeField] private Text		_notificationText   =	null;
    [SerializeField] private Text		_interactionText	=	null;

    [Header("Shared Variables")]
    [SerializeField] private SharedFloat            _health        = null;

    [SerializeField] private SharedInt _rifleAmmo = null;
    [SerializeField] private SharedInt _pistolAmmo = null;
    [SerializeField] private SharedInt _curGun = null;

    [SerializeField] private SharedString           _interactionString = null;
    [SerializeField] private SharedTimedStringQueue _notificationQueue = null;


    [Header("Additional")]
    [SerializeField] private Image 		_screenFade =	null;
    [SerializeField] private GameObject _crosshair = null;

	// Internals
	float _currentFadeLevel = 1.0f;
	IEnumerator _coroutine	= null;
	private bool death = true;
	public void Start()
	{
		death = true;
		if (_screenFade)
		{
			Color color = _screenFade.color;
			color.a = _currentFadeLevel;
			_screenFade.color = color;
		}

       
	}

	public void Fade ( float seconds, ScreenFadeType direction )
	{
		if (_coroutine!=null) StopCoroutine(_coroutine); 
		float targetFade  = 0.0f;;

		switch (direction)
		{
			case ScreenFadeType.FadeIn:
			targetFade = 0.0f;
			break;

			case ScreenFadeType.FadeOut:
			targetFade = 1.0f;
			break;
		}

		_coroutine = FadeInternal( seconds, targetFade);
		StartCoroutine(_coroutine);
	}


	IEnumerator FadeInternal( float seconds, float targetFade )
	{
		if (!_screenFade) yield break;

		float timer = 0;
		float srcFade = _currentFadeLevel;
		Color oldColor = _screenFade.color;
		if (seconds<0.1f) seconds = 0.1f;

		while (timer<seconds)
		{
			timer+=Time.deltaTime;
			_currentFadeLevel = Mathf.Lerp( srcFade, targetFade, timer/seconds );
			oldColor.a = _currentFadeLevel;
			_screenFade.color = oldColor;
			yield return null;
		}

		oldColor.a = _currentFadeLevel = targetFade;
		_screenFade.color = oldColor;
	}

	public void DoDeath()
	{
		if(death)
			StartCoroutine(Cor());
	}

	IEnumerator Cor()
	{
		death = false;
		_deathText.SetActive(true);

		TextMeshProUGUI textCom = _deathText.GetComponentInChildren<TextMeshProUGUI>();
		string words = "YOU ARE DEAD";

		for(int i=1;i<13;i++)
		{

			textCom.text = words.Substring(0, i);
			
			yield return new WaitForSeconds(_deathDuration);
		}
		
		yield return new WaitForSeconds(2f);
		
		if(ApplicationManager.instance)
			ApplicationManager.instance.LoadMainMenu();
	}
    void Update()
    {
	    if (_curGun.value == 1)
	    {
		    _ammoText.text = _rifleAmmo.value + "/" + RifleCount();
	    }
	    else
	    {
		    _ammoText.text = _pistolAmmo.value + "/" + PistolCount();
	    }
        if (_healthText != null && _health != null)
            _healthText.text = _health.value.ToString();

        if (_interactionText != null && _interactionString != null)
            _interactionText.text = _interactionString.value;

        if (_notificationText != null && _notificationQueue != null)
            _notificationText.text = _notificationQueue.text;

    }

    string PistolCount()
    {
	    int cnt = 0;
	    for (int i = 0; i < 8; i++)
	    {
		    if (_inventory.GetBackpack(i).Item == null)
			    continue;
		    InventoryItem it = _inventory.GetBackpack(i).Item;
		    String text = it.inventoryName;
		    if (text == "Pistol Magazine")
		    {
			    cnt += 12;
		    }
	    }

	    return cnt.ToString();
    }
    string RifleCount()
    {
	    int cnt = 0;
	    for (int i = 0; i < 8; i++)
	    {
		    if (_inventory.GetBackpack(i).Item == null)
			    continue;
		    InventoryItem it = _inventory.GetBackpack(i).Item;
		    String text = it.inventoryName;
		    if (text == "rifle magazine")
		    {
			    cnt += 15;
		    }
	    }

	    return cnt.ToString();
    }
}

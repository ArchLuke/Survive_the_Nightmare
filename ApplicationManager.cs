using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ApplicationManager : MonoBehaviour
{
	[Header("Scene Load")]
	
	[SerializeField] private float _fadeTime;
	[SerializeField] float _minLoadTime = 2f;

	[Header("Resets")]
	[SerializeField] private SharedInt _curGun;
	[SerializeField] private SharedFloat _health;
	[SerializeField] private SharedInt _rifleAmmo;
	[SerializeField] private SharedInt _pistolAmmo;
	[SerializeField] private SharedInt _destination;

	[SerializeField] private PlayerInventory _inventory;
	[SerializeField] private List<SharedBool> _doors = null;
	
	private float _loadTime = 0f;
	private Color _col;
	private Image _screenFade;
	// Singleton Design
	private static ApplicationManager _Instance		= null;
	
	public static ApplicationManager instance
	{
		get { 
			// If we don't an instance yet find it in the scene hierarchy
			if (_Instance==null) { _Instance = (ApplicationManager)FindObjectOfType(typeof(ApplicationManager)); }
			
			// Return the instance
			return _Instance;
		}
	}
	void Awake()
	{
		
		// This object must live for the entire application
		DontDestroyOnLoad(gameObject);
	}
	public void LoadMainMenu()
	{
		Reset();
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		SceneManager.LoadScene("Bedroom");
	}

	public void Reset()
	{
		_health.value = 100f;
		_rifleAmmo.value = 0;
		_pistolAmmo.value = 0;
		_inventory.ClearInventory();
		_curGun.value = 0;
		_destination.value = 0;
		
		foreach (SharedBool door in _doors)
			door.value = false;
		
	}
	public void LoadGame(GameObject ui,Thunder light, GameObject rain, GameObject bar, GameObject text, GameObject fade)
	{
		
		_loadTime = Time.time + _minLoadTime;
		StartCoroutine(LoadAsync("Prison",ui,light,rain,bar,text,fade));
	}

	IEnumerator LoadAsync(string name,GameObject _ui,Thunder _light, GameObject _rain, GameObject _bar, GameObject _text, GameObject _fade)
	{
		_screenFade = _fade.GetComponentInChildren<Image>();
		_col = _screenFade.color;
		
		_fade.SetActive(true);
		_ui.SetActive(true);
		_light.doLight = false;
		_rain.SetActive(false);
		
		AsyncOperation op=SceneManager.LoadSceneAsync(name);
		op.allowSceneActivation = false;
		while (!op.isDone)
		{
			if (op.progress >= 0.9f && Time.time > _loadTime)
			{
				_bar.SetActive(false);
				_text.SetActive(true);
				if (Input.GetKeyDown(KeyCode.Space))
				{
					float timer = 0;
					float _currentFadeLevel = 0f;

					while (timer<_fadeTime)
					{
						timer+=Time.deltaTime;
						_currentFadeLevel = Mathf.Lerp( 0, 1, timer/_fadeTime );
						_col.a = _currentFadeLevel;
						_screenFade.color = _col;
						yield return null;
					}
					
					op.allowSceneActivation = true;

				}
			}

			yield return null;
		}
	
	}
	public void QuitGame()
	{
		#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
		#else
			Application.Quit();
		#endif 
	}
}

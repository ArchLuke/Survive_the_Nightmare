using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using Random = UnityEngine.Random;

public class CharacterManager : MonoBehaviour 
{
    // Inspector Assigned
    [SerializeField] private GameObject _rifleMuzzleFlash = null;
    [SerializeField] private Transform _rifleMuzzleParent = null;
    [SerializeField] private GameObject _pistolMuzzleFlash = null;
    [SerializeField] private Transform _pistolMuzzleParent = null;
    [SerializeField] private GameObject _pistol = null;
    [SerializeField] private GameObject _rifle = null;
    [SerializeField] private GameObject _knife = null;
    [SerializeField] private float _shakeMagnitude = 20f;
    [SerializeField] private GameObject _flashLight = null;
    [SerializeField] private CapsuleCollider 	_meleeTrigger 		= null;
	[SerializeField] private Camera				_camera				= null;
	[SerializeField] private AISoundEmitter		_soundEmitter		= null;
	[SerializeField] private float				_walkRadius			= 0.0f;
	[SerializeField] private float				_runRadius			= 7.0f;
	[SerializeField] private float				_landingRadius		= 12.0f;
	[SerializeField] private float				_bloodRadiusScale	= 6.0f;

	[SerializeField] private PlayerHUD			_playerHUD			= null;
	[SerializeField] private GameObject _ammoBar = null;
	[SerializeField] private GameObject _tutorialUI = null;

	//Audio
	[SerializeField] private AudioCollection	_pistolSounds		= null;
	[SerializeField] private AudioCollection	_rifleSounds			= null;
	[SerializeField] private AudioCollection	_damageSounds		= null;

	[Header("Inventory")]
    [SerializeField] private GameObject _inventoryUI = null;
    [SerializeField] private PlayerInventory _inventory = null;
    [SerializeField] private InventoryItemWeapon _pistolStats = null;
    [SerializeField] private InventoryItemWeapon _rifleStats = null;

    [Header("Shared Variables")] 
    [SerializeField] private SharedFloat _health    = null;
    [SerializeField] private SharedString _interactionText = null;
    [SerializeField] private SharedInt _pistolBullets = null;
    [SerializeField] private SharedInt _rifleBullets = null;
    [SerializeField] private SharedBool  _rifleShoot= null;
    [SerializeField]private SharedInt _curGun = null;
    
    // Private
    private bool _shake = false;
    private int effectIdx=0;
    private int _aimParameter = 0;
    private Collider 			_collider 			 = null;
	private FPSController		_fpsController 		 = null;
	private CharacterController _characterController = null;
	private GameSceneManager	_gameSceneManager	 = null;
	private int					_aiBodyPartLayer     = -1;
	private int 				_interactiveMask	 = 0;
	private float				_nextAttackTime		 = 0f;
	private Animator _pistolAnimator = null;
	private Animator _rifleAnimator = null;
	private Animator _knifeAnimator = null;
	private Dictionary<int, Animator> _animatorMapping = new Dictionary<int, Animator>();
	
	private AudioLowPassFilter _lowPass;
	private Volume _vol;
	
	public FPSController	fpsController	{ get{ return _fpsController;}}

	public bool useExp = false;
	public bool canShoot = true;
	// Use this for initialization
	void Start () 
	{
		_collider 			= GetComponent<Collider>();
		_fpsController 		= GetComponent<FPSController>();
		_characterController= GetComponent<CharacterController>();
		_gameSceneManager 	= GameSceneManager.instance;
		_aiBodyPartLayer 	= LayerMask.NameToLayer("AI Body Part");
		_interactiveMask	= 1 << LayerMask.NameToLayer("Interactive");

		if (_gameSceneManager!=null)
		{
			PlayerInfo info 		= new PlayerInfo();
			info.camera 			= _camera;
			info.characterManager 	= this;
			info.collider			= _collider;
			info.meleeTrigger		= _meleeTrigger;

			_gameSceneManager.RegisterPlayerInfo( _collider.GetInstanceID(), info );
		}

		// // Get rid of really annoying mouse cursor
		// Cursor.visible = false;
		// Cursor.lockState = CursorLockMode.Locked;

		// Start fading in
		//if (_playerHUD) _playerHUD.Fade( 2.0f, ScreenFadeType.FadeIn );
		_aimParameter = Animator.StringToHash("aim");

		_pistolAnimator = _pistol.GetComponent<Animator>();
		_animatorMapping[0] = _pistolAnimator;
		_rifleAnimator = _rifle.GetComponent<Animator>();
		_animatorMapping[1] = _rifleAnimator;
		_knifeAnimator = _knife.GetComponent<Animator>();
		_animatorMapping[2] = _knifeAnimator;

		_lowPass = _camera.gameObject.GetComponent<AudioLowPassFilter>();
		_vol=GameObject.Find("volumes/Global Volume").GetComponent<Volume>();

		InventoryItemKeycard.openDoor += DisableInventory;
	}
	
    public void TakeDamage ( float amount, bool doDamage, bool doPain, bool doShake )
    {
	    if (doShake)
		    _shake = true;
	    
	    _health.value = Mathf.RoundToInt(Mathf.Max ( _health.value - (amount *Time.deltaTime*1.25f)  , 0.0f));
		
		if (_fpsController)
		{
			_fpsController.dragMultiplier = 0.0f; 

		}
		

		if (_health.value<=0.0f) 
		{
			DoDeath();
		}
	}
    
	void DoDamage()
	{
		if (!canShoot)
			return;
		if (Time.timeScale == 0)
			return;
		if (_camera==null) return;
		if (_gameSceneManager==null) return;

		if (_curGun.value == 0)
		{
			if (_pistolBullets.value < 1)
				return;
			_pistolBullets.value--;
			_pistolAnimator.SetTrigger("shoot");
			_soundEmitter.SetRadius(10f);
		}
		else if (_curGun.value==1)
		{
			if (_rifleBullets.value < 1)
				return;
			_rifleBullets.value --;
			_rifleShoot.value = false;
			_rifleAnimator.SetTrigger("shoot");
			_soundEmitter.SetRadius(15f);

		}

		var muzzleFlash = Instantiate(_curGun.value==0?_pistolMuzzleFlash:_rifleMuzzleFlash, _curGun.value==0?_pistolMuzzleParent:_rifleMuzzleParent) as GameObject;
		Destroy(muzzleFlash, 4);
		AudioCollection col = _curGun.value == 0 ? _pistolSounds : _rifleSounds;
		if (AudioManager.instance)
		{
			AudioManager.instance.PlayOneShotSound(col.audioGroup,
				col.audioClip, transform.position,
				col.volume,
				col.spatialBlend,
				col.priority);
		}
			
		// Local Variables
		Ray ray;
		RaycastHit hit;
		bool isSomethingHit	=	false;

		ray = _camera.ScreenPointToRay( new Vector3( Screen.width/2, Screen.height/2, 0 ));

		float Range = _curGun.value==0 ?_pistolStats.range:_rifleStats.range;
		isSomethingHit = Physics.Raycast(ray, out hit, Range, 1<<_aiBodyPartLayer);

		
		_nextAttackTime = Time.time+0.5f;
		
		if (isSomethingHit)
		{
			float distance = Vector3.Distance(hit.point, transform.position);
			AIStateMachine stateMachine = _gameSceneManager.GetAIStateMachine( hit.rigidbody.GetInstanceID());
			if (stateMachine)
			{
				float force = _curGun.value == 0
					? _pistolStats.GetAttentuatedForce(distance)
					: _rifleStats.GetAttentuatedForce(distance);
				int damage = 0;
				if (hit.rigidbody.CompareTag("Head"))
				{
				
					damage = _curGun.value == 0
						? _pistolStats.GetAttentuatedDamage("Head", distance)
						: _rifleStats.GetAttentuatedDamage("Head", distance);
				}

				if (hit.rigidbody.CompareTag("Upper Body") || hit.rigidbody.CompareTag("Lower Body"))
				{
					damage = _curGun.value == 0
						? _pistolStats.GetAttentuatedDamage("body", distance)
						: _rifleStats.GetAttentuatedDamage("body", distance);
				}

				stateMachine.TakeDamage( hit.point, hit.normal,ray.direction * force, damage, hit.rigidbody, this, 0 );
			}
			
		}

	}

	void KnifeDoDamage()
	{
		_knifeAnimator.SetBool("attack",true);
		_knifeAnimator.SetFloat("attackType", Random.Range(0f,100f));
		if (_fpsController.movementStatus==PlayerMoveStatus.Crouching)
			_knifeAnimator.SetFloat("attackType", 75f);
	}
	
	private void LateUpdate()
	{
		

		if (Input.GetMouseButton(1))
		{
			_pistolAnimator.SetBool(_aimParameter,true);
			_rifleAnimator.SetBool(_aimParameter,true);
			_fpsController.dragMultiplier = 0.5f;
		}
		else
		{
			_pistolAnimator.SetBool(_aimParameter,false);
			_rifleAnimator.SetBool(_aimParameter,false);

		}
		_vol.profile.TryGet<Exposure>(out var _exp);
		_exp.active = false;
		if (_shake)
		{
			Vector3 newDirection = Vector3.RotateTowards(_camera.transform.forward, Random.insideUnitSphere, _shakeMagnitude*Mathf.Deg2Rad, Mathf.Infinity);
			_camera.transform.rotation = Quaternion.LookRotation(newDirection);
			
			if (AudioManager.instance)
			{
				if (_damageSounds!=null)
						AudioManager.instance.PlayOneShotSound( _damageSounds.audioGroup,
							_damageSounds.audioClip, transform.position,
							_damageSounds.volume,
							_damageSounds.spatialBlend,
							_damageSounds.priority );
			
			}
			_exp.active = true;
			_shake = false;
		}
	}

	void DisableInventory(string name)
	{
		_inventoryUI.SetActive(false);
		if (_playerHUD) _playerHUD.gameObject.SetActive(true);
		Time.timeScale = 1;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}
	void Update()
	{
		
		//process weapon toggles
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			if (_curGun.value == 0)
				return;
			_animatorMapping[_curGun.value].SetTrigger("Hide");
			_curGun.value = 0;
			Invoke("ShowPistol",0.5f);
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			if (_curGun.value == 1)
				return;
			_animatorMapping[_curGun.value].SetTrigger("Hide");
			_curGun.value = 1;
			Invoke("ShowRifle",0.5f);
		}
		
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			if (_curGun.value == 2)
				return;
			_animatorMapping[_curGun.value].SetTrigger("Hide");
			_curGun.value = 2;
			Invoke("ShowKnife",0.5f);
		}
		
        // Process Inventory Key Toggle
        if (Input.GetButtonDown("Inventory"))
        {
            // If its not visible...make it visible
            if (!_inventoryUI.activeSelf)
            {
	            if (!_tutorialUI.activeSelf)
	            {
		            _inventoryUI.SetActive(true);
		            if (_playerHUD) _playerHUD.gameObject.SetActive(false);
		            Time.timeScale = 0;
		            Cursor.visible = true;
		            Cursor.lockState = CursorLockMode.None;
		            return;
	            }
                
            }else
            {
                DisableInventory("");
            }
        }

        if (Input.GetButtonDown("reload"))
	        FindFirstAmmo();
        
        if (Input.GetButtonDown("Flashlight"))
        {
	        if (!_flashLight.activeSelf)
		        _flashLight.SetActive(true);
	        else
		        _flashLight.SetActive(false);
	        
        }
        
        
// Process Tutorial Key Toggle
        if (Input.GetButtonDown("Tutorial"))
        {
	        
	        // If its not visible...make it visible
	        if (!_tutorialUI.activeSelf )
	        {
		        if (!_inventoryUI.activeSelf)
		        {
			        _tutorialUI.SetActive(true);
			        if (_playerHUD) _playerHUD.gameObject.SetActive(false);
			        Time.timeScale = 0;
			        Cursor.visible = true;
			        Cursor.lockState = CursorLockMode.None;
			        return;  
		        }
		        
	        }else
	        {
		        _tutorialUI.SetActive(false);
		        if (_playerHUD) _playerHUD.gameObject.SetActive(true);
		        Time.timeScale = 1;
		        Cursor.lockState = CursorLockMode.Locked;
		        Cursor.visible = false;
	        }
	        if (_inventoryUI.activeSelf)
	        {
		        _inventoryUI.SetActive(false);
		        if (_playerHUD) _playerHUD.gameObject.SetActive(true);
		        Time.timeScale = 1;
		        Cursor.lockState = CursorLockMode.Locked;
		        Cursor.visible = false;
	        }
        }
        
        Ray ray;
		RaycastHit hit;
		RaycastHit [] hits;
		
		// PROCESS INTERACTIVE OBJECTS
		// Is the crosshair over a usuable item or descriptive item...first get ray from centre of screen
		ray = _camera.ScreenPointToRay( new Vector3(Screen.width/2, Screen.height/2, 0));

		// Calculate Ray Length
		float rayLength =  Mathf.Lerp( 1.0f, 1.8f, Mathf.Abs(Vector3.Dot( _camera.transform.forward, Vector3.up )));

		// Cast Ray and collect ALL hits
		hits = Physics.RaycastAll (ray, rayLength, _interactiveMask );

		// Process the hits for the one with the highest priorty
		if (hits.Length>0)
		{
			// Used to record the index of the highest priorty
			int 				highestPriority = int.MinValue;
			InteractiveItem		priorityObject	= null;	

			// Iterate through each hit
			for (int i=0; i<hits.Length; i++)
			{
				// Process next hit
				hit = hits[i];
               
				// Fetch its InteractiveItem script from the database
				InteractiveItem interactiveObject = _gameSceneManager.GetInteractiveItem( hit.collider.GetInstanceID());

				// If this is the highest priority object so far then remember it
				if (interactiveObject!=null && interactiveObject.priority>highestPriority)
				{
					priorityObject = interactiveObject;
					highestPriority= priorityObject.priority;
				}
			}

			// If we found an object then display its text and process any possible activation
			if (priorityObject!=null)
			{
                if (_interactionText)
                    _interactionText.value = priorityObject.GetText();
                
				if (Input.GetButtonDown ( "Use" ))
				{
					priorityObject.Activate( this );
				}
			}
		}
		else
		{
            if (_interactionText)
                _interactionText.value = null;
        }
		bool shoot = true;
		AnimatorClipInfo[] infos = _pistolAnimator.GetCurrentAnimatorClipInfo(0);;
		if (infos.Length == 1)
		{
			if (infos[0].clip.name == "Reload")
				shoot = false;
		}else
			shoot = false;

		// Are we attacking?
		if ((Input.GetMouseButtonDown(0) && _curGun.value==0 && Time.time>_nextAttackTime && shoot))
		{
			DoDamage();
		}

		shoot = true;
		infos = _rifleAnimator.GetCurrentAnimatorClipInfo(0);
		if (infos.Length == 1)
		{
			if (infos[0].clip.name == "Recharge")
				shoot = false;
		}else
			shoot = false;
			
			
		if (Input.GetMouseButton(0) && _rifleShoot.value && _curGun.value==1 && shoot)
		{
			DoDamage();
		}

		if (Input.GetMouseButtonDown(0) &&  _curGun.value==2)
		{
			KnifeDoDamage();
		}


		// Calculate the SoundEmitter radius and the Drag Multiplier Limit
		if (_fpsController && _soundEmitter!=null)
		{
			float newRadius = Mathf.Max( _walkRadius, (100.0f-_health.value)/_bloodRadiusScale);
			switch (_fpsController.movementStatus)
			{
				case PlayerMoveStatus.Landing: newRadius = Mathf.Max( newRadius, _landingRadius ); break;
				case PlayerMoveStatus.Running: newRadius = Mathf.Max( newRadius, _runRadius ); break;
			}

			_soundEmitter.SetRadius( newRadius );

		
			_lowPass.cutoffFrequency = Mathf.SmoothStep(500, 9000, _health.value/100f);
		}

		// if (Input.GetButtonDown("Freeze"))
		// {
		// 	Time.timeScale = 0;
		// 	Cursor.visible = true;
		// 	Cursor.lockState = CursorLockMode.None;
		// }

	}

	public bool FindFirstAmmo()
	{

		for (int i = 0; i < 8; i++)
		{
			if (_inventory.GetBackpack(i).Item == null)
				continue;
			InventoryItem it = _inventory.GetBackpack(i).Item;
			String text = it.inventoryName;
			if (_curGun.value==0? text == "Pistol Magazine": text=="rifle magazine")
			{
				PlayerInventoryUI ui = _inventoryUI.GetComponent<PlayerInventoryUI>();
				ui.selectedMount = i;
				ui.OnActionButton1();
				if(_curGun.value==0)
					_pistolAnimator.SetTrigger("reload");
				else
					_rifleAnimator.SetTrigger("reload");
				return true;
			}
		}

		return false;
	}

	public void ShowPistol()
	{
		_rifle.SetActive(false);
		_knife.SetActive(false);
		_pistol.SetActive(true);
		_ammoBar.SetActive(true);

	}
	
	public void ShowRifle()
	{
		_rifle.SetActive(true);
		_pistol.SetActive(false);
		_knife.SetActive(false);
		_ammoBar.SetActive(true);
	}
	public void ShowKnife()
	{
		_knife.SetActive(true);
		_rifle.SetActive(false);
		_pistol.SetActive(false);
		_ammoBar.SetActive(false);

	}
	public void DoLevelComplete()
	{
		if (_fpsController) 
			_fpsController.freezeMovement = true;
		

		Invoke( "GameOver", 4.0f);
	}


	public void DoDeath()
	{
		
		if (_fpsController) 
			_fpsController.freezeMovement = true;

		
		_playerHUD.DoDeath();
		Destroy(this);
	}
}

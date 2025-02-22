﻿using System;
using UnityEngine;
using System.Collections;

public class AIDamageTrigger : MonoBehaviour 
{
	// Inspector Variables
	[SerializeField] AIZombieStateMachine _stateMachine=null;
	[SerializeField] string			_parameter = "";
	[SerializeField] int			_bloodParticlesBurstAmount	=	10;
	[SerializeField] float			_damageAmount				=	0.1f;
	[SerializeField] bool			_doDamageSound				=	true;
	[SerializeField] bool			_doPainSound				=	true;

	// Private Variables
	private bool _doShake=true;
	Animator	   	 	_animator	 		= null;
	int			    	_parameterHash		= -1;
	GameSceneManager	_gameSceneManager	= null;
	private bool		_firstContact		= false;		

	// ------------------------------------------------------------
	// Name	:	Start
	// Desc	:	Called on object start-up to initialize the script.
	// ------------------------------------------------------------
	void Start()
	{
		// Cache state machine and animator references
		if (_stateMachine != null)
			_animator = _stateMachine.animator;

		// Generate parameter hash for more efficient parameter lookups from the animator
		_parameterHash = Animator.StringToHash (_parameter); 

		_gameSceneManager = GameSceneManager.instance;
	}

	void OnTriggerEnter( Collider col )
	{
		

		if (!_animator) 
			return;

		if (col.gameObject.CompareTag("Player") && _animator.GetFloat(_parameterHash) > 0.9f)
			_firstContact = true;
		
	}

	// -------------------------------------------------------------
	// Name	:	OnTriggerStay
	// Desc	:	Called by Unity each fixed update that THIS trigger
	//			is in contact with another.
	// -------------------------------------------------------------

	void OnTriggerStay(Collider col)
	{

		// If we don't have an animator return
		if (!_animator)
			return;


		// If this is the player object and our parameter is set for damage
		if (col.gameObject.CompareTag("Player") && _animator.GetFloat(_parameterHash) > 0.5f &&
		    _stateMachine.boneControlType != AIBoneControlType.Ragdoll)
		{
			// if (GameSceneManager.instance && GameSceneManager.instance.bloodParticles) 
			// {
			// 	ParticleSystem system = GameSceneManager.instance.bloodParticles;
			//
			// 	// Temporary Code
			// 	system.transform.position = transform.position;
			// 	system.transform.rotation = Camera.main.transform.rotation;
			//
			// 	var settings = system.main;
			// 	settings.simulationSpace = ParticleSystemSimulationSpace.World;
			// 	system.Emit (_bloodParticlesBurstAmount);
			// }

			if (_gameSceneManager != null)
			{
				PlayerInfo info = _gameSceneManager.GetPlayerInfo(col.GetInstanceID());
				if (info != null && info.characterManager != null)
				{
					
					info.characterManager.TakeDamage(_damageAmount, _doDamageSound && _firstContact, _doPainSound,
							_doShake);
					
				}

				_firstContact = false;
				_doShake = false;

			}
			else
			{
				if (col.gameObject.tag == "Player")
					_doShake = true;

			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		_doShake = true;
	}
}

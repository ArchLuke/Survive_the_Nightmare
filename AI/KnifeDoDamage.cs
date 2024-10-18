using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeDoDamage : MonoBehaviour
{
    [SerializeField] private Transform _pos;
    [SerializeField] private AudioCollection _audio;
    [SerializeField] private SharedBool _knife;

    private void OnTriggerStay(Collider other)
    {
        if (!_knife.value)
            return;
        if (other.gameObject.layer != LayerMask.NameToLayer("AI Body Part"))
            return;
        
        AudioManager.instance.PlayOneShotSound(_audio.audioGroup, _audio[0], _pos.position, _audio.volume,
                _audio.spatialBlend);
        Rigidbody body = other.gameObject.GetComponent<Rigidbody>();
            
        AIStateMachine stateMachine = 
                    GameSceneManager.instance.GetAIStateMachine(body.GetInstanceID());
        stateMachine.TakeDamage(_pos.position, -_pos.forward,Vector3.zero,20, body, null, 0,true);
        _knife.value = false;
    }
}

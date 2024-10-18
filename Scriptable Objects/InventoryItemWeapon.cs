using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------------------------------------
// CLASS    :   InventoryItemWeapon
// DESC     :   Represents a Weapon in the game and its capabilities
// ------------------------------------------------------------------------------------------------
[CreateAssetMenu(menuName = "Scriptable Objects/Weapon")]
public class InventoryItemWeapon : ScriptableObject
{

    [Tooltip("Max Range in meters of this weapon.")]
    [SerializeField] protected float _range = 0.0f;

    [Tooltip("Maximum damage done to the Head of an enemy with a single hit.")]
    [SerializeField] protected int _headDamage = 100;

    [Tooltip("Maximum damage done to the body of an enemy with a single hit.")]
    [SerializeField] protected int _bodyDamage = 20;

    [Tooltip("How damage is diluted over the range of the weapon.")]
    [SerializeField] protected AnimationCurve _damageAttenuation = new AnimationCurve();

    [Tooltip("Force applied by this weapon on a target.")]
    [SerializeField] protected float _force = 100.0f;

    [Tooltip("How force is diluted over the range of the weapon.")]
    [SerializeField] protected AnimationCurve _forceAttenuation = new AnimationCurve();

    public float range                              { get { return _range; } }
    public int headDamage                           { get { return _headDamage; } }
    public int bodyDamage                           { get { return _bodyDamage; } }
    public float force                              { get { return _force; } }

    // --------------------------------------------------------------------------------------------
    // Name :   GetAttenuatedDamage
    // Desc :   Given a distance in meters and a body part string ("Head" or "Body") will return
    //          the damage that weapon does to that body part 
    // --------------------------------------------------------------------------------------------
    public int GetAttentuatedDamage(string bodyPart, float distance)
    {
        float normalizedDistance = Mathf.Clamp(distance / _range, 0.0f, 1.0f);
        if (bodyPart.Equals("Head"))
            return (int)(_damageAttenuation.Evaluate(normalizedDistance) * _headDamage);
        else
            return (int)(_damageAttenuation.Evaluate(normalizedDistance) * _bodyDamage);

    }

    // ---------------------------------------------------------------------------------------------
    // Name :   GetAttenuatedForce
    // Desc :   Given a distance to a target return the amount of force that will be recieved 
    //          from this weapon at this distance
    // ---------------------------------------------------------------------------------------------
    public float GetAttentuatedForce(float distance)
    {
        if (_force == 0.0f) return 0.0f;
        else
            return _forceAttenuation.Evaluate(Mathf.Clamp(distance / _range, 0.0f, 1.0f)) * _force;
    }
}

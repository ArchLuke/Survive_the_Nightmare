using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class InteractiveDoor : InteractiveItem {
	[SerializeField] private string _infoText;
	[SerializeField] private AudioCollection _audio;
	public enum rotOrient
	{
		Y_Axis_Up,
		Z_Axis_Up,
		X_Axis_Up
	}

	public enum rotFixAxis
	{
		Y, 
		Z
	}

    public enum doorType
    {
        Regular,
        Sliding
    }

    public doorType doorMovement;
	public rotOrient rotationOrientation;
	public bool applyRotationFix = false;
	public rotFixAxis rotationAxisFix;
	public float doorOpenAngle = -90.0f;
	[Range(1,5)] public float speed = 3.0f;

	Quaternion doorOpen = Quaternion.identity;
	Quaternion doorClosed = Quaternion.identity;

	bool doorStatus = false;

	protected override void Start()
	{
		base.Start();
		// if (this.gameObject.isStatic) {
		// 	Debug.Log ("This door has been set to static and won't be openable. Doorscript has been removed.");
		// 	Destroy (this);
		// }
		switch (rotationOrientation) {
		case rotOrient.Z_Axis_Up:
			doorOpen = Quaternion.Euler (transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z + doorOpenAngle);
			break;
		case rotOrient.Y_Axis_Up:
			doorOpen = Quaternion.Euler (transform.localEulerAngles.x, transform.localEulerAngles.y + doorOpenAngle, transform.localEulerAngles.z);
			break;
		case rotOrient.X_Axis_Up:
			if (!applyRotationFix) {
				doorOpen = Quaternion.Euler (transform.localEulerAngles.x + doorOpenAngle, transform.localEulerAngles.y, transform.localEulerAngles.z);
			} else {
				{
					if (rotationAxisFix.Equals (rotFixAxis.Y)) {
						doorOpen = Quaternion.Euler (transform.localEulerAngles.x + 90, 90f, 270f);
					} else if (rotationAxisFix.Equals (rotFixAxis.Z)) {
						doorOpen = Quaternion.Euler (transform.localEulerAngles.x + 90, 270f, 90f);
					}
				}
			}
			break;
		}
		doorClosed = Quaternion.Euler (transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z);
	}

	public override void Activate ( CharacterManager characterManager){
        switch (doorMovement) {
            case doorType.Regular:
                if (doorStatus) {
                    StartCoroutine(this.moveDoor(doorClosed));
                    if (_audio != null) {
                        StartCoroutine(delayedCloseAudio(speed / 50f));
                    }
                } else {
                    StartCoroutine(this.moveDoor(doorOpen));
                    if (_audio != null) {
	                    AudioManager.instance.PlayOneShotSound("Scene", 
		                    _audio[0], 
		                    transform.position, 
		                    _audio.volume, 
		                    _audio.spatialBlend, 
		                    _audio.priority);
                    }
                }
                break;
            case doorType.Sliding:
                break;
    }
	}
	public override string GetText()
	{
		return _infoText;
	}
	IEnumerator delayedCloseAudio(float delay){
		yield return new WaitForSeconds (delay);
		AudioManager.instance.PlayOneShotSound("Scene", 
			_audio[1], 
			transform.position, 
			_audio.volume, 
			_audio.spatialBlend, 
			_audio.priority);
	}

	IEnumerator moveDoor(Quaternion target) {
		while (Quaternion.Angle (transform.localRotation, target) > 0.5f) {
			transform.localRotation = Quaternion.Slerp (transform.localRotation, target, Time.deltaTime * speed);
			yield return null;
		}
		doorStatus = !doorStatus;
		yield return null;
	}
}




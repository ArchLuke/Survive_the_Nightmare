using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class VictoryScreen : InteractiveItem
{

	[SerializeField] private FPSController _controller = null;
	[SerializeField] private string _infoText;
	[SerializeField] private AudioCollection _audio;
	[SerializeField] private GameObject _fade;
	[SerializeField] private GameObject _zombies;
	private float _startTime;
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
		_startTime = Time.time;
		base.Start();
		if (this.gameObject.isStatic) {
			Debug.Log ("This door has been set to static and won't be openable. Doorscript has been removed.");
			Destroy (this);
		}
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
        _controller.freezeMovement = true;
        KillGame();

	}

	void KillGame()
	{
		StartCoroutine(VictoryFade());
	}

	IEnumerator VictoryFade()
	{
		_zombies.SetActive(false);
		_fade.SetActive(true);
		
		Image _screenFade = _fade.GetComponentInChildren<Image>();
		Color _col = _screenFade.color;
		
		float timer = 0;
		float _currentFadeLevel = 0f;

		while (timer<0.5f)
		{
			timer+=Time.deltaTime;
			_currentFadeLevel = Mathf.Lerp( 0, 1, timer/0.5f);
			_col.a = _currentFadeLevel;
			_screenFade.color = _col;
			yield return null;
		}
		
		TextMeshProUGUI textCom = _fade.GetComponentInChildren<TextMeshProUGUI>();
		string words = "YOU SURVIVED";

		for(int i=1;i<words.Length+1;i++)
		{

			textCom.text = words.Substring(0, i);
			yield return new WaitForSeconds(0.1f);
		}
		
		yield return new WaitForSeconds(2f);
		
		if(ApplicationManager.instance)
			ApplicationManager.instance.LoadMainMenu();
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




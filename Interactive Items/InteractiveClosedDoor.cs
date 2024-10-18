using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class InteractiveClosedDoor : InteractiveItem {
	[SerializeField] private string _infoText;
	public override string GetText()
	{
		return _infoText;
	}
	
}




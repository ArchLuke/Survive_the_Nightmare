using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrapper : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
		DestroyImmediate(GameObject.Find("!ftraceLightmaps"));

		SceneManager.LoadScene("Bedroom");
	}
	
}

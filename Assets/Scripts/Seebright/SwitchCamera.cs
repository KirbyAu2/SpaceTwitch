using UnityEngine;
using System.Collections;

public class SwitchCamera : MonoBehaviour {

	public Camera cam1;
	public Camera cam2;
	

	// Use this for initialization
	void Start () {
		cam1.enabled = true;
		cam2.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space))
		{
			if(cam1.enabled)
			{
				cam1.enabled = false;
				cam2.enabled = true;
				Debug.Log("cam1 enabled");
			}
			else if(cam2.enabled)
			{
				cam2.enabled = false;
				cam1.enabled = true;
				Debug.Log("cam2 enabled");
			}
		}
	}
}

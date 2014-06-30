using UnityEngine;
using System.Collections;

public class Score : MonoBehaviour {
    public GUIStyle style;
    public static int CurrentScore = 0;

	void Start () {
	    
	}
	
	void Update () {
	
	}

    void OnGUI() {
        GUI.Label(new Rect(0, 0, Screen.width, Screen.height), CurrentScore.ToString(), style);
    }
}

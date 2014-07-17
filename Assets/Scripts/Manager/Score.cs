using UnityEngine;
using System.Collections;

public class Score : MonoBehaviour {
    public const int BUILD_UPS_NEEDED = 5;

    public GUIStyle style;
    public static int CurrentScore = 0;
    public static int CurrentMultiplier = 0;
    public static int BuildUp = 0;

	void Start () {
        style.fontSize = (int)ScreenUtil.getPixelHeight(style.fontSize);
        CurrentScore = 0;
	}
	
	void Update () {
        if (BUILD_UPS_NEEDED + CurrentMultiplier * 2 == BuildUp) {
            CurrentMultiplier++;
            BuildUp = 0;
        }
	}

    public static void submit() {
        KongregateAPI.Submit("High Score", CurrentScore);
    }

    void OnGUI() {
        GUI.Label(new Rect(0, 0, 0, 0), CurrentScore.ToString(), style);
        GUI.color = Color.red;
        GUI.Label(new Rect(0, style.fontSize, ScreenUtil.ScreenWidth, ScreenUtil.ScreenHeight), 
            "Multiplier : " + CurrentMultiplier.ToString(), style);
    }
}

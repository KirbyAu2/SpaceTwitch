using UnityEngine;
using System.Collections;

/*
 * Score class handles scoring and multiplier throughout the game
 * Submits score to Kongregate
 */
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
    
    //Submits score to Kongregate
    public static void submit() {
        KongregateAPI.Submit("High Score", CurrentScore);
    }

    /*
     * Shows current scrore and multiplier on top left corner of screen 
     */
    void OnGUI() {
        //Buffer needed incase Seebright is enabled
        float buffer = GameManager.Instance.enableSeebright ? ScreenUtil.getPixelHeight(50) : 0;
        GUIManager.DrawLabel(new Rect(buffer, buffer, 0, 0), CurrentScore.ToString(), style);
        GUI.color = Color.blue;
        GUIManager.DrawLabel(new Rect(buffer, buffer + style.fontSize, ScreenUtil.ScreenWidth, ScreenUtil.ScreenHeight), 
            "Multiplier : " + CurrentMultiplier.ToString(), style);
    }
}

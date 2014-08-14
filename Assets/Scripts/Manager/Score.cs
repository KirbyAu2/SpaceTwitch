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
#if UNITY_WEBPLAYER
        KongregateAPI.Submit("High Score", CurrentScore);
#else
        getTopHighScores();
#endif
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

    /**
     * Sets the top high scores
     */
    private static void getTopHighScores() {
        PlayerPrefs.Save();
        int _newScore = Score.CurrentScore;
        for (int i = 1; i <= 5; i++) {
            if (PlayerPrefs.GetInt("highscorePos" + i) < _newScore) //if New score is better
            {
                int _oldScore = PlayerPrefs.GetInt("highscorePos" + i); //Saves old best score
                PlayerPrefs.SetInt("highscorePos" + i, _newScore); //Sets New score as best score
                if (i < 5) {
                    int j = i + 1; //Move one position down
                    _newScore = PlayerPrefs.GetInt("highscorePos" + j); //saves next position score as new score 
                    PlayerPrefs.SetInt("highscorePos" + j, _oldScore); //sets old best score in that position
                }
            }
        }
        PlayerPrefs.Save();
    }

}

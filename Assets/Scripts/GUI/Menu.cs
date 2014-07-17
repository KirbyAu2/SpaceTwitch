
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * The Menu class draws the main menu.
 * As well as, the options menu and credits screen 
 */
public class Menu : MonoBehaviour {
    public GUIStyle style;
    public Texture2D logo;

    private bool _displayOptions = false;
    private bool _displayCredits = false;

    private Transform[] _levelModels;

    void Start () {
        style.fontSize = (int)ScreenUtil.getPixels(style.fontSize);
        _levelModels = GetComponentsInChildren<Transform>();
    }
	
    void Update () {
        Screen.lockCursor = false;

        transform.Rotate(new Vector3(0, 0, -.2f));

        foreach (Transform t in _levelModels)
        {
            t.Rotate(new Vector3(0, 0, .35f));
        }
    }

    /*
     * OnGUI is called for rendering and handling GUI events
     * Buttons in Main Menu for 'Play Game', 'Tutorial', 'Options', 'Credits'
     */
    void OnGUI() {
        GUI.DrawTexture(new Rect((Screen.width - ScreenUtil.getPixels(logo.width)) / 2 - ScreenUtil.getPixels(30), ScreenUtil.getPixels(100), ScreenUtil.getPixels(logo.width), ScreenUtil.getPixels(logo.height)), logo);
        if(!_displayOptions && !_displayCredits){
            if (GUI.Button(new Rect((Screen.width - ScreenUtil.getPixels(400)) / 2, Screen.height / 2, ScreenUtil.getPixels(400), style.fontSize), "Play Game", style)) {
                Application.LoadLevel(1);
            }
            if (GUI.Button(new Rect((Screen.width - ScreenUtil.getPixels(400)) / 2, Screen.height / 2 + ScreenUtil.getPixels(100), ScreenUtil.getPixels(400), style.fontSize), "Tutorial", style)) {
                Application.LoadLevel(2);
            }
            if (GUI.Button(new Rect((Screen.width - ScreenUtil.getPixels(400)) / 2, Screen.height / 2 + ScreenUtil.getPixels(200), ScreenUtil.getPixels(400), style.fontSize), "Options", style))
            {
                _displayOptions = true;
            }
            if (GUI.Button(new Rect((Screen.width - ScreenUtil.getPixels(400)) / 2, Screen.height / 2 + ScreenUtil.getPixels(300), ScreenUtil.getPixels(400), style.fontSize), "Credits", style)) {
                _displayCredits = true;
            }
        }

        //In Options Menu
        if (_displayOptions)
        {
            Color prev = GUI.color;
            GUI.color = Color.magenta;
            GUI.Label(new Rect(Screen.width/2 - 3*Screen.width/16, Screen.height/2 - ScreenUtil.getPixels(100), 3*Screen.width/8, ScreenUtil.getPixels(200)), "Options",style);
            GUI.color = prev;

            GUI.Label(new Rect((Screen.width + ScreenUtil.getPixels(200)) / 2, Screen.height / 2, ScreenUtil.getPixels(400), ScreenUtil.getPixels(200)), "Effects Volume", style);
            GameManager.effectsVolume = GUI.HorizontalSlider(new Rect((Screen.width + ScreenUtil.getPixels(200)) / 2, Screen.height / 2 + ScreenUtil.getPixels(100), ScreenUtil.getPixels(400), ScreenUtil.getPixels(50)),
                GameManager.effectsVolume, 0f, 1.0f);
            GUI.Label(new Rect((Screen.width - ScreenUtil.getPixels(1000)) / 2, Screen.height / 2, ScreenUtil.getPixels(400), ScreenUtil.getPixels(200)), "Music Volume", style);
            GameManager.musicVolume = GUI.HorizontalSlider(new Rect((Screen.width - ScreenUtil.getPixels(1000)) / 2, Screen.height / 2 + ScreenUtil.getPixels(100), ScreenUtil.getPixels(400), ScreenUtil.getPixels(50)),
                GameManager.musicVolume, 0f, 1.0f);
            GUI.Label(new Rect((Screen.width - ScreenUtil.getPixels(400)) / 2, Screen.height / 2 + ScreenUtil.getPixels(150), ScreenUtil.getPixels(400), ScreenUtil.getPixels(200)), "Sensitivity", style);
            GameManager.mouseSensitivity = GUI.HorizontalSlider(new Rect((Screen.width - ScreenUtil.getPixels(400))/ 2, Screen.height / 2 + ScreenUtil.getPixels(230), ScreenUtil.getPixels(400), ScreenUtil.getPixels(50)),
                GameManager.mouseSensitivity, 0.1f, 0.5f);

            if (GameManager.Instance != null) {
                GameManager.Instance.UpdateMusicVolume();
            }

            if (GUI.Button(new Rect((Screen.width - ScreenUtil.getPixels(200)) / 2, Screen.height - ScreenUtil.getPixels(150), ScreenUtil.getPixels(200), style.fontSize), "Back",style)) {
                _displayOptions = false;
                // update sensitivity when options are closed
                if (GameManager.Instance != null) {
                    foreach (Player player in GameManager.Instance.CurrentPlayerShips) {
                        player.UpdateSensitivity();
                    }
                }
            }
        }
        //Credits Screen
        //Displays Credits
        if (_displayCredits) {
            Color prev = GUI.color;
            GUI.color = Color.magenta;
            GUI.Label(new Rect(Screen.width / 2 - 3 * Screen.width / 16, Screen.height / 2 - ScreenUtil.getPixels(100), 3 * Screen.width / 8, ScreenUtil.getPixels(200)), "Credits", style);
            GUI.color = prev;
            GUI.color = Color.green;
            GUI.Label(new Rect(Screen.width / 2 - 3 * Screen.width / 16, Screen.height / 2,
                3 * Screen.width / 8, ScreenUtil.getPixels(200)), "Anthony Dao", style);
            style.fontSize /= 2;
            GUI.color = Color.white;
            GUI.Label(new Rect(Screen.width / 2 - 3 * Screen.width / 16, Screen.height / 2 + ScreenUtil.getPixels(55),
                3 * Screen.width / 8, ScreenUtil.getPixels(200)), "Programming ( Enemies-Menus-GUI ) / Design", style);
            style.fontSize *= 2;
            GUI.color = Color.green;
            GUI.Label(new Rect(Screen.width / 2 - 3 * Screen.width / 16, Screen.height / 2 + ScreenUtil.getPixels(120),
                3 * Screen.width / 8, ScreenUtil.getPixels(200)), "Chase Khamashta", style);
            style.fontSize /= 2;
            GUI.color = Color.white;
            GUI.Label(new Rect(Screen.width / 2 - 3 * Screen.width / 16, Screen.height / 2 + ScreenUtil.getPixels(170),
                3 * Screen.width / 8, ScreenUtil.getPixels(200)), "Programming ( Player-Controls-Enemies-Powerups ) / Design", style);
            style.fontSize *= 2;
            GUI.color = Color.green;
            GUI.Label(new Rect(Screen.width / 2 - 3 * Screen.width / 16, Screen.height / 2 + ScreenUtil.getPixels(240),
                3 * Screen.width / 8, ScreenUtil.getPixels(200)), "Orlando Salvatore", style);
            style.fontSize /= 2;
            GUI.color = Color.white;
            GUI.Label(new Rect(Screen.width / 2 - 3 * Screen.width / 16, Screen.height / 2 + ScreenUtil.getPixels(290),
                3 * Screen.width / 8, ScreenUtil.getPixels(200)), "Programming ( Enemies-Level Generation-Transitions-Tutorial-Menus-Scoring )/ Art / Music / Design", style);
            style.fontSize *= 2;

            if (GUI.Button(new Rect((Screen.width - ScreenUtil.getPixels(200)) / 2, Screen.height - ScreenUtil.getPixels(100), ScreenUtil.getPixels(200), style.fontSize), "Back", style)) {
                _displayCredits = false;
            }
        }
    }
}

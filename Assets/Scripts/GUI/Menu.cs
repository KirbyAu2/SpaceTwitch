
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
        style.fontSize = (int)ScreenUtil.getPixelHeight(style.fontSize);
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
        GUI.DrawTexture(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelHeight(logo.width)) / 2 - ScreenUtil.getPixelHeight(30),
            ScreenUtil.getPixelHeight(100), ScreenUtil.getPixelHeight(logo.width), ScreenUtil.getPixelHeight(logo.height)), logo);
        if(!_displayOptions && !_displayCredits){
            if (GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2, ScreenUtil.ScreenHeight / 2, 
                ScreenUtil.getPixelWidth(400), style.fontSize), "Play Game", style)) {
                Application.LoadLevel(1);
            }
            if (GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2, 
                ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(100), ScreenUtil.getPixelWidth(400), style.fontSize), "Tutorial", style)) {
                Application.LoadLevel(2);
            }
            if (GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2, 
                ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(200), ScreenUtil.getPixelWidth(400), style.fontSize), "Options", style))
            {
                _displayOptions = true;
            }
            if (GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2, 
                ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelWidth(300), ScreenUtil.getPixelWidth(400), style.fontSize), "Credits", style)) {
                _displayCredits = true;
            }
        }

        //In Options Menu
        if (_displayOptions)
        {
            Color prev = GUI.color;
            GUI.color = Color.magenta;
            GUI.Label(new Rect(ScreenUtil.ScreenWidth / 2 - 3 * ScreenUtil.ScreenWidth / 16, ScreenUtil.ScreenHeight / 2 - 
                ScreenUtil.getPixelHeight(100), 3 * ScreenUtil.ScreenWidth / 8, ScreenUtil.getPixelHeight(200)), "Options", style);
            GUI.color = prev;
            if (GameManager.Instance.enableSeebright == false)
            {
                //SFX Volume
                GUI.Label(new Rect((ScreenUtil.ScreenWidth + ScreenUtil.getPixelWidth(200)) / 2, ScreenUtil.ScreenHeight / 2,
                    ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(200)), "Effects Volume", style);
                GameManager.effectsVolume = GUI.HorizontalSlider(new Rect((ScreenUtil.ScreenWidth + ScreenUtil.getPixelWidth(200)) / 2,
                    ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(100), ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(50)),
                    GameManager.effectsVolume, 0f, 1.0f);
                //Music Volume
                GUI.Label(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(1000)) / 2, ScreenUtil.ScreenHeight / 2,
                    ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(200)), "Music Volume", style);
                GameManager.musicVolume = GUI.HorizontalSlider(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(1000)) / 2,
                    ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(100), ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(50)),
                    GameManager.musicVolume, 0f, 1.0f);
                //Sensitivity Control
                GUI.Label(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2,
                    ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(150), ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(200)),
                    "Sensitivity", style);
                GameManager.mouseSensitivity = GUI.HorizontalSlider(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2,
                    ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(230), ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(50)),
                    GameManager.mouseSensitivity, 0.1f, 0.5f);
            }
            else if (GameManager.Instance.enableSeebright == true)
            {
                //SFX Volume
                GUI.Label(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2, ScreenUtil.ScreenHeight / 2,
                    ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(200)), "Effects Volume", style);
                GameManager.effectsVolume = GUI.HorizontalSlider(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2,
                    ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(70), ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(50)),
                    GameManager.effectsVolume, 0f, 1.0f);
                //Music Volume
                GUI.Label(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2, ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(120),
                    ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(200)), "Music Volume", style);
                GameManager.musicVolume = GUI.HorizontalSlider(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400))/ 2,
                    ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(190), ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(50)),
                    GameManager.musicVolume, 0f, 1.0f);
                //Sensitivity Control
                GUI.Label(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2,
                    ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(240), ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(200)),
                    "Sensitivity", style);
                GameManager.mouseSensitivity = GUI.HorizontalSlider(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2,
                    ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(310), ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(50)),
                    GameManager.mouseSensitivity, 0.1f, 0.5f);
            }
            if (GameManager.Instance != null) {
                GameManager.Instance.UpdateMusicVolume();
            }
            
            if (GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(200)) / 2, 
                ScreenUtil.ScreenHeight - ScreenUtil.getPixelHeight(150), ScreenUtil.getPixelWidth(200), style.fontSize), "Back", style)) {
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
            GUI.Label(new Rect(ScreenUtil.ScreenWidth / 2 - 3 * ScreenUtil.ScreenWidth / 16, ScreenUtil.ScreenHeight / 2 - ScreenUtil.getPixelHeight(100), 
                3 * ScreenUtil.ScreenWidth / 8, ScreenUtil.getPixelHeight(200)), "Credits", style);
            GUI.color = prev;
            GUI.color = Color.green;
            GUI.Label(new Rect(ScreenUtil.ScreenWidth / 2 - 3 * ScreenUtil.ScreenWidth / 16, ScreenUtil.ScreenHeight / 2,
                3 * ScreenUtil.ScreenWidth / 8, ScreenUtil.getPixelHeight(200)), "Anthony Dao", style);
            style.fontSize /= 2;
            GUI.color = Color.white;
            GUI.Label(new Rect(ScreenUtil.ScreenWidth / 2 - 3 * ScreenUtil.ScreenWidth / 16, ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(55),
                3 * ScreenUtil.ScreenWidth / 8, ScreenUtil.getPixelHeight(200)), "Programming ( Enemies-Menus-GUI ) / Design", style);
            style.fontSize *= 2;
            GUI.color = Color.green;
            GUI.Label(new Rect(ScreenUtil.ScreenWidth / 2 - 3 * ScreenUtil.ScreenWidth / 16, ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(120),
                3 * ScreenUtil.ScreenWidth / 8, ScreenUtil.getPixelHeight(200)), "Chase Khamashta", style);
            style.fontSize /= 2;
            GUI.color = Color.white;
            GUI.Label(new Rect(ScreenUtil.ScreenWidth / 2 - 3 * ScreenUtil.ScreenWidth / 16, ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(170),
                3 * ScreenUtil.ScreenWidth / 8, ScreenUtil.getPixelHeight(200)), "Programming ( Player-Controls-Enemies-Powerups ) / Design", style);
            style.fontSize *= 2;
            GUI.color = Color.green;
            GUI.Label(new Rect(ScreenUtil.ScreenWidth / 2 - 3 * ScreenUtil.ScreenWidth / 16, ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(240),
                3 * ScreenUtil.ScreenWidth / 8, ScreenUtil.getPixelHeight(200)), "Orlando Salvatore", style);
            style.fontSize /= 2;
            GUI.color = Color.white;
            GUI.Label(new Rect(ScreenUtil.ScreenWidth / 2 - 3 * ScreenUtil.ScreenWidth / 16, ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(290),
                3 * ScreenUtil.ScreenWidth / 8, ScreenUtil.getPixelHeight(200)), 
                "Programming", style);
            GUI.Label(new Rect(ScreenUtil.ScreenWidth / 2 - 3 * ScreenUtil.ScreenWidth / 16, ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(320),
                3 * ScreenUtil.ScreenWidth / 8, ScreenUtil.getPixelHeight(200)),
                "( Enemies-Level Generation-Transitions-Tutorial-Menus-Scoring )", style);
            GUI.Label(new Rect(ScreenUtil.ScreenWidth / 2 - 3 * ScreenUtil.ScreenWidth / 16, ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(350),
                3 * ScreenUtil.ScreenWidth / 8, ScreenUtil.getPixelHeight(200)),
                "Art / Music / Design", style);
            style.fontSize *= 2;
            if (GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(200)) / 2, 
                ScreenUtil.ScreenHeight - ScreenUtil.getPixelHeight(100), ScreenUtil.getPixelWidth(200), style.fontSize), "Back", style)) {
                _displayCredits = false;
            }
        }
    }
}

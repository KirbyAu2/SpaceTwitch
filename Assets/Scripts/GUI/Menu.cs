
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
    private bool _focusChanged = false;
    private float _focusTimerMax = .2f;
    private float _focusTimer = 0;
    private int _focusID = -1;
    private bool _sliderSelecter = false;

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

    int ManageFocus(int ID, int length)
    {
        GUI.FocusControl(ID.ToString());

        _focusTimer += .01f;
        if (SBRemote.GetJoystickDelta(SBRemote.JOY_VERTICAL) < 0 && ID < length && _focusTimer > _focusTimerMax)
        {
            _focusTimer = 0;
            ID++;
        }
        if (SBRemote.GetJoystickDelta(SBRemote.JOY_VERTICAL) > 0 && ID > 0 && _focusTimer > _focusTimerMax)
        {
            _focusTimer = 0;
            ID--;
        }
        return ID;
    }

    /*
     * OnGUI is called for rendering and handling GUI events
     * Buttons in Main Menu for 'Play Game', 'Tutorial', 'Options', 'Credits'
     */
    void OnGUI() {
        GUI.DrawTexture(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelHeight(logo.width)) / 2 - ScreenUtil.getPixelHeight(30),
            ScreenUtil.getPixelHeight(100), ScreenUtil.getPixelHeight(logo.width), ScreenUtil.getPixelHeight(logo.height)), logo);
        if(!_displayOptions && !_displayCredits){
            GUI.SetNextControlName("0");
            if (GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2, ScreenUtil.ScreenHeight / 2, 
                ScreenUtil.getPixelWidth(400), style.fontSize), "Play Game", style)) {
                Application.LoadLevel(1);
            }
            GUI.SetNextControlName("1");
            if (GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2, 
                ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(100), ScreenUtil.getPixelWidth(400), style.fontSize), "Tutorial", style)) {
                Application.LoadLevel(2);
            }
            GUI.SetNextControlName("2");
            if (GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2, 
                ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelWidth(300), ScreenUtil.getPixelWidth(400), style.fontSize), "Credits", style)) {
                _displayCredits = true;
            }
            GUI.SetNextControlName("3");
            if (GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2, 
                ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(200), ScreenUtil.getPixelWidth(400), style.fontSize), "Options", style))
            {
                _displayOptions = true;
            }
            _focusID = ManageFocus(_focusID, 3);
            if (SBRemote.GetButtonDown(SBRemote.BUTTON_SELECT))
            {
                if (_focusID < 0)
                {
                    return;
                }
                else if (_focusID == 0)
                {
                    Application.LoadLevel(1);
                }
                else if (_focusID == 1)
                {
                    Application.LoadLevel(2);
                }
                else if (_focusID == 2)
                {
                    _displayCredits = true;
                    _focusID = -1;
                }
                else if (_focusID == 3)
                {
                    _displayOptions = true;
                    _focusID = -1;
                }
            }

        }
        

        //In Options Menu
        else if (_displayOptions)
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
                GUI.SetNextControlName("0");
                GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2, ScreenUtil.ScreenHeight / 2,
                    ScreenUtil.getPixelWidth(400), style.fontSize), "Effects Volume", style);
                GameManager.effectsVolume = GUI.HorizontalSlider(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2,
                    ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(70), ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(50)),
                    GameManager.effectsVolume, 0f, 1.0f);
                //Music Volume
                GUI.SetNextControlName("1");
                GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2, ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(120),
                    ScreenUtil.getPixelWidth(400), style.fontSize), "Music Volume", style);
                GameManager.musicVolume = GUI.HorizontalSlider(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400))/ 2,
                    ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(190), ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(50)),
                    GameManager.musicVolume, 0f, 1.0f);
                //Sensitivity Control
                GUI.SetNextControlName("2");
                GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2,
                    ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(240), ScreenUtil.getPixelWidth(400), style.fontSize),
                    "Sensitivity", style);
                GameManager.mouseSensitivity = GUI.HorizontalSlider(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2,
                    ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(310), ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(50)),
                    GameManager.mouseSensitivity, 0.1f, 0.5f);
            }
            if (GameManager.Instance != null) {
                GameManager.Instance.UpdateMusicVolume();
            }
            GUI.SetNextControlName("3");
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
            if (!_sliderSelecter)
            {
                _focusID = ManageFocus(_focusID, 3);
            }
            if (SBRemote.GetButtonDown(SBRemote.BUTTON_SELECT))
            {
                if (_focusID < 0)
                {
                    return;
                }
                else if (_focusID == 0 || _focusID == 1 ||_focusID == 2)
                {
                    if (_sliderSelecter)
                        _sliderSelecter = false;
                    else
                        _sliderSelecter = true;
                }
                else if (_focusID == 3)
                {
                    _displayOptions = false;
                    _focusID = -1;
                }
            }
            if (SBRemote.GetJoystickDelta(SBRemote.JOY_HORIZONTAL) > 0 && _sliderSelecter) {
                if (_focusID == 0 && GameManager.effectsVolume < 1)
                {
                    GameManager.effectsVolume += .01f;
                }
                else if (_focusID == 1 && GameManager.musicVolume < 1)
                {
                    GameManager.musicVolume += .01f;
                }
                else if (_focusID == 2 && GameManager.mouseSensitivity < .5f)
                {
                    GameManager.mouseSensitivity += .01f;
                }
            }
            if (SBRemote.GetJoystickDelta(SBRemote.JOY_HORIZONTAL) < 0 && _sliderSelecter) {
                if (_focusID == 0 && GameManager.effectsVolume > 0)
                {
                    GameManager.effectsVolume -= .01f;
                }
                else if (_focusID == 1 && GameManager.musicVolume > 0)
                {
                    GameManager.musicVolume -= .01f;
                }
                else if (_focusID == 2 && GameManager.mouseSensitivity > .1f)
                {
                    GameManager.mouseSensitivity -= .01f;
                }
            }
        }
        //Credits Screen
        //Displays Credits
        else if (_displayCredits) {
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
            GUI.SetNextControlName("0");
            if (GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(200)) / 2, 
                ScreenUtil.ScreenHeight - ScreenUtil.getPixelHeight(100), ScreenUtil.getPixelWidth(200), style.fontSize), "Back", style)) {
                _displayCredits = false;
            }
           
            _focusID = ManageFocus(_focusID, 0);
            if (SBRemote.GetButtonDown(SBRemote.BUTTON_SELECT))
            {
                if (_focusID < 0)
                {
                    return;
                }
                else if (_focusID == 0)
                {
                    _displayCredits = false ;
                }
            }

        }
        else
        {
            _focusID = -1;
        }
    }
}

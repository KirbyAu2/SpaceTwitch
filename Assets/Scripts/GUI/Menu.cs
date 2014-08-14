
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

    private bool _displayHighScore = false;
    private bool _displayOptions = false;
    private bool _displayCredits = false;
    private bool _focusChanged = false;
    private float _focusTimerMax = .2f;
    private float _focusTimer = 0;
    private int _focusID = -1;
    private int _newScore = 0;
    private int _oldScore = 0;
    private bool _sliderSelecter = false;
    private GUIStyle _highlightStyle;

    private Transform[] _levelModels;

    void Start () {
        style.fontSize = (int)ScreenUtil.getPixelHeight(style.fontSize);
        _levelModels = GetComponentsInChildren<Transform>();
        _highlightStyle = new GUIStyle(style);
    }
	
    void Update () {
        Screen.lockCursor = false;

        transform.Rotate(new Vector3(0, 0, -.2f));

        foreach (Transform t in _levelModels)
        {
            t.Rotate(new Vector3(0, 0, .35f));
        }
    }

    private int ManageFocus(int ID, int length)
    {
        if (!GameManager.Instance.enableSeebright || !SBRemote.remoteStatus) {
            return 0;
        }
        GUI.FocusControl(ID.ToString());

        _focusTimer += .01f;
        if (SBRemote.GetAxis(SBRemote.JOY_VERTICAL) < -0.5f && ID < length && _focusTimer > _focusTimerMax) {
            _focusTimer = 0;
            ID++;
        }
        if (SBRemote.GetAxis(SBRemote.JOY_VERTICAL) > 0.5f && ID > 0 && _focusTimer > _focusTimerMax) {
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
        GUIManager.DrawTexture(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelHeight(logo.width)) / 2 - ScreenUtil.getPixelHeight(30),
                ScreenUtil.getPixelHeight(100), ScreenUtil.getPixelHeight(logo.width), ScreenUtil.getPixelHeight(logo.height)), logo);

        GUIStyle _tempStyle = new GUIStyle(_highlightStyle);
        _tempStyle.fontSize /= 2;
        GUIManager.DrawLabel(new Rect(ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(260), ScreenUtil.ScreenHeight - ScreenUtil.getPixelHeight(50),
                ScreenUtil.getPixelWidth(50), ScreenUtil.getPixelHeight(100)), GameManager.versionID, _tempStyle);

#if !UNITY_IPHONE && !UNITY_WEBPLAYER
        // temporary manual exit program
        GUIManager.DrawLabel(new Rect(10, 10, 300, 25), "Press Escape to quit", _tempStyle);
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
#endif

        if(!_displayOptions && !_displayCredits && !_displayHighScore) {
            drawNormalMenu();
        }
        //In Options Menu
        else if (_displayOptions) {
            drawOptions();
        }
        //In Credits Screen
        else if (_displayCredits) {
            drawCredits();
        }
#if !UNITY_WEBPLAYER
        //In High Score Screen
        else if (_displayHighScore){
            drawHighScore();
        }
#endif
        else {
            _focusID = -1;
        }
    }

    /**
     * Draws the options menu
     */
    private void drawOptions() {
        Color prev = GUI.color;
        GUI.color = Color.magenta;
        GUIManager.DrawLabel(new Rect(ScreenUtil.ScreenWidth / 2 - 3 * ScreenUtil.ScreenWidth / 16, ScreenUtil.ScreenHeight / 2 -
            ScreenUtil.getPixelHeight(100), 3 * ScreenUtil.ScreenWidth / 8, ScreenUtil.getPixelHeight(200)), "Options", style);
        GUI.color = prev;
        if (!GameManager.Instance.enableSeebright) {
            //SFX Volume
            GUI.Label(new Rect((ScreenUtil.ScreenWidth + ScreenUtil.getPixelWidth(200)) / 2, ScreenUtil.ScreenHeight / 2 - ScreenUtil.getPixelHeight(50),
                ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(200)), "Effects Volume", style);
            GameManager.effectsVolume = GUI.HorizontalSlider(new Rect((ScreenUtil.ScreenWidth + ScreenUtil.getPixelWidth(200)) / 2,
                ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(50), ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(50)),
                GameManager.effectsVolume, 0f, 1.0f);
            //Music Volume
            GUI.Label(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(1000)) / 2, ScreenUtil.ScreenHeight / 2 - ScreenUtil.getPixelHeight(50),
                ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(200)), "Music Volume", style);
            GameManager.musicVolume = GUI.HorizontalSlider(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(1000)) / 2,
                ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(50), ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(50)),
                GameManager.musicVolume, 0f, 1.0f);
#if !UNITY_IPHONE
            //Sensitivity Control
            GUI.Label(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2,
                ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(100), ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(200)),
                "Sensitivity", style);
            GameManager.mouseSensitivity = GUI.HorizontalSlider(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2,
                ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(180), ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(50)),
                GameManager.mouseSensitivity, 0.1f, 0.5f);
#endif
        }
        else 
        {
            //SeeBright Enabled
            //SFX Volume
            GUI.SetNextControlName("0");
            GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2, ScreenUtil.ScreenHeight / 2,
                ScreenUtil.getPixelWidth(400), style.fontSize), new GUIContent("Effects Volume", "0"), style);
            GameManager.effectsVolume = GUI.HorizontalSlider(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2,
                ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(70), ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(50)),
                GameManager.effectsVolume, 0f, 1.0f);
            if (GUI.tooltip == "0") {
                _highlightStyle.normal = style.hover;
            }
            GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2 + ScreenUtil.ScreenWidth, ScreenUtil.ScreenHeight / 2,
                ScreenUtil.getPixelWidth(400), style.fontSize), "Effects Volume", _highlightStyle);
            _highlightStyle.normal = style.normal;
            GameManager.effectsVolume = GUI.HorizontalSlider(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2 + ScreenUtil.ScreenWidth,
                ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(70), ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(50)),
                GameManager.effectsVolume, 0f, 1.0f);

            //Music Volume
            GUI.SetNextControlName("1");
            GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2, ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(120),
                ScreenUtil.getPixelWidth(400), style.fontSize), new GUIContent("Music Volume", "1"), style);
            GameManager.musicVolume = GUI.HorizontalSlider(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2,
                ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(190), ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(50)),
                GameManager.musicVolume, 0f, 1.0f);
            if (GUI.tooltip == "1") {
                _highlightStyle.normal = style.hover;
            }
            GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2 + ScreenUtil.ScreenWidth, ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(120),
                ScreenUtil.getPixelWidth(400), style.fontSize), "Music Volume", _highlightStyle);
            _highlightStyle.normal = style.normal;
            GameManager.musicVolume = GUI.HorizontalSlider(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2 + ScreenUtil.ScreenWidth,
                ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(190), ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(50)),
                GameManager.musicVolume, 0f, 1.0f);
        }

        if (GameManager.Instance) {
            GameManager.Instance.updateMusicVolume();
        }

#if UNITY_IPHONE
        GUI.SetNextControlName("3");
        if (GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(200)) / 2,
            ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(230), ScreenUtil.getPixelWidth(200), 
            style.fontSize), new GUIContent((GameManager.invertedJoystick) ? "Inverted Joystick: On" : "Inverted Joystick: Off", "3"), style)) {
                GameManager.invertedJoystick = !GameManager.invertedJoystick;
        }
        if (GUI.tooltip == "3") {
            _highlightStyle.normal = style.hover;
        }
        if (GameManager.Instance.enableSeebright) {
            GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(200)) / 2 + ScreenUtil.ScreenWidth,
                ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(230), ScreenUtil.getPixelWidth(200), style.fontSize), 
                (GameManager.invertedJoystick) ? "Inverted Joystick: On" : "Inverted Joystick: Off", _highlightStyle);
        }
        _highlightStyle.normal = style.normal;
        _focusID = ManageFocus(_focusID, 3);

        if (!GameManager.Instance.enableSeebright) {
            if (GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(200)) / 2,
                    ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(330), ScreenUtil.getPixelWidth(200),
                    style.fontSize), new GUIContent((GameManager.invertedControls) ? "Inverted Controls: On" : "Inverted Controls: Off"), style)) {
                GameManager.invertedControls = !GameManager.invertedControls;
            }
        }
#endif

        GUI.SetNextControlName("4");
        if (GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(200)) / 2,
            (GameManager.Instance.enableSeebright) ? ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(330) : ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(430), 
            ScreenUtil.getPixelWidth(200), style.fontSize), new GUIContent("Back", "4"), style)) {
            _displayOptions = false;
            // update sensitivity when options are closed
            if (GameManager.Instance != null) {
                foreach (Player player in GameManager.Instance.CurrentPlayerShips) {
                    player.UpdateSensitivity();
                }
            }
        }
        if (GUI.tooltip == "4") {
            _highlightStyle.normal = style.hover;
        }
        GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(200)) / 2 + ScreenUtil.ScreenWidth,
            ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(430), ScreenUtil.getPixelWidth(200), style.fontSize), "Back", _highlightStyle);
        _highlightStyle.normal = style.normal;
        _focusID = ManageFocus(_focusID, 4);



        if (!GameManager.Instance.enableSeebright || !SBRemote.remoteStatus) {
            return;
        }
        if (SBRemote.GetButtonDown(SBRemote.BUTTON_SELECT)) {
            if (_focusID < 0) {
                return;
            } else if (_focusID == 3) {
                _displayOptions = false;
                _focusID = -1;
            }
        }
        if (SBRemote.GetAxis(SBRemote.JOY_HORIZONTAL) > 0.5f) {
            if (_focusID == 0 && GameManager.effectsVolume < 1) {
                GameManager.effectsVolume += .01f;
            } else if (_focusID == 1 && GameManager.musicVolume < 1) {
                GameManager.musicVolume += .01f;
            } else if (_focusID == 2 && GameManager.mouseSensitivity < .5f) {
                GameManager.mouseSensitivity += .005f;
            }
        }
        if (SBRemote.GetAxis(SBRemote.JOY_HORIZONTAL) < -0.5f) {
            if (_focusID == 0 && GameManager.effectsVolume > 0) {
                GameManager.effectsVolume -= .01f;
            } else if (_focusID == 1 && GameManager.musicVolume > 0) {
                GameManager.musicVolume -= .01f;
            } else if (_focusID == 2 && GameManager.mouseSensitivity > .1f) {
                GameManager.mouseSensitivity -= .005f;
            }
        }
    }

    /**
     * Draws the credits menu
     */
    private void drawCredits() {
        Color prev = GUI.color;
        GUI.color = Color.magenta;
        GUIManager.DrawLabel(new Rect(ScreenUtil.ScreenWidth / 2 - 3 * ScreenUtil.ScreenWidth / 16, ScreenUtil.ScreenHeight / 2 - ScreenUtil.getPixelHeight(100),
                3 * ScreenUtil.ScreenWidth / 8, ScreenUtil.getPixelHeight(200)), "Credits", style);
        GUI.color = prev;
        GUI.color = Color.green;
        GUIManager.DrawLabel(new Rect(ScreenUtil.ScreenWidth / 2 - 3 * ScreenUtil.ScreenWidth / 16, ScreenUtil.ScreenHeight / 2,
            3 * ScreenUtil.ScreenWidth / 8, ScreenUtil.getPixelHeight(200)), "Anthony Dao", style);
        style.fontSize /= 2;
        GUI.color = Color.white;
        GUIManager.DrawLabel(new Rect(ScreenUtil.ScreenWidth / 2 - 3 * ScreenUtil.ScreenWidth / 16, ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(55),
            3 * ScreenUtil.ScreenWidth / 8, ScreenUtil.getPixelHeight(200)), "Programming ( Enemies-Menus-GUI ) / Design", style);
        style.fontSize *= 2;
        GUI.color = Color.green;
        GUIManager.DrawLabel(new Rect(ScreenUtil.ScreenWidth / 2 - 3 * ScreenUtil.ScreenWidth / 16, ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(120),
            3 * ScreenUtil.ScreenWidth / 8, ScreenUtil.getPixelHeight(200)), "Chase Khamashta", style);
        style.fontSize /= 2;
        GUI.color = Color.white;
        GUIManager.DrawLabel(new Rect(ScreenUtil.ScreenWidth / 2 - 3 * ScreenUtil.ScreenWidth / 16, ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(170),
            3 * ScreenUtil.ScreenWidth / 8, ScreenUtil.getPixelHeight(200)), "Programming ( Player-Controls-Enemies-Powerups ) / Design", style);
        style.fontSize *= 2;
        GUI.color = Color.green;
        GUIManager.DrawLabel(new Rect(ScreenUtil.ScreenWidth / 2 - 3 * ScreenUtil.ScreenWidth / 16, ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(240),
            3 * ScreenUtil.ScreenWidth / 8, ScreenUtil.getPixelHeight(200)), "Orlando Salvatore", style);
        style.fontSize /= 2;
        GUI.color = Color.white;
        GUIManager.DrawLabel(new Rect(ScreenUtil.ScreenWidth / 2 - 3 * ScreenUtil.ScreenWidth / 16, ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(290),
            3 * ScreenUtil.ScreenWidth / 8, ScreenUtil.getPixelHeight(200)), "Programming", style);
        GUIManager.DrawLabel(new Rect(ScreenUtil.ScreenWidth / 2 - 3 * ScreenUtil.ScreenWidth / 16 + ScreenUtil.ScreenWidth, ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(320),
                3 * ScreenUtil.ScreenWidth / 8, ScreenUtil.getPixelHeight(200)),
                "( Enemies-Level Generation-Transitions-Tutorial-Menus-Scoring )", style);
        GUIManager.DrawLabel(new Rect(ScreenUtil.ScreenWidth / 2 - 3 * ScreenUtil.ScreenWidth / 16, ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(350),
            3 * ScreenUtil.ScreenWidth / 8, ScreenUtil.getPixelHeight(200)),
            "Art / Music / Design", style);

        style.fontSize *= 2;
        GUI.SetNextControlName("0");
        if (GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(200)) / 2,
            ScreenUtil.ScreenHeight - ScreenUtil.getPixelHeight(100), ScreenUtil.getPixelWidth(200), style.fontSize), new GUIContent("Back", "0"), style)) {
            _displayCredits = false;
        }
        if (GUI.tooltip == "0") {
            _highlightStyle.normal = style.hover;
        }
        if (GameManager.Instance.enableSeebright) {
            GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(200)) / 2 + ScreenUtil.ScreenWidth,
            ScreenUtil.ScreenHeight - ScreenUtil.getPixelHeight(100), ScreenUtil.getPixelWidth(200), style.fontSize), "Back", _highlightStyle);
        }
        _highlightStyle.normal = style.normal;
        //Joystick Menu Navigation
        _focusID = ManageFocus(_focusID, 0);
        if (!SBRemote.remoteStatus || GameManager.Instance.enableSeebright) {
            return;
        }
        if (SBRemote.GetButtonDown(SBRemote.BUTTON_SELECT)) {
            if (_focusID < 0) {
                return;
            } else if (_focusID == 0) {
                _displayCredits = false;
            }
        }
    }

    /**
     * Draws the HighScore menu
     */
    private void drawHighScore(){
        Color prev = GUI.color;
        GUI.color = Color.magenta;
        GUIManager.DrawLabel(new Rect(ScreenUtil.ScreenWidth / 2 - 3 * ScreenUtil.ScreenWidth / 16, ScreenUtil.ScreenHeight / 2 - ScreenUtil.getPixelHeight(100),
                3 * ScreenUtil.ScreenWidth / 8, ScreenUtil.getPixelHeight(200)), "High Scores", style);
        GUI.color = prev;
        GUI.color = Color.green;
        GUIManager.DrawLabel(new Rect(ScreenUtil.ScreenWidth / 2 - 3 * ScreenUtil.ScreenWidth / 16, ScreenUtil.ScreenHeight / 2,
                3 * ScreenUtil.ScreenWidth / 8, ScreenUtil.getPixelHeight(200)), "1)   " + PlayerPrefs.GetInt("highscorePos"+1), style);
        GUIManager.DrawLabel(new Rect(ScreenUtil.ScreenWidth / 2 - 3 * ScreenUtil.ScreenWidth / 16, ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(70),
                3 * ScreenUtil.ScreenWidth / 8, ScreenUtil.getPixelHeight(200)), "2)   " + PlayerPrefs.GetInt("highscorePos" + 2), style);
        GUIManager.DrawLabel(new Rect(ScreenUtil.ScreenWidth / 2 - 3 * ScreenUtil.ScreenWidth / 16, ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(140),
                3 * ScreenUtil.ScreenWidth / 8, ScreenUtil.getPixelHeight(200)), "3)   " + PlayerPrefs.GetInt("highscorePos" + 3), style);
        GUIManager.DrawLabel(new Rect(ScreenUtil.ScreenWidth / 2 - 3 * ScreenUtil.ScreenWidth / 16, ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(210),
                3 * ScreenUtil.ScreenWidth / 8, ScreenUtil.getPixelHeight(200)), "4)   " + PlayerPrefs.GetInt("highscorePos" + 4), style);
        GUIManager.DrawLabel(new Rect(ScreenUtil.ScreenWidth / 2 - 3 * ScreenUtil.ScreenWidth / 16, ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(280),
                3 * ScreenUtil.ScreenWidth / 8, ScreenUtil.getPixelHeight(200)), "5)   " + PlayerPrefs.GetInt("highscorePos" + 5), style);

        //Back Button
            GUI.color = Color.white;
            GUI.SetNextControlName("0");
        if (GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(200)) / 2,
            ScreenUtil.ScreenHeight - ScreenUtil.getPixelHeight(100), ScreenUtil.getPixelWidth(200), style.fontSize), new GUIContent("Back", "0"), style))
        {
            _displayHighScore = false;
        }
        if (GUI.tooltip == "0")
        {
            _highlightStyle.normal = style.hover;
        }
        if (GameManager.Instance.enableSeebright)
        {
            GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(200)) / 2 + ScreenUtil.ScreenWidth,
            ScreenUtil.ScreenHeight - ScreenUtil.getPixelHeight(100), ScreenUtil.getPixelWidth(200), style.fontSize), "Back", _highlightStyle);
        }
        _highlightStyle.normal = style.normal;
        //Joystick Menu Navigation
        _focusID = ManageFocus(_focusID, 0);
        if(GameManager.Instance.enableSeebright && SBRemote.remoteStatus) {
            if (SBRemote.GetButtonDown(SBRemote.BUTTON_SELECT))
            {
                if (_focusID < 0)
                {
                    return;
                }
                else if (_focusID == 0)
                {
                    _displayHighScore = false;
                }
            }
        }
    }

    /**
     * Draws the main menu
     */
    private void drawNormalMenu() {
        //Play Game button
        GUI.SetNextControlName("0");
        if (GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2, ScreenUtil.ScreenHeight / 2 - ScreenUtil.getPixelHeight(50),
            ScreenUtil.getPixelWidth(400), style.fontSize), new GUIContent("Play Game", "0"), style)) {
            Application.LoadLevel(1);
        }
        if (GUI.tooltip == "0") {
            _highlightStyle.normal = style.hover;
        }
        if (GameManager.Instance.enableSeebright) {
            GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2 + ScreenUtil.ScreenWidth, ScreenUtil.ScreenHeight / 2 - ScreenUtil.getPixelHeight(50),
                ScreenUtil.getPixelWidth(400), style.fontSize), "Play Game", _highlightStyle);
        }
        _highlightStyle.normal = style.normal;

        //Tutorial Button
        GUI.SetNextControlName("1");
        if (GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2,
            ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(50), ScreenUtil.getPixelWidth(400), style.fontSize), new GUIContent("Tutorial", "1"), style)) {
            Application.LoadLevel(2);
        }
        if (GUI.tooltip == "1") {
            _highlightStyle.normal = style.hover;
        }
        if (GameManager.Instance.enableSeebright) {
            GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2 + ScreenUtil.ScreenWidth,
                ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(50), ScreenUtil.getPixelWidth(400), style.fontSize), "Tutorial", _highlightStyle);
        }
        _highlightStyle.normal = style.normal;

        //Credits Button
        GUI.SetNextControlName("2");
        if (GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2,
            ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(250), ScreenUtil.getPixelWidth(400), style.fontSize), new GUIContent("Credits", "2"), style)) {
            _displayCredits = true;
        }
        if (GUI.tooltip == "2") {
            _highlightStyle.normal = style.hover;
        }
        if (GameManager.Instance.enableSeebright) {
            GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2 + ScreenUtil.ScreenWidth,
                ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(250), ScreenUtil.getPixelWidth(400), style.fontSize), "Credits", _highlightStyle);
        }
        _highlightStyle.normal = style.normal;

        //Options Button
        GUI.SetNextControlName("3");
        if (GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2,
            ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(150), ScreenUtil.getPixelWidth(400), style.fontSize), new GUIContent("Options", "3"), style)) {
            _displayOptions = true;
        }
        if (GUI.tooltip == "3") {
            _highlightStyle.normal = style.hover;
        }
        if (GameManager.Instance.enableSeebright) {
            GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2 + ScreenUtil.ScreenWidth,
                ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(150), ScreenUtil.getPixelWidth(400), style.fontSize), "Options", _highlightStyle);
        }
        _highlightStyle.normal = style.normal;

#if !UNITY_WEBPLAYER
        //HighScore Display Button
        GUI.SetNextControlName("4");
        if (GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2,
            ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(350), ScreenUtil.getPixelWidth(400), style.fontSize), new GUIContent("High Scores", "4"), style))
        {
            _displayHighScore = true;
        }
        if (GUI.tooltip == "4")
        {
            _highlightStyle.normal = style.hover;
        }
        if (GameManager.Instance.enableSeebright)
        {
            GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2 + ScreenUtil.ScreenWidth,
                ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(350), ScreenUtil.getPixelWidth(400), style.fontSize), "High Scores", _highlightStyle);
        }
        _highlightStyle.normal = style.normal;
#endif

        //Joystick Menu Navigation
        if (GameManager.Instance.enableSeebright && SBRemote.remoteStatus) {
            _focusID = ManageFocus(_focusID, 4);
            if (SBRemote.GetButtonDown(SBRemote.BUTTON_SELECT)) {
                if (_focusID < 0) {
                    return;
                } else if (_focusID == 0) {
                    Application.LoadLevel(1);
                } else if (_focusID == 1) {
                    Application.LoadLevel(2);
                } else if (_focusID == 2) {
                    _displayCredits = true;
                    _focusID = -1;
                } else if (_focusID == 3) {
                    _displayOptions = true;
                    _focusID = -1;
                } else if (_focusID == 4) {
                    _displayHighScore = true;
                    _focusID = -1;
                }
            }
        }
    }
}

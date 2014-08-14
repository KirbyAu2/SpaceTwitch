using UnityEngine;
using System.Collections;

/*
 * The EscapeMenu class will draw the escape menu when the game is paused
 * The escape menu allows the player to change options, go back to main menu, or resume the game
 */
public class EscapeMenu : MonoBehaviour {
    public GUIStyle style;

    public bool currentlyActive { get; private set; }

    private float currentTime;
    private bool _displayOptions = false;
    private float _focusTimerMax = .2f;
    private float _focusTimer = 0;
    private int _focusID = -1;

    void Start () {
        
        style.fontSize = (int)(ScreenUtil.getPixelHeight(style.fontSize));
        currentTime = Time.timeScale;
    }

    //Escape menu display
    public void display() {
        CameraController.currentCamera.setBlurShader(true);
        _displayOptions = false;
        currentTime = Time.timeScale;
        Time.timeScale = 0;
#if !UNITY_IPHONE
        Screen.lockCursor = false;
#endif
        currentlyActive = true;
    }

    //Exit Escape menu back to game 
    public void exit() {
        CameraController.currentCamera.setBlurShader(false);
        Time.timeScale = currentTime;
        currentlyActive = false;
#if !UNITY_IPHONE
        Screen.lockCursor = true;
#endif
    }

	
    void Update () {

    }

    int ManageFocus(int ID, int length)
    {
        if (!GameManager.Instance.enableSeebright) {
            return 0;
        }
        GUI.FocusControl(ID.ToString());
        if (_focusTimer < _focusTimerMax)
            _focusTimer += .01f;
        if (SBRemote.GetJoystickDelta(SBRemote.JOY_VERTICAL) < -2048 * 4 * 2 && ID < length && _focusTimer > _focusTimerMax)
        {
            _focusTimer = 0;
            ID++;
        }
        if (SBRemote.GetJoystickDelta(SBRemote.JOY_VERTICAL) > 2048 * 4 * 2 && ID > 0 && _focusTimer > _focusTimerMax)
        {
            _focusTimer = 0;
            ID--;
        }
        return ID;
    }

    /*
     * OnGUI is called for rendering and handling GUI events. 
     * Buttons for 'Resume Game', 'Main Menu', and 'Options'
     * As well as buttons for Options menu
     */
    void OnGUI() {
        GUIStyle _highlightStyle = new GUIStyle(style);

        if (!currentlyActive) {
            return;
        }
        if (!_displayOptions) {
            // Resume Game Button
            GUI.SetNextControlName("0");
            if (GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2, ScreenUtil.ScreenHeight / 2,
                                    ScreenUtil.getPixelWidth(400), style.fontSize), new GUIContent("Resume Game", "0"), style)) {
                exit();
            }
            if (GUI.tooltip == "0")
            {
                _highlightStyle.normal = style.hover;
            }
            GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2 + ScreenUtil.ScreenWidth, ScreenUtil.ScreenHeight / 2,
                                    ScreenUtil.getPixelWidth(400), style.fontSize), "Resume Game", _highlightStyle);
            _highlightStyle.normal = style.normal;

            //Main Menu Button
            GUI.SetNextControlName("1");
            if (GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2, 
                                    ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(100), ScreenUtil.getPixelWidth(400), 
                                    style.fontSize), new GUIContent("Main Menu", "1"), style)) {
                Application.LoadLevel(0);
            }
            if (GUI.tooltip == "1")
            {
                _highlightStyle.normal = style.hover;
            }
            if (GameManager.Instance.enableSeebright)
            {
                GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2 + ScreenUtil.ScreenWidth,
                                    ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(100), ScreenUtil.getPixelWidth(400),
                                    style.fontSize), "Main Menu", _highlightStyle);
            }
            _highlightStyle.normal = style.normal;

            //Options Button
            GUI.SetNextControlName("2");
            if (GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2, 
                                    ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(200), ScreenUtil.getPixelWidth(400), 
                                    style.fontSize), new GUIContent("Options","2"), style)) {
                _displayOptions = true;
            }
            if (GUI.tooltip == "2")
            {
                _highlightStyle.normal = style.hover;
            }
            if (GameManager.Instance.enableSeebright)
            {
                GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2 + ScreenUtil.ScreenWidth,
                                    ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(200), ScreenUtil.getPixelWidth(400),
                                    style.fontSize), "Options", _highlightStyle);
            }
            _highlightStyle.normal = style.normal;

            //Joystick Menu Navigation
            if (GameManager.Instance.enableSeebright) {
                _focusID = ManageFocus(_focusID, 2);
                if (SBRemote.GetButtonDown(SBRemote.BUTTON_SELECT)) {
                    if (_focusID < 0) {
                        return;
                    } else if (_focusID == 0) {
                        exit();
                    } else if (_focusID == 1) {
                        Application.LoadLevel(0);
                    } else if (_focusID == 2) {
                        _displayOptions = true;
                        _focusID = -1;
                    }
                }
            }
        }
        //In option Menu
        else if (_displayOptions) {
            Color prev = GUI.color;
            GUI.color = Color.magenta;

            if (GameManager.Instance.enableSeebright == false)
            {
                GUI.Label(new Rect(ScreenUtil.ScreenWidth / 2 - 3 * ScreenUtil.ScreenWidth / 16, ScreenUtil.ScreenHeight / 2 -
                    ScreenUtil.getPixelHeight(100), 3 * ScreenUtil.ScreenWidth / 8, ScreenUtil.getPixelHeight(200)), "Options", style);
                GUI.color = prev;
                GUI.Label(new Rect((ScreenUtil.ScreenWidth + ScreenUtil.getPixelWidth(200)) / 2, ScreenUtil.ScreenHeight / 2,
                    ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(200)), "Effects Volume", style);
                GameManager.effectsVolume = GUI.HorizontalSlider(new Rect((ScreenUtil.ScreenWidth + ScreenUtil.getPixelWidth(200)) / 2,
                    ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(100), ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(50)),
                    GameManager.effectsVolume, 0f, 1.0f);
                GUI.Label(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(1000)) / 2, ScreenUtil.ScreenHeight / 2,
                    ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(200)), "Music Volume", style);
                GameManager.musicVolume = GUI.HorizontalSlider(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(1000)) / 2,
                    ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(100), ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(50)),
                    GameManager.musicVolume, 0f, 1.0f);
#if !UNITY_IPHONE
                GUI.Label(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2,
                    ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(150), ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(200)),
                    "Sensitivity", style);
                GameManager.mouseSensitivity = GUI.HorizontalSlider(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2,
                    ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(230), ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(50)),
                    GameManager.mouseSensitivity, 0.1f, 0.5f);
#endif
            }
            else if (GameManager.Instance.enableSeebright == true)
            {
                GUI.Label(new Rect(ScreenUtil.ScreenWidth / 2 - 3 * ScreenUtil.ScreenWidth / 16, ScreenUtil.ScreenHeight / 2 -
                    ScreenUtil.getPixelHeight(200), 3 * ScreenUtil.ScreenWidth / 8, ScreenUtil.getPixelHeight(200)), "Options", style);
                GUI.Label(new Rect(ScreenUtil.ScreenWidth / 2 - 3 * ScreenUtil.ScreenWidth / 16 + ScreenUtil.ScreenWidth, ScreenUtil.ScreenHeight / 2 -
                    ScreenUtil.getPixelHeight(200), 3 * ScreenUtil.ScreenWidth / 8, ScreenUtil.getPixelHeight(200)), "Options", style);
                GUI.color = prev;
                //SFX Volume
                GUI.SetNextControlName("0");
                GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2, ScreenUtil.ScreenHeight / 2 - ScreenUtil.getPixelHeight(100),
                    ScreenUtil.getPixelWidth(400), style.fontSize), new GUIContent("Effects Volume","0"), style);
                GameManager.effectsVolume = GUI.HorizontalSlider(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2,
                    ScreenUtil.ScreenHeight / 2 - ScreenUtil.getPixelHeight(30), ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(50)),
                    GameManager.effectsVolume, 0f, 1.0f);
                if (GUI.tooltip == "0")
                {
                    _highlightStyle.normal = style.hover;
                }
                GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2 + ScreenUtil.ScreenWidth, ScreenUtil.ScreenHeight / 2 - ScreenUtil.getPixelHeight(100),
                    ScreenUtil.getPixelWidth(400), style.fontSize), "Effects Volume", _highlightStyle);
                _highlightStyle.normal = style.normal;
                GameManager.effectsVolume = GUI.HorizontalSlider(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2 + ScreenUtil.ScreenWidth,
                    ScreenUtil.ScreenHeight / 2 - ScreenUtil.getPixelHeight(30), ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(50)),
                    GameManager.effectsVolume, 0f, 1.0f);
                //Music Volume
                GUI.SetNextControlName("1");
                GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2, ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(20),
                    ScreenUtil.getPixelWidth(400), style.fontSize), new GUIContent("Music Volume","1"), style);
                GameManager.musicVolume = GUI.HorizontalSlider(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2,
                    ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(90), ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(50)),
                    GameManager.musicVolume, 0f, 1.0f);
                if (GUI.tooltip == "1")
                {
                    _highlightStyle.normal = style.hover;
                }                
                GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2 + ScreenUtil.ScreenWidth, ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(20),
                    ScreenUtil.getPixelWidth(400), style.fontSize), "Music Volume", _highlightStyle);
                _highlightStyle.normal = style.normal;
                GameManager.musicVolume = GUI.HorizontalSlider(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2 + ScreenUtil.ScreenWidth,
                    ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(90), ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(50)),
                    GameManager.musicVolume, 0f, 1.0f);
                //Sensitivity Control
                GUI.SetNextControlName("2");
                GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2,
                    ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(140), ScreenUtil.getPixelWidth(400), style.fontSize),
                    new GUIContent("Sensitivity","2"), style);
                GameManager.mouseSensitivity = GUI.HorizontalSlider(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2,
                    ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(210), ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(50)),
                    GameManager.mouseSensitivity, 0.1f, 0.5f);
                if (GUI.tooltip == "2")
                {
                    _highlightStyle.normal = style.hover;
                }
                GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2 + ScreenUtil.ScreenWidth,
                    ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(140), ScreenUtil.getPixelWidth(400), style.fontSize),
                    "Sensitivity", _highlightStyle);
                _highlightStyle.normal = style.normal;
                GameManager.mouseSensitivity = GUI.HorizontalSlider(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2 + ScreenUtil.ScreenWidth,
                    ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(210), ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(50)),
                    GameManager.mouseSensitivity, 0.1f, 0.5f);
            }
            if (GameManager.Instance) {
                GameManager.Instance.updateMusicVolume();
            }


#if UNITY_IPHONE
            if (!GameManager.Instance.enableSeebright) {
                GUI.SetNextControlName("3");
                if (GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(200)) / 2,
                    ScreenUtil.ScreenHeight - ScreenUtil.getPixelHeight(300), ScreenUtil.getPixelWidth(200),
                    style.fontSize), new GUIContent((GameManager.invertedJoystick) ? "Inverted Joystick: On" : "Inverted Joystick: Off", "3"), style)) {
                    GameManager.invertedJoystick = !GameManager.invertedJoystick;
                }
                if (GUI.tooltip == "3") {
                    _highlightStyle.normal = style.hover;
                }
                if (GameManager.Instance.enableSeebright) {
                    GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(200)) / 2 + ScreenUtil.ScreenWidth,
                        ScreenUtil.ScreenHeight - ScreenUtil.getPixelHeight(300), ScreenUtil.getPixelWidth(200), style.fontSize), "Back", _highlightStyle);
                }
                _highlightStyle.normal = style.normal;
                _focusID = ManageFocus(_focusID, 3);


                GUI.SetNextControlName("4");
                if (GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(200)) / 2,
                    ScreenUtil.ScreenHeight - ScreenUtil.getPixelHeight(200), ScreenUtil.getPixelWidth(200),
                    style.fontSize), new GUIContent((GameManager.invertedControls) ? "Inverted Controls: On" : "Inverted Controls: Off", "3"), style)) {
                    GameManager.invertedControls = !GameManager.invertedControls;
                    GameManager.Instance.joystickLeft.SetActive(GameManager.invertedControls);
                    GameManager.Instance.joystickRight.SetActive(!GameManager.invertedControls);
                }
                if (GUI.tooltip == "4") {
                    _highlightStyle.normal = style.hover;
                }
                if (GameManager.Instance.enableSeebright) {
                    GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(200)) / 2 + ScreenUtil.ScreenWidth,
                        ScreenUtil.ScreenHeight - ScreenUtil.getPixelHeight(200), ScreenUtil.getPixelWidth(200), style.fontSize), "Back", _highlightStyle);
                }
                _highlightStyle.normal = style.normal;
                _focusID = ManageFocus(_focusID, 4);
            }
#endif


            GUI.SetNextControlName("5");
            if (GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(200)) / 2,
                ScreenUtil.ScreenHeight - ScreenUtil.getPixelHeight(100), ScreenUtil.getPixelWidth(200), style.fontSize), new GUIContent("Back", "3"), style)) {
                _displayOptions = false;
                // update sensitivity when options are closed
                if (GameManager.Instance != null) {
                    foreach (Player player in GameManager.Instance.CurrentPlayerShips) {
                        player.UpdateSensitivity();
                    }
                }
            }
            if (GUI.tooltip == "5") {
                _highlightStyle.normal = style.hover;
            }
            GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(200)) / 2 + ScreenUtil.ScreenWidth,
                ScreenUtil.ScreenHeight - ScreenUtil.getPixelHeight(100), ScreenUtil.getPixelWidth(200), style.fontSize), "Back", _highlightStyle);
            _highlightStyle.normal = style.normal;
            _focusID = ManageFocus(_focusID, 5);

            if (GameManager.Instance.enableSeebright) {
                if (SBRemote.GetButtonDown(SBRemote.BUTTON_SELECT)) {
                    if (_focusID < 0) {
                        return;
                    } else if (_focusID == 3) {
                        _displayOptions = false;
                        _focusID = -1;
                    }
                }
                if (SBRemote.GetJoystickDelta(SBRemote.JOY_HORIZONTAL) > 2048 * 4 * 2) {
                    if (_focusID == 0 && GameManager.effectsVolume < 1) {
                        GameManager.effectsVolume += .01f;
                    } else if (_focusID == 1 && GameManager.musicVolume < 1) {
                        GameManager.musicVolume += .01f;
                    } else if (_focusID == 2 && GameManager.mouseSensitivity < .5f) {
                        GameManager.mouseSensitivity += .005f;
                    }
                }
                if (SBRemote.GetJoystickDelta(SBRemote.JOY_HORIZONTAL) < -2048 * 4 * 2) {
                    if (_focusID == 0 && GameManager.effectsVolume > 0) {
                        GameManager.effectsVolume -= .01f;
                    } else if (_focusID == 1 && GameManager.musicVolume > 0) {
                        GameManager.musicVolume -= .01f;
                    } else if (_focusID == 2 && GameManager.mouseSensitivity > .1f) {
                        GameManager.mouseSensitivity -= .005f;
                    }
                }
            }

        }
        else
        {
            _focusID = -1;
        }
    }
}

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
    private JoystickButtonMenu mainMenu, optionMenu;
    private int numberOfButtons = 3;
    private Rect[] rectangleArray = { new Rect(15, 15, 15, 15), new Rect(30, 15, 15, 15), new Rect(45, 15, 15, 15) };
    private string[] stringArray = { "Resume", "Options", "Main Menu" };
    private string inputJoyButton = SBRemote.BUTTON_SELECT;
    private JoystickButtonMenu.JoyAxis joystickInputAxis = JoystickButtonMenu.JoyAxis.Vertical;

    void Start () {
        mainMenu = new JoystickButtonMenu(numberOfButtons, rectangleArray, stringArray, inputJoyButton,joystickInputAxis);
        style.fontSize = (int)(ScreenUtil.getPixelHeight(style.fontSize));
        currentTime = Time.timeScale;
    }

    //Escape menu display
    public void display() {
        CameraController.currentCamera.setBlurShader(true);
        _displayOptions = false;
        currentTime = Time.timeScale;
        Time.timeScale = 0;
        Screen.lockCursor = false;
        currentlyActive = true;
    }

    //Exit Escape menu back to game 
    public void exit() {
        CameraController.currentCamera.setBlurShader(false);
        Time.timeScale = currentTime;
        currentlyActive = false;
        Screen.lockCursor = true;
    }

	
    void Update () {
#if UNITY_EDITOR
#else
        if (GameManager.Instance.enableSeebright)
        {
            mainMenu.CheckJoystickAxis();
            mainMenu.CheckJoystickButton();
           
        }
#endif
    }

    /*
     * OnGUI is called for rendering and handling GUI events. 
     * Buttons for 'Resume Game', 'Main Menu', and 'Options'
     * As well as buttons for Options menu
     */
    void OnGUI() {
#if UNITY_EDITOR
#else
        if (GameManager.Instance.enableSeebright)
        {
            mainMenu.DisplayButtons();
        }
#endif
        if (!currentlyActive) {
            return;
        }
        if (!_displayOptions) {
            if (GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2, ScreenUtil.ScreenHeight / 2,
                                    ScreenUtil.getPixelWidth(400), style.fontSize), "Resume Game", style)) {
                exit();
            }
            if (GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2, 
                                    ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(100), ScreenUtil.getPixelWidth(400), 
                                    style.fontSize), "Main Menu", style)) {
                Application.LoadLevel(0);
            }
            if (GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2, 
                                    ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(200), ScreenUtil.getPixelWidth(400), 
                                    style.fontSize), "Options", style)) {
                _displayOptions = true;
            }
        }
        //In option Menu
        if (_displayOptions) {
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
                GUI.Label(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2,
                    ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(150), ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(200)),
                    "Sensitivity", style);
                GameManager.mouseSensitivity = GUI.HorizontalSlider(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2,
                    ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(230), ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(50)),
                    GameManager.mouseSensitivity, 0.1f, 0.5f);
            }
            else if (GameManager.Instance.enableSeebright == true)
            {
                GUI.Label(new Rect(ScreenUtil.ScreenWidth / 2 - 3 * ScreenUtil.ScreenWidth / 16, ScreenUtil.ScreenHeight / 2 -
                    ScreenUtil.getPixelHeight(200), 3 * ScreenUtil.ScreenWidth / 8, ScreenUtil.getPixelHeight(200)), "Options", style);
                GUI.color = prev;
                //SFX Volume
                GUI.Label(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2, ScreenUtil.ScreenHeight / 2 - ScreenUtil.getPixelHeight(100),
                    ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(200)), "Effects Volume", style);
                GameManager.effectsVolume = GUI.HorizontalSlider(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2,
                    ScreenUtil.ScreenHeight / 2 - ScreenUtil.getPixelHeight(30), ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(50)),
                    GameManager.effectsVolume, 0f, 1.0f);
                //Music Volume
                GUI.Label(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2, ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(20),
                    ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(200)), "Music Volume", style);
                GameManager.musicVolume = GUI.HorizontalSlider(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2,
                    ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(90), ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(50)),
                    GameManager.musicVolume, 0f, 1.0f);
                //Sensitivity Control
                GUI.Label(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2,
                    ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(140), ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(200)),
                    "Sensitivity", style);
                GameManager.mouseSensitivity = GUI.HorizontalSlider(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(400)) / 2,
                    ScreenUtil.ScreenHeight / 2 + ScreenUtil.getPixelHeight(210), ScreenUtil.getPixelWidth(400), ScreenUtil.getPixelHeight(50)),
                    GameManager.mouseSensitivity, 0.1f, 0.5f);
            }

            if(GameManager.Instance != null) {
                GameManager.Instance.UpdateMusicVolume();
            }

            if (GUI.Button(new Rect((ScreenUtil.ScreenWidth - ScreenUtil.getPixelWidth(200)) / 2, 
                ScreenUtil.ScreenHeight - ScreenUtil.getPixelHeight(150), ScreenUtil.getPixelWidth(200), style.fontSize), "Back", style)) {
                _displayOptions = false;
                // update sensitivity when options are closed
                foreach (Player player in GameManager.Instance.CurrentPlayerShips)
                {
                    player.UpdateSensitivity();
                }
            }
        }
    }
}

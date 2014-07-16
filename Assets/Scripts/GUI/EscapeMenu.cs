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
    private BlurEffect _blur;

    void Start () {
        style.fontSize = (int)(ScreenUtil.getPixels(style.fontSize));
        currentTime = Time.timeScale;
    }

    //Escape menu display
    public void display() {
        if (_blur == null) {
            _blur = CameraController.currentCamera.gameObject.GetComponent<BlurEffect>();
        }
        _displayOptions = false;
        _blur.enabled = true;
        currentTime = Time.timeScale;
        Time.timeScale = 0;
        Screen.lockCursor = false;
        currentlyActive = true;
    }

    //Exit Escape menu back to game 
    public void exit() {
        _blur.enabled = false;
        Time.timeScale = currentTime;
        currentlyActive = false;
        Screen.lockCursor = true;
    }

	
    void Update () {

    }

    /*
     * OnGUI is called for rendering and handling GUI events. 
     * Buttons for 'Resume Game', 'Main Menu', and 'Options'
     * As well as buttons for Options menu
     */
    void OnGUI() {
        if (!currentlyActive) {
            return;
        }
        if (!_displayOptions) {
            if (GUI.Button(new Rect((Screen.width - ScreenUtil.getPixels(400)) / 2, Screen.height / 2, ScreenUtil.getPixels(400), style.fontSize), "Resume Game", style)) {
                exit();
            }
            if (GUI.Button(new Rect((Screen.width - ScreenUtil.getPixels(400)) / 2, Screen.height / 2 + ScreenUtil.getPixels(100), ScreenUtil.getPixels(400), style.fontSize), "Main Menu", style)) {
                Application.LoadLevel(0);
            }
            if (GUI.Button(new Rect((Screen.width - ScreenUtil.getPixels(400)) / 2, Screen.height / 2 + ScreenUtil.getPixels(200), ScreenUtil.getPixels(400), style.fontSize), "Options", style)) {
                _displayOptions = true;
            }
        }
        //In option Menu
        if (_displayOptions) {
            Color prev = GUI.color;
            GUI.color = Color.magenta;
            GUI.Label(new Rect(Screen.width / 2 - 3 * Screen.width / 16, Screen.height / 2 - ScreenUtil.getPixels(100), 3 * Screen.width / 8, ScreenUtil.getPixels(200)), "Options", style);
            GUI.color = prev;

            //Volume Slider
            GUI.Label(new Rect((Screen.width - ScreenUtil.getPixels(400)) / 2, Screen.height / 2, ScreenUtil.getPixels(400), ScreenUtil.getPixels(200)), "Volume", style);
            AudioListener.volume = GUI.HorizontalSlider(new Rect((Screen.width - ScreenUtil.getPixels(400)) / 2, Screen.height / 2 + ScreenUtil.getPixels(100), ScreenUtil.getPixels(400),
                ScreenUtil.getPixels(50)), AudioListener.volume, 0f, 1.0f);
            //Sensitivity Slider
            GUI.Label(new Rect((Screen.width - ScreenUtil.getPixels(400)) / 2, Screen.height / 2 + ScreenUtil.getPixels(150), ScreenUtil.getPixels(400), ScreenUtil.getPixels(200)), "Sensitivity", style);
            GameManager.mouseSensitivity = GUI.HorizontalSlider(new Rect((Screen.width - ScreenUtil.getPixels(400)) / 2, Screen.height / 2 + ScreenUtil.getPixels(230), ScreenUtil.getPixels(400), ScreenUtil.getPixels(50)),
                GameManager.mouseSensitivity, 0.1f, 0.5f);
            //Back to Escape Menu button
            if (GUI.Button(new Rect((Screen.width - ScreenUtil.getPixels(200)) / 2, Screen.height - ScreenUtil.getPixels(150), ScreenUtil.getPixels(200), style.fontSize), "Back", style)) {
                _displayOptions = false;
            }
        }
    }
}

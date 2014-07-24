using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{


    public GUISkin mySkin;
    public float delayBetweenFocusChanges = .5f;

    private Rect[] myRects = new Rect[3];
    private string[] mainMenuLabels = new string[3];
    private string[] optionMenuLabels = new string[3];
    private JoystickButtonMenu mainMenu, optionMenu;

    private int currentlyPressedButton = -1;

    void Start()
    {
        myRects[0] = new Rect(Screen.width / 2 - 30, Screen.height / 2 - 40, 60, 30);
        myRects[1] = new Rect(Screen.width / 2 - 30, Screen.height / 2, 60, 30);
        myRects[2] = new Rect(Screen.width / 2 - 30, Screen.height / 2 + 40, 60, 30);

        mainMenuLabels[0] = "Play";
        mainMenuLabels[1] = "Options";
        mainMenuLabels[2] = "Exit";

        optionMenuLabels[0] = "Go";
        optionMenuLabels[1] = "Fuck";
        optionMenuLabels[2] = "Yourself";

        mainMenu = new JoystickButtonMenu(3, myRects, mainMenuLabels, "Fire1", JoystickButtonMenu.JoyAxis.Vertical);
        optionMenu = new JoystickButtonMenu(3, myRects, optionMenuLabels, "Fire1", JoystickButtonMenu.JoyAxis.Vertical);

        optionMenu.enabled = false;
    }

    void OnGUI()
    {
        GUI.skin = mySkin;

        mainMenu.DisplayButtons();
        optionMenu.DisplayButtons();
    }

    void Update()
    {
        if (mainMenu.enabled)
        {
            if (mainMenu.CheckJoystickAxis())
            {
                Invoke("Delay", delayBetweenFocusChanges);
            }
            currentlyPressedButton = mainMenu.CheckJoystickButton();

            switch (currentlyPressedButton)
            {
                case 0:
                    Application.LoadLevel(Application.loadedLevel + 1);
                    return;
                case 1:
                    optionMenu.enabled = true;
                    mainMenu.enabled = false;
                    return;
                case 2:
                    Application.Quit();
                    return;
            }
        }
        if (optionMenu.enabled)
        {
            if (optionMenu.CheckJoystickAxis())
            {
                Invoke("Delay", delayBetweenFocusChanges);
            }
            currentlyPressedButton = optionMenu.CheckJoystickButton();

            switch (currentlyPressedButton)
            {
                case 0:
                    mainMenu.enabled = true;
                    optionMenu.enabled = false;
                    return;
                case 1:
                    mainMenu.enabled = true;
                    optionMenu.enabled = false;
                    return;
                case 2:
                    mainMenu.enabled = true;
                    optionMenu.enabled = false;
                    return;
            }
        }
    }

    private void Delay()
    {
        mainMenu.isCheckingJoy = false;
        optionMenu.isCheckingJoy = false;
    }
}

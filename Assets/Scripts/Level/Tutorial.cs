using UnityEngine;
using System.Collections.Generic;

/*
 * The Tutorial class manages the tutorial level for the game
 * Displays messages to help player
 */
public class Tutorial : MonoBehaviour
{
    public GUIStyle style;
    private const float GAME_END_DURATION = 4.0f;
    private const int MOVEMENT_TUT_DURATION = 1;
    private const string TUTORIAL_COMPLETE = "You have successfully \ncompleted the tutorial!";
    private const string MOVEMENT = "Swipe the mouse \nleft or right to move";
    private const string PAWN = "Pawns spawn at the back of lanes";
    private const string SWIRLIE_SPIKES = "Spikes can only be destroyed \nafter the Swirlie has been destroyed!";
    private const string CONFETTI = "Confettis go up and down rows";

    public List<string> messages;
    public bool readyToSpawn = false;

    private GUIItem _currentMessageItem;
    private int _currentMessage = 0;
    private float _startTime;
    private bool _tutorialOver = false;

    void Start () {
        //Adds GUI messages to list 
        if (!GameManager.Instance.enableSeebright)
        {
            _currentMessageItem = new GUIItem((float)(ScreenUtil.ScreenWidth / 2),
                (float)(ScreenUtil.ScreenHeight / 2), MOVEMENT, GUIManager.Instance.defaultStyle, MOVEMENT_TUT_DURATION);
        }
        else
        {
            _currentMessageItem = new GUIItem((float)(ScreenUtil.ScreenWidth / 2),
                (float)(ScreenUtil.ScreenHeight / 2), MOVEMENT, style, MOVEMENT_TUT_DURATION);
        }
        messages = new List<string>();
        messages.Add(MOVEMENT);
        messages.Add(PAWN);
        messages.Add(SWIRLIE_SPIKES);
        messages.Add(CONFETTI);
        GUIManager.Instance.addGUIItem(_currentMessageItem);
    }
	
    void Update () {
        //Returns back to Main menu when tutorial is over
        if (_tutorialOver) {
            if (Time.time > _startTime + GAME_END_DURATION) {
                Application.LoadLevel(0);
            }
            return;
        }

        if (GUIManager.Instance.ItemsCount < 1 && _currentMessage+1 < messages.Count) {
            if (_currentMessage == 0) {
                readyToSpawn = true;
            }
            _currentMessageItem = new GUIItem((float)(ScreenUtil.ScreenWidth / 2), ScreenUtil.getPixelHeight(280),
                messages[++_currentMessage], GUIManager.Instance.defaultStyle);
            GUIManager.Instance.addGUIItem(_currentMessageItem);
        }
    }

    //Displays next Gui message 
    public void displayNext() {
        GUIManager.Instance.removeGUIItem(_currentMessageItem);
    }

    /*
     * Prints end message and Ends tutorial 
     */
    public void endTutorial() {
        if (_tutorialOver) {
            return;
        }
        displayNext();
        _startTime = Time.time;
        _tutorialOver = true;
        GUIManager.Instance.addGUIItem(new GUIItem((float)(ScreenUtil.ScreenWidth / 2), (float)(ScreenUtil.ScreenHeight / 2), 
            TUTORIAL_COMPLETE, GUIManager.Instance.defaultStyle, (int)GAME_END_DURATION));
    }
}

using UnityEngine;
using System.Collections.Generic;

public class Tutorial : MonoBehaviour {
    private const float GAME_END_DURATION = 4.0f;
    private const int MOVEMENT_TUT_DURATION = 1;
    private const string TUTORIAL_COMPLETE = "You have successfully completed the tutorial!";
    private const string MOVEMENT = "Swipe the mouse left or right to move";
    private const string PAWN = "Pawns spawn at the back of lanes";
    private const string SWIRLIE_SPIKES = "Spikes can only be destroyed \nafter the Swirlie has been destroyed!";

    public List<string> messages;
    public bool readyToSpawn = false;

    private GUIItem _currentMessageItem;
    private int _currentMessage = 0;
    private float _startTime;
    private bool _tutorialOver = false;

    void Start () {
        _currentMessageItem = new GUIItem((float)(Screen.width / 2), (float)(Screen.height / 2), MOVEMENT, GUIManager.Instance.defaultStyle, MOVEMENT_TUT_DURATION);
        messages = new List<string>();
        messages.Add(MOVEMENT);
        messages.Add(PAWN);
        messages.Add(SWIRLIE_SPIKES);
        GUIManager.Instance.addGUIItem(_currentMessageItem);
    }
	
    void Update () {
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
            _currentMessageItem = new GUIItem((float)(Screen.width / 2), ScreenUtil.getPixels(280),
                messages[++_currentMessage], GUIManager.Instance.defaultStyle);
            GUIManager.Instance.addGUIItem(_currentMessageItem);
        }
    }

    public void displayNext() {
        GUIManager.Instance.removeGUIItem(_currentMessageItem);
    }

    public void endTutorial() {
        if (_tutorialOver) {
            return;
        }
        displayNext();
        _startTime = Time.time;
        _tutorialOver = true;
        GUIManager.Instance.addGUIItem(new GUIItem((float)(Screen.width / 2), (float)(Screen.height / 2), TUTORIAL_COMPLETE, GUIManager.Instance.defaultStyle, (int)GAME_END_DURATION));
    }
}

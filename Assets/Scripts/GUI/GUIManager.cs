using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * The GUI manager manages GUI throughout the game
 */
public class GUIManager : MonoBehaviour {
    private List<GUIItem> _items;
    private static GUIManager _instance;

    public GUIStyle defaultStyle;

    void Start() {
        //Implements singleton
        if (_instance != null) {
            Debug.LogError("Can't initialize more than one instance of GUI Manager!");
        }
        defaultStyle.fontSize = (int)ScreenUtil.getPixelHeight(defaultStyle.fontSize);
        _instance = this;
        _items = new List<GUIItem>();
    }

    public static GUIManager Instance {
        get
        {
            return _instance;
        }
    }

    public int ItemsCount {
        get {
            return _items.Count;
        }
    }
	
    void Update() {
        for (int i = _items.Count - 1; i >= 0; i--) {
            _items[i].update();
        }
    }

    //add GUI Item
    //initialize GUIItem start time
    public void addGUIItem(GUIItem g) {
        _items.Add(g);
        g.starttime = Time.time;
    }

    //removes GUI Item
    public void removeGUIItem(GUIItem g) {
        _items.Remove(g);
    }
    
    //clears GUI Item
    public void clearGUIItem() {
        _items.Clear();
    }

    /*
     * OnGUI is called for rendering and handling GUI events
     * Checks if currently in gameplay
     * Initializes text font, style, color, and positioning 
     */
    void OnGUI() {
        if(GameManager.Instance == null) {
            return;
        }
        if(GameManager.Instance.CurrentPlayerShips.Count > 0) {
            if (GameManager.Instance.CurrentPlayerShips[0].GetComponent<EscapeMenu>().currentlyActive) {
                return;
            }
        }
        foreach (GUIItem i in _items){
            Color originalColor = i.customGuiStyle.normal.textColor;
            Color temp = i.customGuiStyle.normal.textColor;
            temp.a = i.currentAlpha;
            i.customGuiStyle.normal.textColor = temp;
            GUI.Label(new Rect(i.xpos, i.ypos, 0, 0), i.message, i.customGuiStyle);
            i.customGuiStyle.normal.textColor = originalColor;
        }
    }

}


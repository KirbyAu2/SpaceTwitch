using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIManager : MonoBehaviour {
    private List<GUIItem> _items;
    private static GUIManager _instance;

    public GUIStyle defaultStyle;

    void Start() {
        if (_instance != null) {
            Debug.LogError("Can't initialize more than one instance of GUI Manager!");
        }
        defaultStyle.fontSize = (int)ScreenUtil.getPixels(defaultStyle.fontSize);
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

    public void addGUIItem(GUIItem g) {
        _items.Add(g);
        g.starttime = Time.time;
    }

    public void removeGUIItem(GUIItem g) {
        _items.Remove(g);
    }

    public void clearGUIItem() {
        _items.Clear();
    }

    void OnGUI() {
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


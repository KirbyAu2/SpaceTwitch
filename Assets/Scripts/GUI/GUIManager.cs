using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIManager : MonoBehaviour {
    private List<GUIItem> _items;

    // Use this for initialization
    void Start () {
	    _items = new List<GUIItem>();
    }
	
    // Update is called once per frame
    void Update()
    {
        foreach (GUIItem i in _items)
        {
            if (Time.time >= i.starttime + i.dur)
            {
                removeGUIItem(i);
            }

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

    void OnGUI(){
        foreach (GUIItem i in _items){
            GUI.Label(new Rect(i.xpos, i.ypos, Screen.width, Screen.height), i.message, i.customGuiStyle);
        }

    }

}


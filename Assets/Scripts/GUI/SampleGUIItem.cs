using UnityEngine;
using System.Collections;

public class SampleGUIItem : MonoBehaviour {
    public GUIStyle style;
    private bool _displayed = false;
    // Use this for initialization
    void Start () {
    }
	
    // Update is called once per frame
    void Update () {
        if (GUIManager.Instance && !_displayed)
        {
            _displayed = true;
            GUIManager.Instance.addGUIItem(new GUIItem(100, 100, Time.time, 4, "Testing", style));
        }
    }
}

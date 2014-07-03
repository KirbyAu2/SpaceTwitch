using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
    public static CameraController currentCamera;
    private const float EASING_TIME = 20.0f;
    private Camera _mainCamera;

    void Start () {
        if(currentCamera != null) {
            Debug.LogError("Can't have more than one camera!");
            return;
        }
        currentCamera = this;
        _mainCamera = gameObject.camera;
    }
	
    void Update () {
        if (GameManager.Instance.CurrentPlayerShips.Count < 1) {
            _mainCamera.transform.LookAt(new Vector3(1, 0, 0));
        }
        Vector3 pos = gameObject.transform.position;
        gameObject.transform.position = pos;
        Vector3 midPoint = Vector3.zero;
        foreach(Player s in GameManager.Instance.CurrentPlayerShips) {
            if(s.CurrentLane != null) {
                midPoint += s.CurrentLane.Front;
            }
        }
        midPoint = midPoint / GameManager.Instance.CurrentPlayerShips.Count;
        midPoint += GameManager.Instance.CurrentLevel.gameObject.transform.position;
        _mainCamera.transform.LookAt(Vector3.Lerp(pos, midPoint, EASING_TIME * Time.deltaTime));
    }
}

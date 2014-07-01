using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
    private const float EASING_TIME = 5.0f;
    private Camera _mainCamera;

    void Start () {
        _mainCamera = gameObject.camera;
    }
	
    void Update () {
        if (GameManager.Instance.CurrentPlayerShips.Count < 1) {
            _mainCamera.transform.LookAt(new Vector3(1, 0, 0));
        }
        Vector3 pos = gameObject.transform.position;
        pos.z = (GameManager.Instance.CurrentLevel.gameObject.transform.position.z) / 2.0f;
        gameObject.transform.position = pos;
        Vector3 midPoint = Vector3.zero;
        foreach(Player s in GameManager.Instance.CurrentPlayerShips) {
            midPoint += s.CurrentLane.Front;
        }
        midPoint = midPoint / GameManager.Instance.CurrentPlayerShips.Count;
        midPoint += new Vector3(1, 0, 0);
        midPoint /= 2;
        _mainCamera.transform.LookAt(Vector3.Lerp(_mainCamera.transform.forward, midPoint, EASING_TIME * Time.fixedDeltaTime));
    }
}

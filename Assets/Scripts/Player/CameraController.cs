using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
    Camera mainCamera;

    void Start () {
        mainCamera = gameObject.camera;
    }
	
    void Update () {
        if (GameManager.Instance.CurrentPlayerShips.Count < 1) {
            mainCamera.transform.LookAt(new Vector3(1, 0, 0));
        }
        Vector3 pos = gameObject.transform.position;
        pos.z = (GameManager.Instance.CurrentLevel.gameObject.transform.position.z) / 2.0f;
        Debug.Log(pos);
        gameObject.transform.position = pos;
        Vector3 midPoint = Vector3.zero;
        foreach(Player s in GameManager.Instance.CurrentPlayerShips) {
            midPoint += s.gameObject.transform.position;
        }
        midPoint = midPoint / GameManager.Instance.CurrentPlayerShips.Count;
        midPoint += new Vector3(1, 0, 0);
        midPoint /= 2;
        mainCamera.transform.LookAt(Vector3.Lerp(mainCamera.transform.forward, midPoint, 5.0f * Time.deltaTime));
    }
}

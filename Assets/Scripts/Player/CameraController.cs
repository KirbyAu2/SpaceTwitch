using UnityEngine;
using System.Collections;

/*
 * The CameraController class controls the movement of the camera throughout the gameplay
 * The camera slightly moves along with the playership 
 */
public class CameraController : MonoBehaviour {
    public static CameraController currentCamera;
    private const float EASING_TIME = 20.0f;
    private Camera _mainCamera;

    void Start () {
        //Implements Singleton
        if(currentCamera != null) {
            Debug.LogError("Can't have more than one camera!");
            return;
        }
        currentCamera = this;
        _mainCamera = gameObject.camera;
    }
	
    void Update () {
        //if no player ship
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
        if (GameManager.Instance.CurrentLevel == null || GameManager.Instance.CurrentPlayerShips.Count < 1) {
            return;
        }
        //if two player ships currently active then midPoint is at the middle of the two ships 
        midPoint = midPoint / GameManager.Instance.CurrentPlayerShips.Count;
        midPoint += GameManager.Instance.CurrentLevel.gameObject.transform.position;
        //Rotates the transform so the forward vector points at the midpoint of the player ship and the center of the screen
        _mainCamera.transform.LookAt(Vector3.Lerp(pos, midPoint, EASING_TIME * Time.deltaTime));
    }
}

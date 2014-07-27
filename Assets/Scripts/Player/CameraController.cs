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
    private AlpacaSound.RetroPixel _retroPixelShader;
    private BlurEffect _blurShader;

    //Seebright cameras
    //Right hand side
    private Camera _SBRHS;
    private AlpacaSound.RetroPixel _retroPixelShaderRHS;
    private BlurEffect _blurShaderRHS;
    //Left hand side
    private Camera _SBLHS;
    private AlpacaSound.RetroPixel _retroPixelShaderLHS;
    private BlurEffect _blurShaderLHS;

    void Start () {
        //Implements Singleton
        if(currentCamera != null) {
            Debug.LogError("Can't have more than one camera!");
            return;
        }
        _retroPixelShader = GetComponent<AlpacaSound.RetroPixel>();
        _blurShader = GetComponent<BlurEffect>();
        currentCamera = this;
        _mainCamera = gameObject.camera;
    }

    /**
     * initializes everything needed for seebright
     * Called specifically from Game Manager class
     */
    public void initSeebright() {
        if(_SBLHS != null || _SBRHS != null) {
            Debug.LogError("Already enabled seebright for camera controller!");
            return;
        }
        SBCamera sbCamera = GetComponent<SBCamera>();
        if(sbCamera == null) {
            Debug.LogError("No Seebright Camera attached to main camera object!");
            return;
        }
        sbCamera.enabled = true;
        //return if it just got enabled for the first time
        if(sbCamera.sbLeftCamera == null || sbCamera.sbRightCamera == null) {
            return;
        }
        _SBLHS = sbCamera.sbLeftCamera.GetComponentInChildren<Camera>();
        _SBRHS = sbCamera.sbRightCamera.GetComponentInChildren<Camera>();

        //Get RetroPixel shaders
        _retroPixelShaderLHS = _SBLHS.gameObject.AddComponent<AlpacaSound.RetroPixel>();
        _retroPixelShaderRHS = _SBRHS.gameObject.AddComponent<AlpacaSound.RetroPixel>();
        _retroPixelShaderLHS.enabled = false;
        _retroPixelShaderRHS.enabled = false;

        //Blur shader
        _blurShaderLHS = _SBLHS.gameObject.AddComponent<BlurEffect>();
        _blurShaderRHS = _SBRHS.gameObject.AddComponent<BlurEffect>();
        _blurShaderLHS.blurShader = (Shader)Resources.Load("Shaders/BlurEffectConeTaps");
        _blurShaderRHS.blurShader = (Shader)Resources.Load("Shaders/BlurEffectConeTaps");
        _blurShaderLHS.enabled = false;
        _blurShaderRHS.enabled = false;

        //Fast Bloom shaders
        //_SBLHS.gameObject.AddComponent<FastBloom>();
    }

    /**
     * returns right hand side camera if Seebright is enabled
     */
    public Camera RightCamera {
        get {
            return _SBRHS;
        }
    }

    /**
     * returns right hand side camera if Seebright is enabled
     */
    public Camera LeftCamera {
        get {
            return _SBLHS;
        }
    }

    /**
     * Sets the current Retro Shader(s) to the given value
     */
    public void setRetroShader(bool value) {
        if(GameManager.Instance.enableSeebright) {
            _retroPixelShaderLHS.enabled = value;
            _retroPixelShaderRHS.enabled = value;
        } else {
            _retroPixelShader.enabled = value;
        }
    }

    /**
     * Sets the current Blur Shader(s) to the given value
     */
    public void setBlurShader(bool value) {
        if(GameManager.Instance.enableSeebright) {
            _blurShaderLHS.enabled = value;
            _blurShaderRHS.enabled = value;
        } else {
            _blurShader.enabled = value;
        }
    }
	
    void Update () {
        if(GameManager.Instance.enableSeebright && _SBLHS == null) {
            initSeebright();
        }
        //if no player ship
        if (GameManager.Instance.CurrentPlayerShips.Count < 1 && _mainCamera != null) {
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
        if (_mainCamera != null) {
            _mainCamera.transform.LookAt(Vector3.Lerp(pos, midPoint, EASING_TIME * Time.deltaTime));
        }
    }
}

using UnityEngine;
using System.Collections;

/**
 * Seebright Camera v1.0 - Seebright, Inc
 *
 *  Copyright 2014 Seebright. 
 *  All rights reserved.
 * 
 * @author      John Murray - <john@seebright.com>
 * @author      Scott Holman - <scott@seebright.com>
 * @version     1.0
 * @data		2014
 * 
 * The SBCamera class is the main camera class which creates side-by-side stereographic cameras that are configured for the Seebright Visor.
 * The camera can be configured for many different functions such as a Gyroscopically enabled head movement and head constraints. 
 * Inherits all property's from the original Camera except for the Field of View (FOV), Interpupilary Distance (IPD), and BackGround Color (backgroundColor).
 * These property's can be edited and changed however.
 */

public class SBCamera : MonoBehaviour {
	
	public bool showStereoCam = true;			/** Shows the StereoCam within the editor. */	
	public float IPD = 0.06f;					/** Specifies the Interpupilary distance. */
	public Color backgroundColor = Color.black;	/** The Background Color of each the cameras. */
	public float FOV = 38.0f;					/** Specifies the Field of View of each of the cameras. */

	// Original Camera
	[HideInInspector]
	public Camera mainCamera;		/*!<Declares the Original Main Camera in the Scene*/
	private bool isActive = true;
	
	/** @name Gyroscopic Head Controls */
	//!@{
	public bool enableGyroCam = false;		/** Activates the Gyroscopically enabled head movement. */
	private bool offsetPerformed = false;
	public bool contrainHeadMovement = false;	/** Allows the developer to constrain the cameras along different axis in degrees. */
	public Vector2 pitchConstraint = new Vector2(0.0f, 0.0f);	/** Creates a movement constraint for the pitch rotation of the GyroCam that the user can specify. */
	public Vector2 yawConstraint = new Vector2(0.0f, 0.0f);		/** Creates a movement constraint for the yaw rotation of the GyroCam that the user can specify. */
	public Vector2 rollConstraint = new Vector2(0.0f, 0.0f);	/** Creates a movement constraint for the roll rotation of the GyroCam that the user can specify. */
	//!@}
	
	private int metaioScale = 1000;	/*!<Declares a new integer that represents the unit scale for the Metaio SDK that he user can specify. Metaio measures everything in units, and 1000 Unity units is equal to 1 meter.*/
	private int aRUnitScale = 1;		/*!<Creates a new integer that represents the aRUnitScale and sets it to 1. Used to move the cameras according to Metaio's scale factor.*/


	private static Quaternion forwardLookRotation;
	public static Quaternion currentLookRotation;	/** The current raw data rotation from the current forward look quaternion. */
	public static Quaternion normalizedLookRotation = Quaternion.identity;	/** The normalized rotation from the users look rotation. */

	private const float tiltOfScreen = 19.77f;

	private Quaternion offsetCameraRotation;
	private Quaternion rotationFix = new Quaternion (0, 0, 1, 0);
	private Vector3 neckPos;
	private Vector3 eyePos;
	private Vector3 metaioLensOffset = new Vector3(-43.415f, -142.2f, -3.759f);

	private bool isGyroscopeSupported;
	private Quaternion gyroRotation = Quaternion.identity;

	/** The left stereographic camera GameObject. */
	public GameObject sbLeftCamera {
		get; 
		set;
	}
	/** The right stereographic camera GameObject. */
	public GameObject sbRightCamera {
		get; 
		set;
	}
	private GameObject sbOffsetCamera {
		get; 
		set;
	}
	private GameObject sbGyroOrientCamera {
		get; 
		set;
	}
	private GameObject sbGyroHeadCamera {
		get; 
		set;
	}

	void Start()
	{
		SetMainCamera();
		InitializeSBCamera();
	}

	/** 
	 * Grabs the Camera component from the attached GameObject. If one isn't found it creates one.
	 */
	
	private void SetMainCamera()
	{
		mainCamera = gameObject.GetComponent<Camera>();
		if(mainCamera == null)
		{
			mainCamera = gameObject.AddComponent<Camera>();
		}
	}


	private void InitializeSBCamera()
	{
		if(mainCamera != null)
		{
			SetCameraVariables ();
			if(sbOffsetCamera == null || sbGyroOrientCamera == null)
			{
				CreateCameraParents();
			}
			if(sbLeftCamera == null || sbRightCamera == null)
			{
				CreateCameras();
			}
			if(sbOffsetCamera.transform.parent != mainCamera.transform)
			{
				SlaveObject(mainCamera.gameObject, sbOffsetCamera, neckPos);
			}
			if(sbGyroOrientCamera.transform.parent != sbOffsetCamera.transform)
			{
				SlaveObject(sbOffsetCamera, sbGyroOrientCamera);
			}
			if(sbGyroHeadCamera.transform.parent != sbGyroOrientCamera.transform)
			{
				SlaveObject(sbGyroOrientCamera, sbGyroHeadCamera);
			}
			if(sbLeftCamera.transform.parent != sbGyroHeadCamera.transform)
			{
				SlaveObject(sbGyroHeadCamera, sbLeftCamera, eyePos);
			}
			if(sbRightCamera.transform.parent != sbGyroHeadCamera.transform)
			{
				SlaveObject(sbGyroHeadCamera, sbRightCamera, eyePos);
			}

			SetIPD();
			CreateStereoCamera();
			SetCameraFlags();
			SetBackgroundColorCameras();
			SetCullingMask();
			SetProjectionType();
			if(mainCamera.isOrthoGraphic)
			{
				SetSize();
			}
			else
			{
				SetFOV();
			}
			SetNearClipPlane();
			SetFarClipPlane();
			SetDepth();
			SetRenderingPath();
			SetTargetTexture();
			SetOcclusionCulling();
			SetHDR();

			if(SeebrightSDK.singleton.enableMetaio)
			{
				//sbLeftCamera.transform.localPosition += metaioLensOffset;		/*!<Orignal Fresnel unit offset*/
				//sbRightCamera.transform.localPosition += metaioLensOffset;		/*!<Orignal Fresnel unit offset*/
				sbLeftCamera.transform.localPosition += new Vector3(-53.856f, -131.107f, 24.973f);		/*!<Pre-Alpha Frensel unit offset*/
				sbRightCamera.transform.localPosition += new Vector3(-53.856f, -131.107f, 24.973f);		/*!<Pre-Alpha Frensel unit offset*/
			}
		}
	}

	private void SetCameraVariables()
	{
		offsetCameraRotation.eulerAngles = new Vector3(tiltOfScreen, 180, 180);
		isGyroscopeSupported = SystemInfo.supportsGyroscope;
		if(SeebrightSDK.singleton.enableMetaio)
		{
			aRUnitScale = metaioScale;
		}
		else
		{
			aRUnitScale = 1;
		}
		neckPos = new Vector3(0, -0.14f * aRUnitScale, -0.16f * aRUnitScale);
		eyePos = new Vector3(0, 0.14f * aRUnitScale, 0.16f * aRUnitScale);
	}

	private void CreateCameras()
	{
		if(sbLeftCamera == null)
		{
			sbLeftCamera = new GameObject ("SBLeftCamera");
		}
		if(sbRightCamera == null)
		{
			sbRightCamera = new GameObject ("SBRightCamera");
		}
	}

	private void CreateCameraParents()
	{
		if(sbOffsetCamera == null)
		{
			sbOffsetCamera = new GameObject ("SBOffsetCamera");
		}
		if(sbGyroOrientCamera == null)
		{
			sbGyroOrientCamera = new GameObject ("SBGyroOrientCamera");
		}
		if(sbGyroHeadCamera == null)
		{
			sbGyroHeadCamera = new GameObject ("SBGyroHeadCamera");
		}
	}

	private void SlaveObject(GameObject parentObject, GameObject childObject)
	{
		childObject.transform.position = parentObject.transform.position;
		childObject.transform.rotation = parentObject.transform.rotation;
		childObject.transform.parent = parentObject.transform;
	}

	private void SlaveObject(GameObject parentObject, GameObject childObject, Vector3 offsetPos)
	{
		childObject.transform.position = parentObject.transform.position;
		childObject.transform.rotation = parentObject.transform.rotation;
		childObject.transform.localPosition += offsetPos;
		childObject.transform.parent = parentObject.transform;
	}

	private void SetIPD()
	{
		sbLeftCamera.transform.localPosition += new Vector3 (-(IPD / 2) * aRUnitScale, 0f, 0f);
		sbRightCamera.transform.localPosition += new Vector3 ((IPD / 2) * aRUnitScale, 0f, 0f);
	}

	private void CreateStereoCamera()
	{
		sbLeftCamera.AddComponent<Camera> ();
		sbRightCamera.AddComponent<Camera> ();
		sbLeftCamera.camera.rect = new Rect (0f, 0f, .5f, 1f);
		sbRightCamera.camera.rect = new Rect (.5f, 0f, .5f, 1f);
		if(mainCamera == Camera.main && mainCamera.enabled)
		{
			SeebrightSDK.currentCamera = this;
		}
	}

	private void SetCameraFlags()
	{
		sbLeftCamera.camera.clearFlags = mainCamera.clearFlags;
		sbRightCamera.camera.clearFlags = mainCamera.clearFlags;
	}

	private void SetBackgroundColorCameras()
	{
		sbLeftCamera.camera.backgroundColor = backgroundColor;
		sbRightCamera.camera.backgroundColor = backgroundColor;
	}

	private void SetCullingMask()
	{
		sbLeftCamera.camera.cullingMask = mainCamera.cullingMask;
		sbRightCamera.camera.cullingMask = mainCamera.cullingMask;
	}

	private void SetProjectionType()
	{
		sbLeftCamera.camera.isOrthoGraphic = mainCamera.isOrthoGraphic;
		sbRightCamera.camera.isOrthoGraphic = mainCamera.isOrthoGraphic;
	}

	private void SetSize()
	{
		sbLeftCamera.camera.orthographicSize = mainCamera.orthographicSize;
		sbRightCamera.camera.orthographicSize = mainCamera.orthographicSize;
	}

	private void SetFOV()
	{
		sbLeftCamera.camera.fieldOfView = FOV;
		sbRightCamera.camera.fieldOfView = FOV;
	}

	private void SetNearClipPlane()
	{
		sbLeftCamera.camera.nearClipPlane = mainCamera.nearClipPlane;
		sbRightCamera.camera.nearClipPlane = mainCamera.nearClipPlane;
	}

	private void SetFarClipPlane()
	{
		sbLeftCamera.camera.farClipPlane = mainCamera.farClipPlane;
		sbRightCamera.camera.farClipPlane = mainCamera.farClipPlane;
	}

	private void SetDepth()
	{
		sbLeftCamera.camera.depth = mainCamera.depth;
		sbRightCamera.camera.depth = mainCamera.depth;
	}

	private void SetRenderingPath()
	{
		sbLeftCamera.camera.renderingPath = mainCamera.renderingPath;
		sbRightCamera.camera.renderingPath = mainCamera.renderingPath;
	}

	private void SetTargetTexture()
	{
		sbLeftCamera.camera.targetTexture = mainCamera.targetTexture;
		sbRightCamera.camera.targetTexture = mainCamera.targetTexture;
	}

	private void SetOcclusionCulling()
	{
		sbLeftCamera.camera.useOcclusionCulling = mainCamera.useOcclusionCulling;
		sbRightCamera.camera.useOcclusionCulling = mainCamera.useOcclusionCulling;
	}

	private void SetHDR()
	{
		sbLeftCamera.camera.hdr = mainCamera.hdr;
		sbRightCamera.camera.hdr = mainCamera.hdr;
	}

	void Update() {

		if(mainCamera.enabled)
		{
			if(!isActive)
			{
				sbLeftCamera.camera.enabled = true;
				sbRightCamera.camera.enabled = true;
				SeebrightSDK.currentCamera = this;
				isActive = true;
			}
			//iPhone and Android Quaternion Adjustments for Gyro Data
			if (isGyroscopeSupported && enableGyroCam && !SeebrightSDK.singleton.enableMetaio) {
				#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
				gyroRotation = Input.gyro.attitude;
				currentLookRotation = gyroRotation * rotationFix * offsetCameraRotation;
				NormalizeHeadMovement();
				rotateOffset();
				if(contrainHeadMovement)
				{
					sbGyroOrientCamera.transform.localRotation = currentLookRotation;
					sbGyroHeadCamera.transform.localRotation = LockHeadMovement(sbGyroHeadCamera.transform.localRotation);
				}
				else
				{
					sbGyroOrientCamera.transform.localRotation = currentLookRotation;
				}
				#endif
			}
			#if UNITY_EDITOR
			if(enableGyroCam && !SeebrightSDK.singleton.enableMetaio)
			{

				float rotationX = Input.GetAxis("Horizontal");
				float rotationY = Input.GetAxis("Vertical");
				sbGyroOrientCamera.transform.localEulerAngles = new Vector3(rotationY, rotationX, 0);
			}
			#endif
		}
		if(!mainCamera.enabled)
		{
			if(isActive)
			{
				sbLeftCamera.camera.enabled = false;
				sbRightCamera.camera.enabled = false;
				isActive = false;
			}
		}
	}

	private void rotateOffset()
	{
		#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
		if(enableGyroCam && !SeebrightSDK.singleton.enableMetaio && !offsetPerformed)
		{
			sbOffsetCamera.transform.eulerAngles = new Vector3 (90, 270, 0);
			forwardLookRotation = Input.gyro.attitude * offsetCameraRotation * rotationFix;
			offsetPerformed = true;
		}
		else if(!enableGyroCam && !SeebrightSDK.singleton.enableMetaio && !offsetPerformed)
		{
			sbOffsetCamera.transform.eulerAngles = new Vector3 (90, 270, 0);
			forwardLookRotation = Input.gyro.attitude * offsetCameraRotation * rotationFix;
			offsetPerformed = false;
		}
		#endif
	}
	
	private float ClampAngle(float angle, float min, float max) {
		if (angle < -360)
			angle += 360;
		if (angle > 360)
			angle -= 360;
		return Mathf.Clamp(angle, min, max);
	}

	private void NormalizeHeadMovement ()
	{
		float pitch, yaw, roll;
		pitch = sbGyroHeadCamera.transform.eulerAngles.x - mainCamera.transform.localEulerAngles.x;
		yaw = sbGyroHeadCamera.transform.eulerAngles.y - mainCamera.transform.localEulerAngles.y;
		roll = sbGyroHeadCamera.transform.eulerAngles.z - mainCamera.transform.localEulerAngles.z;
		normalizedLookRotation.eulerAngles = new Vector3(pitch, yaw, roll);
	}

	private Quaternion LockHeadMovement(Quaternion finalRotation)
	{
		float x, y, z;

		x = finalRotation.eulerAngles.x;
		y = finalRotation.eulerAngles.y;
		z = finalRotation.eulerAngles.z;

		if(pitchConstraint.x != 0 && pitchConstraint.y != 0)
		{
			if((normalizedLookRotation.eulerAngles.x < (360 - pitchConstraint.x)) && (normalizedLookRotation.eulerAngles.x > 180))
			{
				x = (360 - pitchConstraint.x) - normalizedLookRotation.eulerAngles.x;
			}
			else if((normalizedLookRotation.eulerAngles.x > (pitchConstraint.y)) && (normalizedLookRotation.eulerAngles.x <= 180))
			{
				x = pitchConstraint.y - normalizedLookRotation.eulerAngles.x;
			}
			else
			{
				x = 0f;
			}
		}
		if(yawConstraint.x != 0 && yawConstraint.y != 0)
		{
			if((normalizedLookRotation.eulerAngles.y < (360 - yawConstraint.x)) && (normalizedLookRotation.eulerAngles.y > 180))
			{
				x = (360 - yawConstraint.x) - normalizedLookRotation.eulerAngles.y;
			}
			else if((normalizedLookRotation.eulerAngles.y > (yawConstraint.y)) && (normalizedLookRotation.eulerAngles.y <= 180))
			{
				x = yawConstraint.y - normalizedLookRotation.eulerAngles.y;
			}
			else
			{
				x = 0f;
			}
		}
		if(rollConstraint.x != 0 && rollConstraint.y != 0)
		{
			if((normalizedLookRotation.eulerAngles.z < (360 - rollConstraint.x)) && (normalizedLookRotation.eulerAngles.z > 180))
			{
				x = (360 - rollConstraint.x) - normalizedLookRotation.eulerAngles.z;
			}
			else if((normalizedLookRotation.eulerAngles.z > (rollConstraint.y)) && (normalizedLookRotation.eulerAngles.z <= 180))
			{
				x = rollConstraint.y - normalizedLookRotation.eulerAngles.z;
			}
			else
			{
				x = 0f;
			}
		}
		finalRotation.eulerAngles = new Vector3(x, y, z);
		return finalRotation;
	}
}

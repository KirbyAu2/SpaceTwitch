using UnityEngine;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System;

/**
 * Seebright SDK v1.0 - Seebright, Inc
 *
 *  Copyright 2014 Seebright. 
 *  All rights reserved.
 * 
 * @author      John Murray - <john@seebright.com>
 * @author      Scott Holman - <scott@seebright.com>
 * @version     1.0
 * @data		2014
 * 
 * The SeebrightSDK Class is the main class used to retreive and send information to and from all other Seebright Classes.
 * It is the central hub in which all relevant code for the Seebright classes goes through.
 * It initializes and grabs updates from the SBCursors (Seebright Cursors) class, as well as the SBRemote (Seebright Remote) class.
 * SeebrightSDK can also output Debug Code to the screen during runtime, be used to enable ARTracking either through Ball Tracking or MetaioSDK, and keeps track of the current SBCamera.
 * @see SBCursors()
 * @see SBRemote()
 * @see SBCamera()
 */

#pragma warning disable 0414 // variable assigned but not used.
public class SeebrightSDK : MonoBehaviour
{
	/** Declares SeebrightSDK class as the only one of it's type at any given time. */
	public static SeebrightSDK singleton;

	/** Enables the Debug GUI. */
	public bool enableDebugOutput = false;

	/** Variable that keeps track of the currently active SBCamera in the scene. */
	public static SBCamera currentCamera;
	
	/** @name AR Functions
	 * enableBallTracking is used to turn on functionality when tracking the Seebright Remote's ball. Used for 2D/3D Tracking.
	 * enableMetaio is to configure the SBCameras to metaio's unit scale.
	 * Cannot have both enable at the same time.
	 */
	//!@{
	public bool enableMetaio = false;
	//!@}


	/** When first initialized it will set this instance of the SeebrightSDK as the only instance */
	void Awake()
	{
		singleton=this;
	}

	/**
	 * Creates a periscope camera in the SBCursors class for more accurate Ball Tracking.
	 * Only works if enableBallTracking is true.
	 * @see SBCursors.StartCursors()
	 * Starts the Remote functionality by looking for the Seebright Remote by starting bluetooth in native iOS code.
	 * @see SBRemote.InitializeRemote()
	 */
	void Start() 
	{
		SBRemote.InitializeRemote();
	}

	/**
	 * For Android Only
	 * The Function OnApplicationPause checks to see of the current state of pausedState is true,
	 * and if the state is true, it accesses the SBRemote classes function and stops it,
	 * otherwise if the pausedStatus is false, then it starts the SBRemote class service.
	 */
	void OnApplicationPause(bool pauseStatus) 
	{
		if(pauseStatus)
		{
			SBRemote.StopService();
		}
		else
		{
			SBRemote.StartService();
		}
	}

	/**
	 * Calls remote updates and receieves input from the Seebright Blutooth remote.
	 * @see SBRemote.remoteLateUpdate()
	 */
	void LateUpdate() 
	{
		SBRemote.remoteLateUpdate();
	}


	private const float timeTillSleep = 3.0f;
	private float sleepTimer = 0.0f;
	private const string GUI_LABEL = "guiLabel";
	private GUIStyle cursorStyle;
	
	private void writeGUI (string type, int x, int y, int width, int height, string stringContent)
	{
		int newLeftX = (x < 0) ? Screen.width / 2 + x : x;
		int newRightX = (x < 0) ? Screen.width + x : Screen.width / 2 + x;
		int newY = (y < 0) ? Screen.height + y : y;
		switch (type) {
		case GUI_LABEL:
			GUI.Label (new UnityEngine.Rect (newLeftX, newY, width, height), stringContent, cursorStyle);
			GUI.Label (new UnityEngine.Rect (newRightX, newY, width, height), stringContent, cursorStyle);
			//Rotation not necessary for SeebrightSDK, because right-side-up text on Landscape Left shows up
			//upside-down through the mirrors.
			break;
		default:
			break;
		}
	}

	/**
	 * This overlays Debug information onto the screen if enabled in the SeebrightSDK. Also checks the remote status and shows whether it's connected or not.
	 * Shows both 2D/3D cursor information, the Normalized camera rotation, and the Remote Output.
	 */
	void OnGUI ()
	{
		if (cursorStyle == null) 
		{
			cursorStyle = new GUIStyle ();
			cursorStyle.fontSize = 20;
			cursorStyle.fontStyle = FontStyle.Bold;
			cursorStyle.normal.textColor = Color.white;
			
		}
		if (enableDebugOutput) {
			if(currentCamera.enableGyroCam)
			{
				writeGUI (GUI_LABEL, 25, -200, 100, 100, "NormalizedCamera: \nx:" +
				          SBCamera.normalizedLookRotation.eulerAngles.x + "\ny:" + 
				          SBCamera.normalizedLookRotation.eulerAngles.y + "\nz:" + 
				          SBCamera.normalizedLookRotation.eulerAngles.z);
			}
			#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR 
			writeGUI(GUI_LABEL,Screen.width/4-200, 160, 400, 50, "UnityEdit: " + SBRemote.printRemoteControls());
			#endif
			
		}
		#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR   
		SBRemote.updateRemoteStatus();
		if(SBRemote.remoteStatus)
		{
			if(sleepTimer <= timeTillSleep)
			{
				writeGUI(GUI_LABEL, (Screen.width / 4) - 200, 100, 400, 50, "Remote Connected");
				sleepTimer += Time.deltaTime;
			}
		}
		else
		{
			sleepTimer = 0.0f;
			writeGUI(GUI_LABEL, (Screen.width / 4) - 200, 100, 400, 50, "Remote Disconnected");
		}
		#endif
	}
}

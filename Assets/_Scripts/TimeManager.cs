using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TimeManager : MonoBehaviour {

	public static TimeManager instance;

	public static event Action OnFreezeTime;
	public static event Action OnUnfreezeTime;

	public static bool freezeTime = true;

	void Awake ()
	{
		instance = this;
	}

	public static void FreezeTime ()
	{
		Time.timeScale = 0;
		freezeTime = true;

		if (OnFreezeTime != null) {
			OnFreezeTime ();
		}

	}

	public static void UnfreezeTime ()
	{
		if (OnUnfreezeTime != null) {
			OnUnfreezeTime ();
		}

		Time.timeScale = 1;
		freezeTime = false;


	}

}

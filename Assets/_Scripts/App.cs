using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class App : MonoBehaviour {

	public static bool isExiting;
	public static bool isLoading;

	public static string sceneName;

	public void LateUpdate ()
	{

		if (isLoading) {

			KMonoBehaviour.isLoadingScene = true;

			SceneManager.LoadScene (sceneName);

			isLoading = false;
			sceneName = null;
		}

	}

	public static void LoadScene (string scene_name)
	{
		KMonoBehaviour.isLoadingScene = true;
		isLoading = true;
		sceneName = scene_name;
	}

	void OnApplicationQuit ()
	{
		isExiting = true;
	}


}

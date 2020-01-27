using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class ScreenFader : MonoBehaviour {

	public GUIAnim fadeScreen;

	static ScreenFader Instance;

    public Transform ray;

	void Start ()
	{
		GUIAnimSystem.Instance.m_AutoAnimation = false;
		
		DontDestroyOnLoad (this);

		fadeScreen.gameObject.GetComponent<Image> ().enabled = true;

        if (!DOTween.IsTweening(ray.transform))
            ray.transform.DORotate(new Vector3(0, 0, 360), .3f).SetLoops(-1, LoopType.Incremental);

        //fadeScreen.gameObject.GetComponentInChildren<Text> ().enabled = true;
    }

    private void Update()
    {
       
    }

#if UNITY_EDITOR

    void OnApplicationQuit ()
	{
		Database.SaveAll ();
	}

	#else
	
	void OnApplicationPause(){
		Database.SaveAll ();
	}

	#endif

	public static void LoadSceneWithFade (string sceneName)
	{
		if (Instance == null) {
			CreateInstance ();
		}

		Instance.StartCoroutine (Instance.LoadSceneWithFadeCoroutine (sceneName));
	}

	static void CreateInstance ()
	{
		GameObject sf = Resources.Load ("Prefabs/Fader") as GameObject;

		ScreenFader screenFader = Instantiate (sf).GetComponentInChildren<ScreenFader> ();

		Instance = screenFader;
	}

	bool sceneLoading;

	IEnumerator LoadSceneWithFadeCoroutine (string sceneName)
	{
		if (sceneLoading)
			yield break;
		
		sceneLoading = true;

		fadeScreen.MoveIn (GUIAnimSystem.eGUIMove.SelfAndChildren);

		yield return new WaitForSecondsRealtime (.6f);

		AsyncOperation async = SceneManager.LoadSceneAsync (sceneName, LoadSceneMode.Single);
		async.allowSceneActivation = false;
        async.completed += Async_completed;

        float lastTime = Time.unscaledTime;

		while ((async.progress < .89f  && !async.isDone) || lastTime + .3f >= Time.unscaledTime) {
			yield return null;
		}

		async.allowSceneActivation = true;
        
		yield return null;
        
		sceneLoading = false;

	}

    private void Async_completed(AsyncOperation obj)
    {
        fadeScreen.MoveOut(GUIAnimSystem.eGUIMove.SelfAndChildren);
    }
}

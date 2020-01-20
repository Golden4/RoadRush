using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KScreenManager : MonoBehaviour {
	
	public KScreen[] dialogs;
	public int curDialogIndex;

	Dictionary<string,int> _screens = new Dictionary<string, int> ();

	public static KScreenManager Instance;

	static GraphicRaycaster gr;
	[System.NonSerialized]
	public Canvas canvas;

	public GUIAnim fader;

    public string curScreen;

	void Awake ()
	{
		Instance = this;

		canvas = GetComponent<Canvas> ();

		gr = FindObjectOfType<GraphicRaycaster> ();

		GUIAnimSystem.Instance.m_AutoAnimation = false;

		for (int i = 0; i < dialogs.Length; i++) {
			if (!string.IsNullOrEmpty (dialogs [i].name)) {
				_screens.Add (dialogs [i].name, i);
			} else {
				Debug.LogError (i + "Change screen Name!");
			}
		}

		ShowScreen (curDialogIndex);

	}

	void CreateWhiteFader ()
	{
		if (fader == null) {
			GameObject go = new GameObject ("ScreenFader");

			go.transform.SetParent (canvas.transform, false);

			go.transform.SetAsLastSibling ();

			go.AddComponent<Image> ().color = Color.white;
			go.GetComponent<RectTransform> ().anchorMin = new Vector2 (0, 0);
			go.GetComponent<RectTransform> ().anchorMax = new Vector2 (1, 1);

			fader = go.AddComponent<GUIAnim> ();

			fader.m_FadeIn = new GUIAnim.cFade ();

			fader.m_FadeIn.Enable = true;
			fader.m_FadeIn.Began = false;
			fader.m_FadeIn.Time = .2f;

			fader.m_FadeOut = new GUIAnim.cFade ();

			fader.m_FadeOut.Enable = true;
			fader.m_FadeOut.Time = .2f;
			fader.m_FadeOut.Delay = .2f;
		}
	}

	public void ShowScreen (int index)
	{
		if (fader != null) {
			fader.gameObject.SetActive (true);
			fader.MoveIn (GUIAnimSystem.eGUIMove.Self);
			fader.MoveOut (GUIAnimSystem.eGUIMove.Self);
		}

		Debug.Log ("ShowedScreen" + index);

		for (int i = 0; i < dialogs.Length; i++) {
			
			if (i == index)
				dialogs [i].Activate ();
			else
				dialogs [i].Deactivate ();
			
		}

        if (index >= 0 && index < dialogs.Length)
            curScreen = dialogs[index].name;
        else curScreen = "";

        curDialogIndex = index;

	}

	public void ShowScreen (string name)
	{
		int index = -1;

		if (!_screens.TryGetValue (name, out index)) {
			Debug.LogError ("Not Founded Screen: " + name);
		}
		if (index >= 0)
			ShowScreen (index);

        curScreen = name;
    }

	public void ShowOneMoreChanceScreen ()
	{

		if (!EndGameController.Instance.chanceGived) {
			ShowScreen (1);
			CameraController.BlurEnable ();
			StartCoroutine (EndGameController.Instance.StartCountDown ());
		} else {
			ShowEndGameScreen ();
		}


	}

	public void ShowEndGameScreen ()
	{
		CameraController.BlurEnable ();

		EndGameController.Instance.CalculateStats ();
		ShowScreen (2);

	}

	public static void EnableRaycaster (bool enable)
	{
		gr.enabled = enable;
	}


	/*	IEnumerator Start ()
	{

		for (int i = 0; i < dialogs.Length; i++) {
			dialogs [i].gameObject.SetActive (true);
		}

		yield return null;

		curDialogIndex = 0;

		dialogs [0].MoveIn (GUIAnimSystem.eGUIMove.SelfAndChildren);

	}

	public void ShowDialog (int index)
	{
		if (index < dialogs.Length) {
			
			StartCoroutine ("showDialogCoroutine", index);

		} else {
			Debug.LogWarning ("Out array length!");
		}
	}

	IEnumerator showDialogCoroutine (int index)
	{
		StartCoroutine (DisableButtons (index, 1.3f));

		dialogs [curDialogIndex].MoveOut (GUIAnimSystem.eGUIMove.SelfAndChildren);

		yield return new WaitForSeconds (.4f);

		curDialogIndex = index;

		dialogs [index].MoveIn (GUIAnimSystem.eGUIMove.SelfAndChildren);

	}

	IEnumerator DisableButtons (int index, float delay)
	{

		GUIAnimSystem.Instance.EnableAllGraphicRaycasters (false);

		yield return new WaitForSeconds (delay);

		for (int i = 0; i < dialogs.Length; i++) {
			Button[] buttons = dialogs [i].GetComponentsInChildren<Button> ();
			for (int j = 0; j < buttons.Length; j++) {
				buttons [j].enabled = i == index;
			}
		}

		yield return null;

		GUIAnimSystem.Instance.EnableAllGraphicRaycasters (true);

	}*/

}

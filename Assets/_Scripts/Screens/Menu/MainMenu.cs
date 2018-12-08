using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

	public RectTransform rays;

	void Start ()
	{
/*		if (PlayerPrefs.HasKey ("Control")) {
			inputIndex = PlayerPrefs.GetInt ("Control");
		}

		BtnTextUpdate ();

		changeControlBtn.onClick.AddListener (() => {

			if (inputIndex < 1)
				inputIndex++;
			else {
				inputIndex = 0;
			}
			print (inputIndex);

			BtnTextUpdate ();
			ChangeControlIndex ();

		});*/

	}

	void Update ()
	{
		rays.transform.localEulerAngles += new Vector3 (0, 0, -.1f);
	}

	public void GoToGarage ()
	{
		ScreenFader.LoadSceneWithFade ("Shop");
	}

	/*	void BtnTextUpdate ()
	{
		changeControlBtn.GetComponentInChildren<Text> ().text = ((InputInfos)inputIndex).ToString ();
	}*/

	public enum InputInfos
	{
		Gyro,
		Touch
	}

}



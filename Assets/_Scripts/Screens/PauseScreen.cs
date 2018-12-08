using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScreen : MonoBehaviour {
	
	public void RestartMessage ()
	{
		DialogBox.Show ("Restart?", "All progress will be lost", () => {
			ScreenFader.LoadSceneWithFade (Application.loadedLevelName);
		});
	}

	public void GoToGarage ()
	{

		DialogBox.Show ("Go to garage?", "All progress will be lost", () => {
			ScreenFader.LoadSceneWithFade ("Shop");
		});
	}
}

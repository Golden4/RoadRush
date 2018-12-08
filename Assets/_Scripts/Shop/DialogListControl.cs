using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DialogListControl : MonoBehaviour {

	public enum ScreenType
	{
		GameMode,
		Location
	}

	public ScreenType levelInfo;

	public Button[] dialogBtns;

	public Button backBtn;

	/*void Start ()
	{

		for (int i = 0; i < dialogBtns.Length; i++) {
			
			int index = i;

			dialogBtns [i].onClick.AddListener (() => {
				switch (levelInfo) {
				case ScreenType.GameMode:
					ChangeGameMode (index);
					KScreenManager.Instance.ShowScreen ("Location");
					break;
				case ScreenType.Location:
					ChangeLocation (index);
					ScreenFader.LoadSceneWithFade ("1");
					break;
				}
			});


			if (levelInfo == ScreenType.GameMode) {
				dialogBtns [i].transform.Find ("Record").GetComponentInChildren<Text> ().text = Database.GetLevelRecord ((LevelInfo.GameMode)i).ToString ();
			}

		}

		backBtn.onClick.AddListener (() => {
			KScreenManager.Instance.ShowScreen ("Shop");
		});

	}*/

	void ChangeGameMode (int index)
	{
		LevelInfo.gameMode = (LevelInfo.GameMode)index;
		print ((LevelInfo.GameMode)index);
	}

	void ChangeLocation (int index)
	{
		LevelInfo.enviroment = (LevelInfo.EnviromentType)index;
		print ((LevelInfo.EnviromentType)index);
	}

}

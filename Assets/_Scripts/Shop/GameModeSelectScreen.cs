using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameModeSelectScreen : MonoBehaviour {

	public Button[] dialogBtns;

	public Button backBtn;

	void Start ()
	{
		for (int i = 0; i < dialogBtns.Length; i++) {

			int index = i;

			dialogBtns [i].onClick.AddListener (() => {

				ChangeGameMode (index);
				KScreenManager.Instance.ShowScreen ("Location");
			});


			dialogBtns [i].transform.Find ("Record").GetComponentInChildren<Text> ().text = Database.GetLevelRecord ((LevelInfo.GameMode)i).ToString ();
		}

		backBtn.onClick.AddListener (() => {
			KScreenManager.Instance.ShowScreen ("Shop");
		});
	}

	void ChangeGameMode (int index)
	{
		LevelInfo.gameMode = (LevelInfo.GameMode)index;
		print ((LevelInfo.GameMode)index);
	}

}

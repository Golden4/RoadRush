using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocationSelectScreen : MonoBehaviour {

	public Location[] dialogBtns;

	void Start ()
	{
		UpdateDialogs ();
	}

	void BuyLocation (int index)
	{
		if (GameManager.Buy (dialogBtns [index].price)) {
			AudioManager.Instance.PlaySound2D ("Buy");
			Database.PlayerData.levelsData.locationsAvaible.Add (index);
			UpdateDialogs ();
		}
	}

	void UpdateDialogs ()
	{
		for (int i = 0; i < dialogBtns.Length; i++) {
			Transform price = null;

			dialogBtns [i].button.onClick.RemoveAllListeners ();

			if (i != 0)
				price = dialogBtns [i].button.transform.Find ("Price");

			int index = i;

			if (!Database.PlayerData.levelsData.locationsAvaible.Exists (x => x == i) && i != 0) {
				price.gameObject.SetActive (true);
				price.GetComponentInChildren<Text> ().text = dialogBtns [i].price.ToString ();

				dialogBtns [i].button.onClick.AddListener (() => {
					BuyLocation (index);
				});

			} else {
				if (i != 0)
					price.gameObject.SetActive (false);

				dialogBtns [i].button.onClick.AddListener (() => {
					ChangeLocation (index);
					ScreenFader.LoadSceneWithFade ("1");
				});

			}

		}
	}

	void ChangeLocation (int index)
	{
		LevelInfo.enviroment = (LevelInfo.EnviromentType)index;
		print ((LevelInfo.EnviromentType)index);
	}

	[System.Serializable]
	public class Location {
		public int price;
		public Button button;
	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorsList : MonoBehaviour {

	public GameObject colorPrefab;

	public GameObject playBtn;

	public Button buyBtn;

	Image[] colors;

	public int selectedColorIndex;

	public bool colorsIsShowing = false;

	public Button colorBtn;

	BoostList boostList;

	public static ColorsList Instance;

	void Awake ()
	{
		Instance = this;

		boostList = FindObjectOfType<BoostList> ();

		colors = new Image[10];

		InitColorsList ();

		colorBtn.onClick.AddListener (() => {

			if (colorsIsShowing) {
				HideColors ();
			} else {

				if (boostList.boostBtnsIsShowing)
					boostList.HideBoostBtns ();

				ShowColors ();

			}
		});
	}

	void BuyColor (int colorIndex)
	{
		if (GameManager.Buy (Database.PlayerData.playerCarData [ShopController.curCarIndex].avaibleColorList [colorIndex].price)) {
			AudioManager.Instance.PlaySound2D ("Buy");
			Database.PlayerData.playerStats [ShopController.curCarIndex].colorsIsAvaible.Add (colorIndex);
			ShopController.Instance.ChangeCarColor (colorIndex);

		}
	}

	void InitColorsList ()
	{
		for (int i = 0; i < colors.Length; i++) {
			GameObject go = Instantiate (colorPrefab);

			go.transform.SetParent (transform, false);

			colors [i] = go.GetComponent<Image> ();

			Button btn = go.gameObject.AddComponent<Button> ();

			int colorIndex = i;

			btn.onClick.AddListener (() => {
				if (selectedColorIndex != colorIndex) {
					
					selectedColorIndex = colorIndex;

					AudioManager.Instance.PlaySound2D ("Spray");
					ShopController.Instance.ChangeCarColor (colorIndex);
					SelectColor (colorIndex);
				}
			});
		}
	}

	void UpdateColorsList (CarColor[] avaibleCarColors)
	{
		for (int i = 0; i < colors.Length; i++) {

			if (i < avaibleCarColors.Length) {
				colors [i].gameObject.SetActive (true);
				colors [i].color = avaibleCarColors [i].color;
			} else {
				colors [i].gameObject.SetActive (false);
			}
		}
	}

	void SelectColor (int index)
	{
		if (Database.PlayerData.playerStats [ShopController.curCarIndex].colorsIsAvaible.FindIndex (x => x == index) != -1) {
			playBtn.SetActive (true);
			buyBtn.gameObject.SetActive (false);
		} else {
			playBtn.SetActive (false);
			buyBtn.gameObject.SetActive (true);
			buyBtn.transform.Find ("Text").GetComponent<Text> ().text = Database.PlayerData.playerCarData [ShopController.curCarIndex].avaibleColorList [index].price.ToString ();
			buyBtn.onClick.RemoveAllListeners ();

			buyBtn.onClick.AddListener (() => {
				BuyColor (index);
				SelectColor (index);
			});
		}

		for (int i = 0; i < colors.Length; i++) {
			colors [i].gameObject.GetComponent<Outline> ().effectColor = (i == index) ? new Color (255 / 255f, 243 / 255f, 88f / 255f) : Color.white;

			int indexC = Database.PlayerData.playerStats [ShopController.curCarIndex].colorsIsAvaible.FindIndex (x => x == i);

			colors [i].transform.GetChild (0).gameObject.SetActive (indexC == -1);
		}

	}

	float lastTimeShow = -1f;

	public void ShowColors ()
	{
		if (!colorsIsShowing && lastTimeShow + .2f < Time.time) {
			lastTimeShow = Time.time;
			CarColor[] avaibleCarColors = Database.PlayerData.playerCarData [ShopController.curCarIndex].avaibleColorList;

			UpdateColorsList (avaibleCarColors);

			/*if (selectedColorIndex > -1) {
				SelectColor (selectedColorIndex);
			}
			else {*/

			SelectColor (Database.PlayerData.playerStats [ShopController.curCarIndex].colorIndex);
			//}

			for (int i = 0; i < colors.Length; i++) {
				if (transform.GetChild (i).gameObject.activeInHierarchy)
					transform.GetChild (i).GetComponent<GUIAnim> ().MoveIn (GUIAnimSystem.eGUIMove.Self);
			}

			AudioManager.Instance.PlaySound2D ("Click");
			colorsIsShowing = true;
		}
	}

	public void HideColors ()
	{
		if (colorsIsShowing && lastTimeShow + .2f < Time.time) {
			lastTimeShow = Time.time;

			for (int i = 0; i < colors.Length; i++) {
				if (transform.GetChild (i).gameObject.activeInHierarchy)
					transform.GetChild (i).GetComponent<GUIAnim> ().MoveOut (GUIAnimSystem.eGUIMove.Self);
			}
			AudioManager.Instance.PlaySound2D ("Click");
			colorsIsShowing = false;
		}

		//if (curCarIndex != ShopController.curCarIndex) {
		playBtn.SetActive (true);
		buyBtn.gameObject.SetActive (false);

		ShopController.Instance.ChangeCarColor (Database.PlayerData.playerStats [ShopController.curCarIndex].colorIndex);
		SelectColor (Database.PlayerData.playerStats [ShopController.curCarIndex].colorIndex);
		selectedColorIndex = -1;
		//}


	}

	public void HideBtnsAndColors ()
	{
		if (colorsIsShowing)
			HideColors ();
		
		colorBtn.gameObject.SetActive (false);
	}

	public void ShowBtnsAndColors ()
	{
		colorBtn.gameObject.SetActive (true);
	}

}
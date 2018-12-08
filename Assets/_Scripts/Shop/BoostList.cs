using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoostList : MonoBehaviour {

	public GameObject boostPrefab;

	public Button boostBtn;

	public bool boostBtnsIsShowing;

	ColorsList colorList;

	public static BoostList Instance;

	void Awake ()
	{
		Instance = this;

		colorList = FindObjectOfType<ColorsList> ();

		string[] keys = new string[]{ "max_speed", "acceleration", "handling", "breaking", "nitro", "durability" };

		string[] boostNames = new string[keys.Length]; 

		for (int i = 0; i < keys.Length; i++) {
			boostNames [i] = LocalizationManager.GetLocalizedText (keys [i]);
		}

		upgradeBtns = new Button[boostNames.Length];

		for (int i = 0; i < boostNames.Length; i++) {
			InstatiateBoostPrefab (boostNames [i], i);
		}
	}

	void Start ()
	{
		UpdateBtnsListeners ();
		UpdateBntsPrices ();

		boostBtn.onClick.AddListener (() => {
			
			if (boostBtnsIsShowing) {
				HideBoostBtns ();
			} else {

				if (colorList.colorsIsShowing)
					colorList.HideColors ();

				ShowBoostBtns ();
			}
		});
	}

	public void UpdateBtnsListeners ()
	{
		for (int i = 0; i < upgradeBtns.Length; i++) {
			
			int index = i;

			upgradeBtns [i].onClick.AddListener (() => {
				ShopController.UpgradeCar ((ShopController.CarStatInfos)index);
			});
		}
	}

	public static void UpdateBntsPrices ()
	{
		for (int i = 0; i < upgradeBtns.Length; i++) {
			string text = "";

			if (ShopController.GetBoostLevel ((ShopController.CarStatInfos)i) < ShopController.maxBoostLevel) {
				text = ShopController.GetUpgradePrice ((ShopController.CarStatInfos)i).ToString ();
				upgradeBtns [i].transform.GetChild (1).gameObject.SetActive (true);
				upgradeBtns [i].transform.GetChild (2).gameObject.SetActive (false);
				upgradeBtns [i].transform.GetChild (1).GetComponent<Text> ().text = text;
			} else {
				text = "max";
				upgradeBtns [i].transform.GetChild (1).gameObject.SetActive (false);
				upgradeBtns [i].transform.GetChild (2).gameObject.SetActive (true);
				upgradeBtns [i].transform.GetChild (2).GetComponent<Text> ().text = text;
			}


		}
	}

	static	Button[] upgradeBtns;

	void InstatiateBoostPrefab (string name, int index)
	{
		GameObject go = Instantiate (boostPrefab);
		go.transform.SetParent (transform, false);
		upgradeBtns [index] = go.gameObject.AddComponent<Button> ();

		Text t = go.transform.GetChild (0).GetComponent<Text> ();
		t.text = name;

		t.font = LocalizationManager.curLanguage.font;
	}

	float lastTimeShow = -1f;

	public void ShowBoostBtns ()
	{

		if (!boostBtnsIsShowing && lastTimeShow + .2f < Time.time) {
			lastTimeShow = Time.time;

			for (int i = 0; i < transform.childCount; i++) {
				transform.GetChild (i).GetComponent<GUIAnim> ().MoveIn (GUIAnimSystem.eGUIMove.Self);
			}

			AudioManager.Instance.PlaySound2D ("Click");
			boostBtnsIsShowing = true;
		}

	}

	public void HideBoostBtns ()
	{

		if (boostBtnsIsShowing && lastTimeShow + .2f < Time.time) {
			lastTimeShow = Time.time;

			for (int i = 0; i < transform.childCount; i++) {
				transform.GetChild (i).GetComponent<GUIAnim> ().MoveOut (GUIAnimSystem.eGUIMove.Self);
			}
			AudioManager.Instance.PlaySound2D ("Click");
			boostBtnsIsShowing = false;
		}
	}

	public void HideBtnsAndBoosts ()
	{
		if (boostBtnsIsShowing)
			HideBoostBtns ();
		
		boostBtn.gameObject.SetActive (false);
	}

	public void ShowBtnsAndBoosts ()
	{
		boostBtn.gameObject.SetActive (true);
	}

}

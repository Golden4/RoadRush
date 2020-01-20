using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour {

	public GameObject[] cars;
	public Button nextBtn;
	public Button prevBtn;
	public Button playBtn;
	public GameObject buyBtn;
	public Text[] moneyCount;
	public Text[] rubyCount;

	public static int curCarIndex = 0;
	public static int maxBoostLevel = 5;
	public static float upgradePercent = .05f;

	public static ShopController Instance;

	void Awake ()
	{
		Instance = this;

		TimeManager.UnfreezeTime ();

		EventManager.OnMoneyChangedEvent += ChangeMoney;
		EventManager.OnRubyChangedEvent += ChangeRuby;
	}

	void OnDestroy ()
	{
		EventManager.OnMoneyChangedEvent -= ChangeMoney;
		EventManager.OnRubyChangedEvent -= ChangeRuby;
	}
    
	void Start ()
	{
		SpawnCars ();

		for (int i = 0; i < moneyCount.Length; i++) {
			moneyCount [i].text = GameManager.money.ToString ();
			rubyCount [i].text = GameManager.ruby.ToString ();
		}

		curCarIndex = Database.PlayerData.curCarIndex;

		ShowCar (curCarIndex);

		UpdateCarsColors ();

		playBtn.onClick.AddListener (() => {

			KScreenManager.Instance.ShowScreen ("GameMode");

			Database.PlayerData.curCarIndex = curCarIndex;

		});
	}


    void ChangeMoney(int startMoney, int finalMoney)
    {
        for (int i = 0; i < moneyCount.Length; i++)
        {
            moneyCount[i].AnimateValue(this, startMoney, finalMoney, .5f);
        }
    }

    void ChangeRuby(int startRuby, int finalRuby)
    {
        for (int i = 0; i < rubyCount.Length; i++)
        {
            rubyCount[i].AnimateValue(this, startRuby, finalRuby, .5f);
        }
    }

    void SpawnCars ()
	{
		cars = new GameObject[Database.PlayerData.playerCarData.Length];

		for (int i = 0; i < Database.PlayerData.playerCarData.Length; i++) {
			GameObject car = Instantiate (Database.PlayerData.playerCarData [i].playerPrefab.gameObject);
			cars [i] = car.gameObject;
			cars [i].isStatic = true;
			car.transform.position = Vector3.zero;
			car.transform.eulerAngles = Vector3.zero;

			foreach (Component item in car.GetComponents<Component>()) {
				if (item.GetType () != typeof(Transform) && item.GetType () != typeof(MeshFilter) && item.GetType () != typeof(MeshRenderer)) {
					DestroyImmediate (item);
				}
			}

			car.gameObject.AddComponent<MeshCollider> ();

			/*if (Database.PlayerData.playerStats [i].bought) {
				ActivateColorCar (i);
			} else {
				DeactivateColorCar (i);
			}*/
		}

	}

	void UpdateCarsColors ()
	{
		for (int i = 0; i < cars.Length; i++) {
			ChangeCarColor (Database.PlayerData.playerStats [i].colorIndex, i);
		}
	}

	void DeactivateColorCar (int index)
	{

		ChangeCarColor (Database.PlayerData.playerStats [index].colorIndex, index);

		Material[] materials = cars [index].GetComponent<MeshRenderer> ().materials;

		for (int i = 0; i < materials.Length; i++) {

			Color color = materials [i].color;

			color.b = Mathf.Clamp01 (color.b - .7f);
			color.r = Mathf.Clamp01 (color.r - .7f);
			color.g = Mathf.Clamp01 (color.g - .7f);

			materials [i].color = color;
		}

	}

	void ActivateColorCar (int index)
	{
		foreach (Material mat in cars[index].GetComponent<MeshRenderer>().materials) {

			mat.color = Color.white;

		}

		ChangeCarColor (Database.PlayerData.playerStats [index].colorIndex, index);
	}

	void ShowCar (int index)
	{
		for (int i = 0; i < cars.Length; i++) {
			cars [i].gameObject.SetActive (i == index);
		}

		curCarIndex = index;

		if (!Database.PlayerData.playerStats [curCarIndex].bought) {
			UpdateBuyBtn ();
			ColorsList.Instance.HideBtnsAndColors ();
			BoostList.Instance.HideBtnsAndBoosts ();

			playBtn.gameObject.SetActive (false);

		} else {
			buyBtn.gameObject.SetActive (false);
			ColorsList.Instance.ShowBtnsAndColors ();
			BoostList.Instance.ShowBtnsAndBoosts ();
			playBtn.gameObject.SetActive (true);
		}


		CarStatController.updateStats = true;

		BoostList.UpdateBntsPrices ();
		BoostList.Instance.HideBoostBtns ();
		ColorsList.Instance.HideColors ();
		ColorsList.Instance.selectedColorIndex = -1;

		ChangeCarColor (Database.PlayerData.playerStats [index].colorIndex);

	}

	void UpdateBuyBtn ()
	{
		buyBtn.gameObject.SetActive (true);

		Button buyButton = buyBtn.GetComponentInChildren<Button> ();

		buyButton.onClick.RemoveAllListeners ();

		string buyButtonText = "";

		if (Database.PlayerData.playerCarData [curCarIndex].needLevelToUnlock > GameManager.CurLevel && !Database.PlayerData.playerStats [curCarIndex].unlocked) {
			buyButtonText = LocalizationManager.GetLocalizedText ("unlock");

			buyBtn.transform.Find ("UnlockPrice").gameObject.SetActive (true);
			buyBtn.transform.Find ("Price").gameObject.SetActive (false);

			buyBtn.transform.Find ("UnlockPrice").GetComponent<Text> ().text = Database.PlayerData.playerCarData [curCarIndex].priceToUnlock.ToString ();
			buyBtn.transform.Find ("UnlockPrice").GetChild (0).GetComponent<Text> ().text = Database.PlayerData.playerCarData [curCarIndex].needLevelToUnlock + " " + LocalizationManager.GetLocalizedText ("lvl");

			buyButton.onClick.AddListener (() => {
				UnlockCar (curCarIndex);
			});

		} else {
			buyButtonText = LocalizationManager.GetLocalizedText ("buy");

			buyBtn.transform.Find ("UnlockPrice").gameObject.SetActive (false);
			buyBtn.transform.Find ("Price").gameObject.SetActive (true);

			buyBtn.transform.Find ("Price").GetComponentInChildren<Text> ().text = Database.PlayerData.playerCarData [curCarIndex].price.ToString ();

			buyButton.onClick.AddListener (() => {
				BuyCar (curCarIndex);
			});

		}

		buyButton.GetComponentInChildren<Text> ().text = buyButtonText;


	}



	public void ShowNextCar ()
	{
		curCarIndex++;

		if (curCarIndex == cars.Length) {
			curCarIndex = 0;
		}

		AudioManager.Instance.PlaySound2D ("Button");

		ShowCar (curCarIndex);
	}

	public void ShowPrevCar ()
	{
		curCarIndex--;

		if (curCarIndex == -1) {
			curCarIndex = cars.Length - 1;
		}

		AudioManager.Instance.PlaySound2D ("Button");

		ShowCar (curCarIndex);

	}

	void BuyCar (int carIndex)
	{
		if (GameManager.Buy (Database.PlayerData.playerCarData [carIndex].price)) {
			
			AudioManager.Instance.PlaySound2D ("Buy");

			Database.PlayerData.playerStats [carIndex].bought = true;

			ShowCar (carIndex);

			Database.PlayerData.curCarIndex = carIndex;

		}
	}

	void UnlockCar (int carIndex)
	{
		
		if (GameManager.BuyWithRuby (Database.PlayerData.playerCarData [carIndex].priceToUnlock)) {

			//AudioManager.Instance.PlaySound2D ("Buy");

			Database.PlayerData.playerStats [carIndex].unlocked = true;

			ShowCar (carIndex);

		}
	}

	public static int startUpgradePrice = 1000;
	public static int nextUpgradePriceMultiplier = 500;

	public static void UpgradeCar (CarStatInfos carStat)
	{

		CarStats stats = Database.PlayerData.playerStats [curCarIndex];

		if (GetBoostLevel (carStat) == maxBoostLevel)
			return;
		

		switch (carStat) {
		case CarStatInfos.MaxSpeed:
			if (GameManager.Buy (GetUpgradePrice (carStat))) {
				stats.boostLvlMaxSpeed++;
				stats.boostLvlMaxSpeed = Mathf.Clamp (stats.boostLvlMaxSpeed, 0, maxBoostLevel);
			}

			break;
		case CarStatInfos.Acceleration:
			if (GameManager.Buy (GetUpgradePrice (carStat))) {
				stats.boostLvlAcceleration++;
				stats.boostLvlAcceleration = Mathf.Clamp (stats.boostLvlAcceleration, 0, maxBoostLevel);
			}
			break;
		case CarStatInfos.Handling:
			if (GameManager.Buy (GetUpgradePrice (carStat))) {
				stats.boostLvlHandling++;
				stats.boostLvlHandling = Mathf.Clamp (stats.boostLvlHandling, 0, maxBoostLevel);
			}
			break;
		case CarStatInfos.Breaking:
			if (GameManager.Buy (GetUpgradePrice (carStat))) {
				stats.boostLvlBreaking++;
				stats.boostLvlBreaking = Mathf.Clamp (stats.boostLvlBreaking, 0, maxBoostLevel);
			}
			break;
		
		case CarStatInfos.Nitro:
			if (GameManager.Buy (GetUpgradePrice (carStat))) {
				stats.boostLvlNitro++;
				stats.boostLvlNitro = Mathf.Clamp (stats.boostLvlNitro, 0, maxBoostLevel);
			}
			break;

		case CarStatInfos.Durability:
			if (GameManager.Buy (GetUpgradePrice (carStat))) {
				stats.boostLvlDurability++;
				stats.boostLvlDurability = Mathf.Clamp (stats.boostLvlDurability, 0, maxBoostLevel);
			}
			break;
		}

		BoostList.UpdateBntsPrices ();
		CarStatController.updateStats = true;

	}

	public static int GetUpgradePrice (CarStatInfos carStat)
	{
		return (GetBoostLevel (carStat) * nextUpgradePriceMultiplier + startUpgradePrice) * Database.PlayerData.playerCarData [curCarIndex].price / 25000;
	}


	public static int GetBoostLevel (CarStatInfos carStat)
	{
		
		switch (carStat) {
		case CarStatInfos.MaxSpeed:
			return Database.PlayerData.playerStats [curCarIndex].boostLvlMaxSpeed;
		case CarStatInfos.Acceleration:
			return Database.PlayerData.playerStats [curCarIndex].boostLvlAcceleration;

		case CarStatInfos.Handling:
			return Database.PlayerData.playerStats [curCarIndex].boostLvlHandling;

		case CarStatInfos.Breaking:
			return Database.PlayerData.playerStats [curCarIndex].boostLvlBreaking;

		case CarStatInfos.Nitro:
			return Database.PlayerData.playerStats [curCarIndex].boostLvlNitro;

		case CarStatInfos.Durability:
			return Database.PlayerData.playerStats [curCarIndex].boostLvlDurability;

		}

		return -1;
	}

	public void ChangeCarColor (int colorIndex, int index = -1)
	{
		int carIndex = (index == -1) ? curCarIndex : index;

		Color carColor = Database.PlayerData.playerCarData [carIndex].avaibleColorList [colorIndex].color;

		cars [carIndex].GetComponent<Renderer> ().materials [1].color = carColor;

		CheckAndSaveCarColor (colorIndex, carIndex);

	}

	public void CheckAndSaveCarColor (int colorIndex, int carIndex)
	{
		if (Database.PlayerData.playerStats [carIndex].colorsIsAvaible.FindIndex (x => x == colorIndex) > -1) {
			Database.PlayerData.playerStats [carIndex].colorIndex = colorIndex;
		}
	}


	public enum CarStatInfos
	{
		MaxSpeed,
		Acceleration,
		Handling,
		Breaking,
		Nitro,
		Durability
	}

}

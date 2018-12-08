using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarStatController : MonoBehaviour {

	[Space (10)]
	public RectTransform maxSpeedStats;
	public RectTransform accelerationStats;
	public RectTransform handlingStats;
	public RectTransform breakingStats;
	public RectTransform nitroStats;
	public RectTransform durabilityStats;
	[Space (10)]
	public RectTransform maxSpeedStatsBoost;
	public RectTransform accelerationStatsBoost;
	public RectTransform handlingStatsBoost;
	public RectTransform breakingStatsBoost;
	public RectTransform nitroStatsBoost;
	public RectTransform durabilityStatsBoost;
	[Space (10)]
	public float maxSpeed;
	public float maxAcceleration;
	public float maxHandling;
	public float maxBreaking;
	public float maxNitro;
	public float maxDurability;
	[Space (10)]
	public RectTransform xpBarSlider;
	public Text levelNum;
	public Text XPAmount;

	public static bool updateStats = false;
	public static bool updateXPBar = false;

	ShopController shopController;

	void Start ()
	{
		shopController = GetComponent<ShopController> ();
		UpdateStats ();
		UpdateXPbar ();
	}

	void Update ()
	{
		if (updateStats) {
			updateStats = false;
			UpdateStats ();
		}

		if (updateXPBar) {
			updateXPBar = false;
			UpdateXPbar ();
		}

		LerpValues ();

	}

	float maxSpeedValue;
	float maxAccelerationValue;
	float maxHandlingValue;
	float maxBreakingValue;
	float maxNitroValue;
	float maxDurabilityValue;
	CarStats stats;

	public void UpdateStats ()
	{
		PlayerCar car = Database.PlayerData.playerCarData [ShopController.curCarIndex].playerPrefab;

		stats = Database.PlayerData.playerStats [ShopController.curCarIndex];

		maxSpeedValue = car.maxSpeed / maxSpeed;
		maxAccelerationValue = car.acceleration / maxAcceleration;
		maxHandlingValue = car.handling / maxHandling;
		maxBreakingValue = car.breaking / maxBreaking;
		maxNitroValue = car.nitroAcceleration / maxNitro;
		maxDurabilityValue = car.deadSpeed / maxDurability;
	}

	float lerpSpeed = 7;

	void LerpValues ()
	{
		maxSpeedStats.localScale = Vector3.Lerp (maxSpeedStats.localScale, new Vector3 (maxSpeedValue, 1, 1), Time.deltaTime * lerpSpeed);
		accelerationStats.localScale = Vector3.Lerp (accelerationStats.localScale, new Vector3 (maxAccelerationValue, 1, 1), Time.deltaTime * lerpSpeed);
		handlingStats.localScale = Vector3.Lerp (handlingStats.localScale, new Vector3 (maxHandlingValue, 1, 1), Time.deltaTime * lerpSpeed);
		breakingStats.localScale = Vector3.Lerp (breakingStats.localScale, new Vector3 (maxBreakingValue, 1, 1), Time.deltaTime * lerpSpeed);
		nitroStats.localScale = Vector3.Lerp (nitroStats.localScale, new Vector3 (maxNitroValue, 1, 1), Time.deltaTime * lerpSpeed);
		durabilityStats.localScale = Vector3.Lerp (durabilityStats.localScale, new Vector3 (maxDurabilityValue, 1, 1), Time.deltaTime * lerpSpeed);

		maxSpeedStatsBoost.localScale = Vector3.Lerp (maxSpeedStatsBoost.localScale, new Vector3 (maxSpeedValue + ShopController.upgradePercent * stats.boostLvlMaxSpeed, 1, 1), Time.deltaTime * lerpSpeed);
		accelerationStatsBoost.localScale = Vector3.Lerp (accelerationStatsBoost.localScale, new Vector3 (maxAccelerationValue + ShopController.upgradePercent * stats.boostLvlAcceleration, 1, 1), Time.deltaTime * lerpSpeed);
		handlingStatsBoost.localScale = Vector3.Lerp (handlingStatsBoost.localScale, new Vector3 (maxHandlingValue + ShopController.upgradePercent * stats.boostLvlHandling, 1, 1), Time.deltaTime * lerpSpeed);
		breakingStatsBoost.localScale = Vector3.Lerp (breakingStatsBoost.localScale, new Vector3 (maxBreakingValue + ShopController.upgradePercent * stats.boostLvlBreaking, 1, 1), Time.deltaTime * lerpSpeed);
		nitroStatsBoost.localScale = Vector3.Lerp (nitroStatsBoost.localScale, new Vector3 (maxNitroValue + ShopController.upgradePercent * stats.boostLvlNitro, 1, 1), Time.deltaTime * lerpSpeed);
		durabilityStatsBoost.localScale = Vector3.Lerp (durabilityStatsBoost.localScale, new Vector3 (maxDurabilityValue + ShopController.upgradePercent * stats.boostLvlDurability, 1, 1), Time.deltaTime * lerpSpeed);
	}

	public void UpdateXPbar ()
	{
		xpBarSlider.localScale = new Vector3 (GameManager.CurXPForCurLevel / (float)GameManager.NeedXPforNextLevel (GameManager.CurLevel), 1, 1);
		levelNum.text = GameManager.CurLevel.ToString ();
		XPAmount.text = GameManager.CurXPForCurLevel + " / " + GameManager.NeedXPforNextLevel (GameManager.CurLevel).ToString ();
	}




}

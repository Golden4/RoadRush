using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGameController : SingletonGeneric<EndGameController> {

	[Header ("OneMoreChanceScreen")]
	public Text continueCountDownText;

	[Header ("EndGameScreen")]
	public Text totalScore;
	[Space (10)]
	public Text distanceText;
	public Text averageSpeedText;
	public Text highSpeedText;
	public Text drivingOppositeText;
	public Text dangerousOvertacingText;
	public Text totalText;
	[Space (10)]
	public Text distanceMoneyText;
	public Text averageSpeedMoneyText;
	public Text highSpeedMoneyText;
	public Text drivingOppositeMoneyText;
	public Text dangerousOvertacingMoneyText;
	public Text totalMoneyText;
	[Space (10)]
	public RectTransform xpBarSlider;
	public RectTransform xpBarProgressSlider;
	public Text levelNum;
	//public Text XPAmount;
	public bool chanceGived = false;


	int continueCountDown = 5;

	public IEnumerator StartCountDown ()
	{
		continueCountDown = 5;

		while (continueCountDown > 0) {

			continueCountDownText.text = continueCountDown.ToString ();

			yield return new WaitForSeconds (1);

			if (KScreenManager.Instance.curDialogIndex != 1)
				yield break;

			continueCountDown--;
		}

		KScreenManager.Instance.ShowEndGameScreen ();

	}

	public void CalculateStats ()
	{
		
		string score = LocalizationManager.GetLocalizedText ("score");
		totalScore.text = string.Format (score, Mathf.RoundToInt (ScoreController.Instance.score).ToString ());
		distanceText.text = (ScoreController.Instance.distance / 1000).ToString ("F") + " km";
		averageSpeedText.text = Mathf.RoundToInt (ScoreController.Instance.averageSpeed) + " km/h";
		highSpeedText.text = ScoreController.Instance.highSpeedTime.ToString ("F") + " sec.";
		dangerousOvertacingText.text = ScoreController.Instance.dangerousOvertacing.ToString ();
		drivingOppositeText.text = ScoreController.Instance.drivingOppositeTime.ToString ("F") + " sec.";

		Database.SetLevelRecord (LevelInfo.gameMode, Mathf.RoundToInt (ScoreController.Instance.score));

		CalculateMoney ();
	}

	void CalculateMoney ()
	{
		int totalMoney = 0;

		int distanceMoney = Mathf.RoundToInt (ScoreController.Instance.distance / 10);
		int averageSpeedMoney = Mathf.RoundToInt (ScoreController.Instance.averageSpeed / 50 * distanceMoney);
		int highSpeedMoney = Mathf.RoundToInt (ScoreController.Instance.highSpeedTime * 10);
		int dangerousOvertacing = ScoreController.Instance.dangerousOvertacing * 30;
		int drivingOppositeMoney = Mathf.RoundToInt (ScoreController.Instance.drivingOppositeTime * 20);
		totalMoney += distanceMoney + highSpeedMoney + drivingOppositeMoney + dangerousOvertacing;

		GameManager.AddMoney (totalMoney);

		distanceMoneyText.AnimateValue (this, 0, distanceMoney, .5f, "+", "", .05f);
		averageSpeedMoneyText.AnimateValue (this, 0, averageSpeedMoney, .5f, "+", "", .2f);
		dangerousOvertacingMoneyText.AnimateValue (this, 0, dangerousOvertacing, .5f, "+", "", .4f);
		highSpeedMoneyText.AnimateValue (this, 0, highSpeedMoney, .5f, "+", "", .6f);
		drivingOppositeMoneyText.AnimateValue (this, 0, drivingOppositeMoney, .5f, "+", "", .8f);
		totalMoneyText.AnimateValue (this, 0, totalMoney, .5f, "+", "", 1f);

		CalculateXP ();

	}

	void CalculateXP ()
	{

		int xpAmount = Mathf.RoundToInt (ScoreController.Instance.score / 80);

		StartCoroutine (levelUPCoroutine (xpAmount));

//		print ("xpAmount" + xpAmount);

	}

	IEnumerator levelUPCoroutine (int xpAmount)
	{
		
		int startTotalXp = GameManager.TotalXP;
		int targetTotalXP = xpAmount + GameManager.TotalXP;
		int startLevel = GameManager.CurLevel;
		int curLevel = GameManager.CurLevel - 1;

		GameManager.AddXP (xpAmount);

		yield return new WaitForSeconds (.01f);

		do {

			curLevel++;

			UpdateLevelSliders (startTotalXp, targetTotalXP, curLevel, .01f);

			yield return new WaitForSeconds (1.6f);

		} while (targetTotalXP >= GameManager.NeedTotalXPforNextLevel (curLevel));			

	}

	void UpdateLevelSliders (int startXP, int targetXP, int level, float delay)
	{
		float fromValue = Mathf.Clamp (startXP - GameManager.NeedTotalXPforNextLevel (level - 1), 0, GameManager.NeedXPforNextLevel (level)) / (float)GameManager.NeedXPforNextLevel (level);
		float toValue = Mathf.Clamp (targetXP - GameManager.NeedTotalXPforNextLevel (level - 1), 0, GameManager.NeedXPforNextLevel (level)) / (float)GameManager.NeedXPforNextLevel (level);

		xpBarSlider.localScale = new Vector3 (fromValue, 1, 1);

		levelNum.text = level.ToString ();

		xpBarProgressSlider.AnimateSlider (this, fromValue, toValue, 1.5f, delay);

	}

	public void GiveOneChance ()
	{
		chanceGived = true;
		PlayerCarController.Instance.player.lifeCount++;
		PlayerCarController.Instance.player.Respawn ();
		KScreenManager.Instance.ShowScreen ("UI");
		//FindObjectOfType<PauseManager> ().Resume ();

	}

	[ContextMenu ("Retry")]
	public void Retry ()
	{
		ScreenFader.LoadSceneWithFade (Application.loadedLevelName);
	}

	[ContextMenu ("GoToGarage")]
	public void GoToGarage ()
	{
		ScreenFader.LoadSceneWithFade ("Shop");
	}

	public void GoToMenu ()
	{
		ScreenFader.LoadSceneWithFade ("Menu");
	}


}

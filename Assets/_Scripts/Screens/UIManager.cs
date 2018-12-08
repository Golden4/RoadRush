using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : SingletonGeneric<UIManager> {

	public Text speedText;
	public Text timeText;
	public Text distanceText;
	public Text addScoreText;
	public Gradient gradient;

	public Text scoreText;
	public Text BeforePauseCountDownText;
	GUIAnim addScoreAnim;
	GUIAnim RestartPlayerCountDownAnim;

	public Text highSpeedInfo;
	public Text drivingOppositeInfo;

	GUIAnim highSpeedInfoAnim;
	bool highSpeedInfoIsShowing = true;

	void Start ()
	{
		if (LevelInfo.gameMode == LevelInfo.GameMode.InTime) {
			timeText.gameObject.SetActive (true);
			StartCoroutine (timeCountDownCoroutine ());

		}
		else {
			timeText.gameObject.SetActive (false);
		}
	}


	public int curTime = 60;


	IEnumerator timeCountDownCoroutine ()
	{
		curTime = 60;
		PlayerCarController.Instance.player.lifeCount = 999999;
		while (curTime > 0) {
			timeText.text = "0:" + curTime.ToString ("D2");

			curTime--;

			yield return new WaitForSeconds (1);

			if (curTime == 0) {
				PlayerCarController.Instance.player.lifeCount = 1;
				EndGameController.Instance.chanceGived = true;
				PlayerCarController.Instance.player.Dead (false);


			}

		}
	}

	public void ShowHighSpeedInfo (float timeAmount)
	{
		if (highSpeedInfoAnim == null)
			highSpeedInfoAnim = highSpeedInfo.transform.parent.GetComponent<GUIAnim> ();

		//highSpeedInfo.transform.parent.gameObject.SetActive (true);
		if (!highSpeedInfoIsShowing) {
			highSpeedInfoAnim.MoveIn (GUIAnimSystem.eGUIMove.SelfAndChildren);
			highSpeedInfoIsShowing = true;
		}

		highSpeedInfo.text = timeAmount.ToString ("F1") + " sec.";
	}

	public void HideHighSpeedInfo ()
	{
		if (highSpeedInfoAnim == null)
			highSpeedInfoAnim = highSpeedInfo.transform.parent.GetComponent<GUIAnim> ();
		
		if (highSpeedInfoIsShowing) {
			highSpeedInfoAnim.MoveOut (GUIAnimSystem.eGUIMove.SelfAndChildren);
			highSpeedInfoIsShowing = false;
		}

		//highSpeedInfo.transform.parent.gameObject.SetActive (false);
	}

	GUIAnim drivingOppositeInfoAnim;
	bool drivingOppositeInfoIsShowing = true;

	public void ShowDrivingOppositeInfo (float timeAmount)
	{


		if (drivingOppositeInfoAnim == null)
			drivingOppositeInfoAnim = drivingOppositeInfo.transform.parent.GetComponent<GUIAnim> ();

		if (!drivingOppositeInfoIsShowing) {
			drivingOppositeInfoAnim.MoveIn (GUIAnimSystem.eGUIMove.SelfAndChildren);
			drivingOppositeInfoIsShowing = true;
		}


		//drivingOppositeInfo.transform.parent.gameObject.SetActive (true);

		drivingOppositeInfo.text = timeAmount.ToString ("F1") + " sec.";
	}

	public void HideDrivingOppositeInfo ()
	{
		if (drivingOppositeInfoAnim == null)
			drivingOppositeInfoAnim = drivingOppositeInfo.transform.parent.GetComponent<GUIAnim> ();

		if (drivingOppositeInfoIsShowing) {
			drivingOppositeInfoAnim.MoveOut (GUIAnimSystem.eGUIMove.SelfAndChildren);
			drivingOppositeInfoIsShowing = false;
		}


		//drivingOppositeInfo.transform.parent.gameObject.SetActive (false);
	}

	public void SpeedText (float value)
	{
		speedText.text = Mathf.RoundToInt (value) + " km/h";
	}

	public void DistanceText (float value)
	{
		distanceText.text = (value / 1000f).ToString ("F") + " km";
	}

	public void AddScoreText (float score, Vector3 pos)
	{
		addScoreText.gameObject.SetActive (true);
		addScoreText.text = "+" + score;
		addScoreText.transform.position = Camera.main.WorldToScreenPoint (pos);

		addScoreText.color = gradient.Evaluate (score / 1001f);

		if (addScoreAnim == null)
			addScoreAnim = addScoreText.GetComponent<GUIAnim> ();

		if (addScoreText.gameObject.activeSelf)
			addScoreAnim.MoveIn (GUIAnimSystem.eGUIMove.Self);

		if (IsInvoking ("DisableAddScoreText")) {
			CancelInvoke ("DisableAddScoreText");
		}

		Invoke ("DisableAddScoreText", 1);
	}

	void DisableAddScoreText ()
	{
		addScoreText.gameObject.SetActive (false);
	}

	public void ScoreText (float score, bool increase)
	{
		if (increase) {
			scoreText.color = Color.green;
		}
		else {
			scoreText.color = Color.white;
		}

		scoreText.text = Mathf.RoundToInt (score).ToString ();
	}

	public void BeforePauseCountDown (int value)
	{
		BeforePauseCountDownText.gameObject.SetActive (true);
		if (RestartPlayerCountDownAnim == null)
			RestartPlayerCountDownAnim = BeforePauseCountDownText.GetComponent<GUIAnim> ();

		RestartPlayerCountDownAnim.MoveIn (GUIAnimSystem.eGUIMove.Self);

		BeforePauseCountDownText.text = value.ToString ();

		RestartPlayerCountDownAnim.MoveOut (GUIAnimSystem.eGUIMove.Self);
	}

}

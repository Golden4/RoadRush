using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour {

	public bool isPauseScreen;

	public Text countDownText;

	public Sound countdownNum;
	public Sound countdownGO;

	bool focus;

	void Start ()
	{
		if (!Unfreezing) {
			KScreenManager.Instance.ShowScreen (-1);
			StartCoroutine ("UnfreezeTimeCorutine");
		}
	}

	/*public override void  ()
	{
		if (!Unfreezing) {
			KScreenManager.Instance.StartCoroutine (UnfreezeTimeColorutine ());
			KScreenManager.Instance.ShowScreen (-1);
		}
	}*/
	public static bool Unfreezing = false;

	IEnumerator UnfreezeTimeCorutine ()
	{
		Unfreezing = true;

		countDownText.gameObject.SetActive (true);

		countDownText.gameObject.GetComponentInChildren <Text> ().enabled = true;

		TimeManager.FreezeTime ();

		OnFreezeTime ();

		for (int i = 3; i >= 0; i--) {

			string text = i.ToString ();
			countDownText.color = Color.white;

			if (i == 0) {
				text = "GO!";
				countDownText.color = Color.green;

				AudioManager.Instance.PlaySound2D (countdownGO);

			}
			else {
				AudioManager.Instance.PlaySound2D (countdownNum);
			}

			countDownText.text = text;

			countDownText.GetComponent<GUIAnim> ().MoveIn ();

			if (i == 0)
				continue;

			yield return new WaitForSecondsRealtime (.8f);
		}

		OnUnfreezeTime ();

		TimeManager.UnfreezeTime ();

		yield return new WaitForSecondsRealtime (.7f);
		countDownText.gameObject.SetActive (false);
		Unfreezing = false;
	}

	void OnUnfreezeTime ()
	{
		print ("UnfreezeTime");

		//KScreenManager.EnableRaycaster (true);


		KScreenManager.Instance.ShowScreen ("UI");

		AudioManager.Instance.UnpauseSounds ();

		PlayerCarController.Instance.player.GetComponent<CarAudio> ().UnpauseSounds ();

	}

	void OnFreezeTime ()
	{
		//KScreenManager.EnableRaycaster (false);
		KScreenManager.Instance.ShowScreen ("UI");


	}

	void UnFreezeTime ()
	{
		if (!Unfreezing) {
			isPauseScreen = false;
			StartCoroutine ("UnfreezeTimeCorutine");
			//KScreenManager.Instance.ShowScreen (-1);
		}
	}

	public void Pause ()
	{
		if (!PlayerCarController.Instance.player.isDead) {
			isPauseScreen = true;

			if (Unfreezing) {
				StopCoroutine ("UnfreezeTimeCorutine");
				//countDownText.GetComponent<GUIAnim> ().Reset ();
				countDownText.gameObject.GetComponent<GUIAnim> ().MoveOut ();

				countDownText.gameObject.GetComponentInChildren <Text> ().enabled = false;
				Unfreezing = false;
			}

			AudioManager.Instance.PauseSounds ();
			if (PlayerCarController.Instance.player.GetComponent<CarAudio> () != null)
				PlayerCarController.Instance.player.GetComponent<CarAudio> ().PauseSounds ();

			KScreenManager.Instance.ShowScreen ("Pause");
			TimeManager.FreezeTime ();
		}
	}

	void OnApplicationFocus ()
	{
		focus = true;

		Pause ();
	}

	public void Resume ()
	{
		UnFreezeTime ();
	}

	/*public override void OnInit ()
	{
		SceneManager.sceneLoaded += OnFreezeTime;
	}

	public override void OnCleanUp ()
	{
		SceneManager.sceneLoaded -= OnFreezeTime;
	}*/


}

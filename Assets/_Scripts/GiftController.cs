using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GiftController : MonoBehaviour {

	public Button giftBtn;

	public Text timer;

	public DateTime nextGiveGiftTime;

	public int money = 10000;

	void Start ()
	{
		if (PlayerPrefs.HasKey ("giftTime"))
			nextGiveGiftTime = new DateTime (long.Parse (PlayerPrefs.GetString ("giftTime")));
		giftBtn.onClick.AddListener (GiveGift);


		if (CanTakeGift ()) {
			OnCanTakeGift ();
		} else {
			OnDontTakeGift ();
		}
	}

	void GiveGift ()
	{
		if (CanTakeGift ()) {

			nextGiveGiftTime = DateTime.Now.AddMinutes (1);

			PlayerPrefs.SetString ("giftTime", nextGiveGiftTime.Ticks.ToString ());

			GameManager.AddMoney (money);

		}
	}

	bool CanTakeGift ()
	{
		return nextGiveGiftTime.Ticks < DateTime.Now.Ticks;
	}

	bool lastOnTakeGiftBool;

	void Update ()
	{
		if ((nextGiveGiftTime - DateTime.Now).Ticks > 0) {
			
			if (!lastOnTakeGiftBool) {
				lastOnTakeGiftBool = true;
				OnDontTakeGift ();
			}

			TimeSpan ts = new DateTime ((nextGiveGiftTime - DateTime.Now).Ticks).TimeOfDay;

			timer.text = string.Format ("{0}", ts).Split ('.') [0];
		} else if (lastOnTakeGiftBool) {
			lastOnTakeGiftBool = false;
			OnCanTakeGift ();
		}
	}

	void OnCanTakeGift ()
	{
		print ("OnCanTakeGift");
		giftBtn.enabled = true;
		timer.gameObject.SetActive (false);
		giftBtn.GetComponent<Animation> ().Play ();
		giftBtn.GetComponent<Animation> ().wrapMode = WrapMode.Loop;
	}

	void OnDontTakeGift ()
	{
		print ("OnDontTakeGift");
		giftBtn.enabled = false;
		timer.gameObject.SetActive (true);
		giftBtn.GetComponent<Animation> ().wrapMode = WrapMode.Clamp;
	}
}

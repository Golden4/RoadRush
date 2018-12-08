using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Utility {

	public static void AnimateValue (this Text text, MonoBehaviour objToStartCorutine, float fromValue, float toValue, float time = 1, string startString = "", string endString = "", float delay = 0, bool round = true)
	{
		objToStartCorutine.StartCoroutine (AnimateValueCoroutine (text, fromValue, toValue, time, round, startString, endString, delay));
	}

	static IEnumerator AnimateValueCoroutine (Text text, float fromValue, float toValue, float time = 1, bool round = true, string startString = "", string endString = "", float delay = 0)
	{

		text.text = "";

		yield return new WaitForSeconds (delay);

		float t = 1;

		float value = fromValue;

		while (t > 0) {

			t -= Time.deltaTime / time;

			value = Mathf.Lerp (fromValue, toValue, 1f - t);

			string valueString = (round) ? Mathf.RoundToInt (value).ToString () : value.ToString ();

			text.text = startString + valueString + endString;

			yield return null;

		}

	}

	public static void AnimateSlider (this RectTransform slider, MonoBehaviour objToStartCorutine, float fromValue, float toValue, float time = 1, float delay = 0)
	{
		objToStartCorutine.StartCoroutine (AnimateSliderCoroutine (slider, fromValue, toValue, time, delay));
	}

	static IEnumerator AnimateSliderCoroutine (RectTransform slider, float fromValue, float toValue, float time = 1, float delay = 0)
	{

		slider.localScale = new Vector3 (fromValue, 1, 1);

		yield return new WaitForSeconds (delay);

		float t = 1;

		float value = fromValue;

		while (t > 0) {

			t -= Time.deltaTime / time;

			value = Mathf.Lerp (fromValue, toValue, 1f - t);

			slider.localScale = new Vector3 (value, 1, 1);

			yield return null;
		}

		slider.localScale = new Vector3 (toValue, 1, 1);

	}

	public static void SetActiveToFalseForTime (this GameObject go, MonoBehaviour objToStartCorutine, float timeToActiveFalse = 1)
	{
		objToStartCorutine.StartCoroutine (SetActiveFalseForTimeCoroutine (go, timeToActiveFalse));
	}

	static IEnumerator SetActiveFalseForTimeCoroutine (GameObject go, float timeToActiveFalse = 1)
	{
		yield return new WaitForSecondsRealtime (timeToActiveFalse);

		go.SetActive (false);

	}

}

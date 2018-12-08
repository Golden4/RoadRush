using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour {

	public string key;

	void Awake ()
	{
		LocalizationManager.OnLanguageChangeEvent += Start;
	}

	void Start ()
	{
		Text text = GetComponent<Text> ();

		if (!string.IsNullOrEmpty (key))
			text.text = LocalizationManager.GetLocalizedText (key);

		text.font = LocalizationManager.curLanguage.font;
		text.fontStyle = LocalizationManager.curLanguage.fontStyle;
//		text.GetComponent <Renderer> ().sharedMaterial = LocalizationManager.GetLocalizedFont ().material;
	}

	void OnDestroy ()
	{
		LocalizationManager.OnLanguageChangeEvent -= Start;
	}



}

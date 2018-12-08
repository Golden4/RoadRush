using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LocalizationManager : MonoBehaviour {

	static Dictionary<string,string> localizedText = new Dictionary<string, string> ();

	static bool isLoaded;

	public static event System.Action OnLanguageChangeEvent;

	static LanguageInfo _curLanguage;

	public static LanguageInfo curLanguage {
		get {
			if (!isLoaded) {
				_curLanguage = Application.systemLanguage;
			}

			return _curLanguage;
		}
		set {
			_curLanguage = value;
		}
	}

	public static string GetLocalizedText (string key)
	{
		if (!isLoaded) {

			curLanguage = Application.systemLanguage;

			LoadLocalizedTexts (GetFileName (curLanguage));

		}

		string valueText = "";

		if (!localizedText.TryGetValue (key, out valueText)) {
			Debug.LogError ("Localization key not found: " + key);
			valueText = "Error";
		}

		return valueText;

	}

	static string GetFileName (LanguageInfo language)
	{
		return "Languages/language_" + language.filePrefix + ".json";

	}

	static void LoadLocalizedTexts (string fileName)
	{
		string filePath = Path.Combine (Application.streamingAssetsPath, fileName);

		if (File.Exists (filePath)) {

			string jsonDataString = File.ReadAllText (filePath);

			LocalizationData loadedData = JsonUtility.FromJson<LocalizationData> (jsonDataString);

			localizedText.Clear ();

			for (int i = 0; i < loadedData.items.Length; i++) {
				localizedText [loadedData.items [i].key] = loadedData.items [i].value;
			}

			Debug.Log ("localizedText loaded: " + loadedData.items.Length);

		} else {
			Debug.LogError ("Localization file not founded");
		}
		isLoaded = true;

	}

	public static void ChangeLanguage (SystemLanguage language)
	{
		curLanguage = language;

		LoadLocalizedTexts (GetFileName (curLanguage));

		print (language.ToString ());

		if (OnLanguageChangeEvent != null) {
			OnLanguageChangeEvent ();
		}
	}

	[System.Serializable]
	public class LocalizationData {
		public LocalizationItem[] items;
	}

	[System.Serializable]
	public class LocalizationItem {
		public string key;
		public string value;
	}
}

public class LanguageInfo {
	public string name;
	public SystemLanguage systemLanguage;
	public string filePrefix;
	public Font font;
	public FontStyle fontStyle;

	public LanguageInfo (SystemLanguage lang, string filePrefix, string fontName, FontStyle style)
	{
		if (!string.IsNullOrEmpty (fontName))
			this.font = (Font)Resources.Load<Font> ("Fonts/" + fontName);

		fontStyle = style;
		Debug.Log (font);

		this.filePrefix = filePrefix;
		this.name = lang.ToString ();
		systemLanguage = lang;
	}

	public static implicit operator LanguageInfo (SystemLanguage info)
	{
		return LanguagesList.GetLanguageInfo (info);
	}

}

public class LanguagesList {

	public static LanguageInfo[] languages = {
		new LanguageInfo (SystemLanguage.English, "en", "Digivolve", FontStyle.Normal),
		new LanguageInfo (SystemLanguage.Russian, "ru", "Digivolve", FontStyle.Normal)
	};

	public static LanguageInfo GetLanguageInfo (SystemLanguage lang)
	{

		LanguageInfo langInfo = System.Array.Find (languages, x => x.systemLanguage == lang);

		if (langInfo != null)
			return langInfo;
		else
			return languages [0];
	}

	public static int GetLanguageIndex (SystemLanguage lang)
	{
		int index = System.Array.FindIndex (languages, x => x.systemLanguage == lang);

		if (index >= 0)
			return index;
		else
			return 0;
	}

	public static int Count ()
	{
		return languages.Length;
	}

}

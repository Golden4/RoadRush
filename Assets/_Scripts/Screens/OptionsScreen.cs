using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsScreen : MonoBehaviour {

	public Dropdown inputDropdown;
	public Dropdown languagesDropdown;
	public Toggle sfxToggle;
	public Toggle musicToggle;

	void Start ()
	{
		LocalizationManager.OnLanguageChangeEvent += UpdateInputDropdown;

		if (PlayerPrefs.HasKey ("Control"))
			inputDropdown.value = PlayerPrefs.GetInt ("Control");

		UpdateInputDropdown ();

		languagesDropdown.ClearOptions ();
		List<string> list = new List<string> ();

		for (int i = 0; i < LanguagesList.Count (); i++)
			list.Add (LanguagesList.languages [i].name);

		languagesDropdown.AddOptions (list);
		languagesDropdown.value = LanguagesList.GetLanguageIndex (LocalizationManager.curLanguage.systemLanguage);
		languagesDropdown.RefreshShownValue ();

		inputDropdown.onValueChanged.AddListener (ChangeInputControl);
		languagesDropdown.onValueChanged.AddListener (OnChangeLanguage);
		sfxToggle.onValueChanged.AddListener (toggleSfx);
		musicToggle.onValueChanged.AddListener (toggleMusic);
	}

	void UpdateInputDropdown ()
	{
		
		inputDropdown.options [0].text = LocalizationManager.GetLocalizedText ("gyro");
		inputDropdown.options [1].text = LocalizationManager.GetLocalizedText ("touch");
		inputDropdown.RefreshShownValue ();
	}

	public void OnChangeLanguage (int langNum)
	{

		SystemLanguage lang = SystemLanguage.English;

		switch (langNum) {
		case 1:
			lang = SystemLanguage.Russian;
			break;
		}

		LocalizationManager.ChangeLanguage (lang);
	}

	public void ChangeInputControl (int inputIndex)
	{
		PlayerPrefs.SetInt ("Control", inputIndex);
		PlayerPrefs.Save ();

		InputManager.ChangeInput (inputIndex);

	}

	public void toggleMusic (bool enable)
	{
		AudioManager.musicEnabled = enable;
	}

	public void toggleSfx (bool enable)
	{
		AudioManager.sfxEnabled = enable;
	}

	void OnDestroy ()
	{
		LocalizationManager.OnLanguageChangeEvent -= UpdateInputDropdown;
	}
}

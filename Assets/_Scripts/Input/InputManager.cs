using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour {

	public InputInfo[] inputPresets;
	public Transform inputHolder;
	public int curInputIndex = 0;
	public MobileInputBtn nitro;
	public Gradient nitroGradient;

	public static event System.Action OnChangeInputEvent;

	public static Vector2 input;

	static MobileInputBtn[] btns;
	static bool gyro = false;

	public GameObject inputGO;

	public static bool disableInput = false;

	public static InputManager Instance;

	void Awake ()
	{
		Instance = this;
	}

	void Start ()
	{

		if (PlayerPrefs.HasKey ("Control")) {
			curInputIndex = PlayerPrefs.GetInt ("Control");
		}

		Instance.InitInput (curInputIndex);

	}

	public static Vector2 MobileInput ()
	{
		Vector2 input = Vector2.zero;

		if (disableInput)
			return input;

		if (btns != null)
			for (int i = 0; i < btns.Length; i++) {
				if (btns [i].holdingBtn) {
					switch (btns [i].inputBtnType) {
					case MobileInputBtn.TypeOfInpuBtn.Gaz:
						if (input.y != -1)
							input.y = 1;
						break;
					case MobileInputBtn.TypeOfInpuBtn.Break:
						input.y = -1;
						break;
					case MobileInputBtn.TypeOfInpuBtn.Left:
						input.x = -1;
						break;
					case MobileInputBtn.TypeOfInpuBtn.Right:
						input.x = 1;
						break;

					default:
						break;
					}
				}
			}

		if (gyro) {
			input.x = Mathf.Clamp (Input.acceleration.x * 4f, -1, 1);
		}

		return input;

	}

	public static void ChangeInput (int index)
	{
		if (OnChangeInputEvent != null)
			OnChangeInputEvent ();
		
		if (Application.loadedLevelName == "1")
			Instance.InitInput (index);

	}

	void InitInput (int index)
	{
		if (inputGO != null)
			Destroy (inputGO);

		GameObject go = Instantiate (inputPresets [index].btsPrefab);
		go.transform.SetParent (inputHolder, false);
		inputGO = go;
		gyro = inputPresets [index].gyro;
		btns = go.GetComponentsInChildren<MobileInputBtn> ();
		nitroImage = nitro.GetComponentInChildren<Image> ();
	}

	Image nitroImage;

	public static bool nitroIsHolding (float nitroAmount)
	{
		if (disableInput)
			return false;
		
		Instance.nitroImage.color = Instance.nitroGradient.Evaluate (1f - nitroAmount);
		Instance.nitroImage.fillAmount = nitroAmount;
		return Instance.nitro.holdingBtn;
	}


}

[System.Serializable]
public class InputInfo {
	
	public bool gyro;
	public GameObject btsPrefab;

}

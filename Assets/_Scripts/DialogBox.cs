using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class DialogBox : MonoBehaviour {

	public Button okBtn;
	public Button cancelBtn;
	public Text textBox;
	public Text title;

	GUIAnim anim;

	public static DialogBox instance;

	static void Init ()
	{
		GameObject go = Instantiate (Resources.Load<GameObject> ("Prefabs/DialogBox"));
		go.transform.SetParent (FindObjectOfType<Canvas> ().transform, false);
		instance = go.GetComponent<DialogBox> ();
		instance.anim = instance.GetComponentInChildren<GUIAnim> ();
	}

	public static void Show (string title, string text, Action onClickOk, Action onClickCancel = null, bool showCancel = true)
	{
		if (instance == null)
			Init ();

		instance.gameObject.SetActive (true);

		instance.anim.MoveIn (GUIAnimSystem.eGUIMove.SelfAndChildren);

		instance.textBox.text = text;
		instance.title.text = title;

		instance.okBtn.onClick.RemoveAllListeners ();
		instance.cancelBtn.onClick.RemoveAllListeners ();

		instance.okBtn.onClick.AddListener (() => {
			if (onClickOk != null)
				onClickOk ();
			Hide ();
		});

		instance.cancelBtn.onClick.AddListener (() => {
			if (onClickCancel != null)
				onClickCancel ();
			Hide ();
		});

        instance.cancelBtn.gameObject.SetActive(showCancel);
        

	}
    
	static void Hide ()
	{
		instance.anim.MoveOut (GUIAnimSystem.eGUIMove.SelfAndChildren);
		instance.gameObject.SetActiveToFalseForTime (instance, .7f);
	}

}

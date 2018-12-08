using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KScreen : KMonoBehaviour {

	public string name;
	public bool isActive;

	public bool useGUIAnim;

	public void Activate ()
	{
		InitializeComponent ();

		gameObject.SetActive (true);

		if (useGUIAnim)
			GetComponentInChildren <GUIAnim> ().MoveIn (GUIAnimSystem.eGUIMove.SelfAndChildren);


		isActive = true;
	}

	public void Deactivate ()
	{
		InitializeComponent ();

		if (useGUIAnim)
			GetComponentInChildren <GUIAnim> ().MoveOut (GUIAnimSystem.eGUIMove.SelfAndChildren);

		gameObject.SetActive (false);
		isActive = false;
	}

	/*
	public override void OnSpawn ()
	{
		base.OnSpawn ();

		if (activateOnSpawn) {
			Activate ();
		}
		else {
			Deactivate ();
		}

	}*/

}

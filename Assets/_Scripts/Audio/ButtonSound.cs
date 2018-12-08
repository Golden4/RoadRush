using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonSound : MonoBehaviour, IPointerClickHandler {

	public void OnPointerClick (PointerEventData eventData)
	{
		AudioManager.Instance.PlaySound2D ("Click2");
	}
}

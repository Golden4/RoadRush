using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MobileInputBtn : MonoBehaviour, IPointerEnterHandler , IPointerExitHandler {

	public enum TypeOfInpuBtn
	{
		Gaz,
		Break,
		Left,
		Right,
		Nitro
	}

	public TypeOfInpuBtn inputBtnType;

	public bool holdingBtn = false;

	Image image;

	void Start ()
	{
		image = GetComponentInChildren<Image> ();
	}

	void Update ()
	{
		#if !UNITY_EDITOR
		if (holdingBtn && Input.touchCount == 0) {
			holdingBtn = false;
		}
		#endif
	}


	public void OnPointerEnter (PointerEventData eventData)
	{
		SetAlpha (.6f);
		holdingBtn = true;
	}

	public void OnPointerExit (PointerEventData eventData)
	{
		SetAlpha (1f);
		holdingBtn = false;
	}

	public void OnPointerDown (PointerEventData eventData)
	{

		SetAlpha (.6f);

		holdingBtn = true;
	}


	public void OnPointerUp (PointerEventData eventData)
	{
		SetAlpha (1f);

		holdingBtn = false;
	}

	void SetAlpha (float a)
	{
		if (image == null)
			return;
		
		Color color = image.color;
		color.a = a;
		image.color = color;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraShopController : MonoBehaviour {
	
	public bool rotateWithMouse;
	public float distance = 10;
	float x;
	float y;
	Vector3 target = new Vector3 (0, .3f, 0);

	void Start ()
	{

		#if UNITY_EDITOR
		rotateWithMouse = true;
		#endif

		x = transform.eulerAngles.y;
		y = transform.eulerAngles.x;
	}

	bool dragging = false;
	Vector3 velocity;
	float velocityForceX;
	float velocityForceY;
	float dir;

	void Update ()
	{
		RotateCameraAround ();
	}

	bool clicked;
	float multiplyX;
	float multiplyY;

	void RotateCameraAround ()
	{
		Quaternion rotation = Quaternion.Euler (y, x, 0);

		Vector3 position = rotation * new Vector3 (0, 0, -distance) + target;

		if (!rotateWithMouse) {
			clicked = Input.touchCount > 0;
			if (clicked) {
				multiplyX = Input.touches [0].deltaPosition.x / Screen.width * 20;
				multiplyY = -Input.touches [0].deltaPosition.y / Screen.height * 20;
			}
		}
		else {
			clicked = Input.GetMouseButton (0);
			multiplyX = Input.GetAxis ("Mouse X") * .35f;
			multiplyY = -Input.GetAxis ("Mouse Y") * .5f;
		}


		if (clicked && KScreenManager.Instance.curScreen == "Shop") {
			velocityForceX += multiplyX;
			velocityForceY += multiplyY;

			dragging = true;
		}
		else {
			dragging = false;
		}


		velocityForceX = Mathf.Clamp (velocityForceX, -4f, 4f);
		velocityForceY = Mathf.Clamp (velocityForceY, -4f, 4f);

		float dirX = (velocityForceX > 0) ? 1 : -1;

		if (velocityForceX * dirX > .1f * -dirX) {
			
			velocityForceX -= Time.deltaTime * dirX * ((!dragging) ? 4 : 1);

		}
		else if (!dragging) {
			
			velocityForceX = 0;

		}

		float dirY = (velocityForceX > 0) ? 1 : -1;

		if (velocityForceY * dirY > .1f * -dirY) {

			velocityForceY -= Time.deltaTime * dirY * ((!dragging) ? 1 : .5f);

		}
		else if (!dragging) {
			
			velocityForceY = 0;

		}

		x += velocityForceX;
		y += velocityForceY;

		if (y < 10 || y > 50) {
			velocityForceY = 0;
		}

		y = Mathf.Clamp (y, 9f, 60f);



		transform.rotation = rotation;
		transform.position = position;
	}



}

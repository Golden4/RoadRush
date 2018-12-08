using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {

	public bool debugMode = false;

	float shakeAmount;
	float shakeDuration;

	float shakePercentage;
	public  float startAmount;
	public float startDuration;

	bool isRunning = false;

	public bool smooth;
	public float smoothAmount = 5f;

	void Awake ()
	{
		if (debugMode)
			ShakeCamera ();

		EventManager.OnPlayerDeathEvent += ShakeCamera;

	}

	void ShakeCamera ()
	{
		shakeAmount = startAmount;//Set default (start) values
		shakeDuration = startDuration;//Set default (start) values

		if (!isRunning)
			StartCoroutine (Shake ());//Only call the coroutine if it isn't currently running. Otherwise, just set the variables.
	}

	public void ShakeCamera (float amount = 2, float duration = 1)
	{
		shakeAmount += amount;//Add to the current amount.
		startAmount = shakeAmount;//Reset the start amount, to determine percentage.
		shakeDuration += duration;//Add to the current time.
		startDuration = shakeDuration;//Reset the start time.

		if (!isRunning)
			StartCoroutine (Shake ());//Only call the coroutine if it isn't currently running. Otherwise, just set the variables.
	}

	IEnumerator Shake ()
	{
		isRunning = true;

		Quaternion startRot = transform.localRotation;

		while (shakeDuration > 0.01f) {
			Vector3 rotationAmount = Random.insideUnitSphere * shakeAmount + startRot.eulerAngles;//A Vector3 to add to the Local Rotation
			rotationAmount.z = 0;//Don't change the Z; it looks funny.

			shakePercentage = shakeDuration / startDuration;//Used to set the amount of shake (% * startAmount).

			shakeAmount = startAmount * shakePercentage;//Set the amount of shake (% * startAmount).
			shakeDuration -= Time.deltaTime;//Lerp the time, so it is less and tapers off towards the end.


			if (smooth)
				transform.localRotation = Quaternion.Lerp (transform.localRotation, Quaternion.Euler (rotationAmount), Time.deltaTime * smoothAmount);
			else
				transform.localRotation = Quaternion.Euler (rotationAmount);//Set the local rotation the be the rotation amount.

			yield return null;
		}
		transform.localRotation = startRot;//Set the local rotation to 0 when done, just to get rid of any fudging stuff.
		isRunning = false;
	}

	void OnDestroy ()
	{
		EventManager.OnPlayerDeathEvent -= ShakeCamera;
	}
	
}

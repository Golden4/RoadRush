using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class CameraController : SingletonGeneric<CameraController> {

	public PlayerCar target;
	public Vector3 centerRoadPos;

	public CameraPosRot[] cameraPositions;

	bool _curCameraPosIndexLoaded = false;
	int _curCameraPosIndex = 0;
    
	public int CurCameraPosIndex {
		get {
			if (!_curCameraPosIndexLoaded) {
				_curCameraPosIndexLoaded = true;

				if (PlayerPrefs.HasKey ("cameraPos")) {
					_curCameraPosIndex = PlayerPrefs.GetInt ("cameraPos");
				}
			}

			return _curCameraPosIndex;
		}

		set {

			if (!_curCameraPosIndexLoaded) {
				_curCameraPosIndexLoaded = true;

				if (PlayerPrefs.HasKey ("cameraPos")) {
					_curCameraPosIndex = PlayerPrefs.GetInt ("cameraPos");
				}
			}

			PlayerPrefs.SetInt ("cameraPos", value);

			_curCameraPosIndex = value;
		}
	}

	public Vector3 offset;

	void Start ()
	{
        target = PlayerCar.Ins;

		ChangeCamera (CurCameraPosIndex);

		UpdateRoadCenter ();
        
	}

	/*	void Update ()
	{
		print (Camera.main.WorldToScreenPoint (sasaa.position));

		Vector3 posInScreen = Camera.main.WorldToScreenPoint (sasaa.position);
		posInScreen.x = Screen.width - 100;
		posInScreen.y = (posInScreen.y > Screen.height) ? Screen.height : posInScreen.y;

		pointer.transform.position = posInScreen;

		pointer.text = Mathf.RoundToInt (-Camera.main.ScreenToWorldPoint (pointer.transform.position.x * Vector3.left).x + sasaa.position.x) + "  =>";

	}*/
	Vector3 velocity;
	float nitroSpeed;

	public void UpdateCameraPos ()
	{

		if (TimeManager.freezeTime)
			return;

		float targetSpeed = (target.usingNitro) ? 2.5f : 1;

		nitroSpeed = Mathf.Lerp (nitroSpeed, targetSpeed, Time.fixedDeltaTime * 2);
        
        if (cameraPositions[CurCameraPosIndex].clampToTarget)
        {
            transform.position = target.transform.position + offset;
        } else if (!target.isDead && !target.startingCountDown) {
			transform.position = Vector3.SmoothDamp (transform.position, target.transform.position.x * Vector3.right + offset, ref velocity, Time.fixedDeltaTime * nitroSpeed);
		} else {
			transform.position = Vector3.MoveTowards (transform.position, transform.position + Vector3.right * 50, Time.fixedDeltaTime * ((target.isDead) ? 14 : 6));
		}

		UpdateRoadCenter ();

	}

	void ChangeCamera (int index)
	{
		Camera.main.fieldOfView = cameraPositions [index].fieldOfView;
        offset = cameraPositions[index].pos;

        transform.position = target.transform.position.x * Vector3.right + cameraPositions[index].pos;

        transform.eulerAngles = cameraPositions[index].rot;

    }

	public void ChangeCameraNext ()
	{
		CurCameraPosIndex = (CurCameraPosIndex + 1) % cameraPositions.Length;
		ChangeCamera (CurCameraPosIndex);
	}

	void UpdateRoadCenter ()
	{
		centerRoadPos = transform.position - offset + Vector3.up * .56f;
	}

	public static void BlurEnable ()
	{
		Instance.GetComponent<BlurOptimized> ().enabled = true;
	}

	public static void BlurDisable ()
	{
		Instance.GetComponent<BlurOptimized> ().enabled = false;
	}

	void Awake ()
	{
		EventManager.OnPlayerRespawnedEvent += BlurDisable;
	}

	void OnDestroy ()
	{
		EventManager.OnPlayerRespawnedEvent -= BlurDisable;
	}

	[System.Serializable]
	public class CameraPosRot {
		public Vector3 pos;
		public Vector3 rot;
		public int fieldOfView = 40;
        public bool clampToTarget;
	}
}

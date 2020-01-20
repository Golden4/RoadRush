using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(BoxCollider))]
[RequireComponent (typeof(Rigidbody))]

public class PlayerCar : KMonoBehaviour {
    public static PlayerCar Ins;
	[Header ("Characteristics")]
	public float acceleration = 3;
	public float nitroAcceleration = .01f;
	public float nitroTime = 4;
	public float handling = 3;
	public float breaking = 5;
	public float deadSpeed = 100;
	public Gear[] gears;

	public float maxSpeed {
		get {
			return gears [gears.Length - 1].speedForSwitch;
		}
		set {
			gears [gears.Length - 1].speedForSwitch = value;
		}
	}

	[Space (25)]
    public float breakingIdle = .01f;
    [System.NonSerialized]
    public float minSpeed = 30;
    [System.NonSerialized]
    public bool isDead = false;

	public float switchingGearTime = .5f;
    [System.NonSerialized]
    public float highSpeedForBonus = 100;
    [System.NonSerialized]
	public int lifeCount = 1;
    [System.NonSerialized]
    public bool undying = false;

	float nitroAmount = 1;
    [System.NonSerialized]
    public bool usingNitro = false;
    [System.NonSerialized]
    public float speed = 5;
    [System.NonSerialized]
    public int curGear = 0;
    [System.NonSerialized]
    public bool startingCountDown = true;

	public Vector2 input{ get; private set; }

	[Header ("WheelsPos")]
	public Vector3[] wheelsPos;

	public Vector3[] nitroPos;


	Rigidbody rb;
	BoxCollider boxCol;

	Material[] materials;

    
    public override void OnSpawn ()
	{
		foreach (Light item in GetComponentsInChildren<Light> ()) {
			Destroy (item.gameObject);
		} 

		InitAddScoreTrigger ();


		rb = GetComponent<Rigidbody> ();
		materials = GetComponent<Renderer> ().materials;
        
		SetDefaults ();
		CreateVisualMeshObject ();
		InitSkidSmoke ();
		InitNitro ();
		InitGearShiftEffect ();
		UpdateCarStats ();
        Breaking();
        SkidMarks();
        UpdateInput();
        CalcSpeed();
        AnimateCar();
        UpdateScore();
        Nitro();

        startingCountDown = true;

	}

	Transform mesh;

	void CreateVisualMeshObject ()
	{
		GameObject meshGO = new GameObject ("Mesh");
		meshGO.transform.SetParent (transform);
		meshGO.transform.localPosition = Vector3.zero;
		meshGO.transform.localScale = Vector3.one;

		MeshFilter mf = meshGO.gameObject.AddComponent<MeshFilter> ();
		MeshRenderer mr = meshGO.gameObject.AddComponent<MeshRenderer> ();

		mf.mesh = GetComponent<MeshFilter> ().mesh;
		mr.materials = GetComponent<MeshRenderer> ().materials;

		Destroy (GetComponent<MeshFilter> ());
		Destroy (GetComponent<MeshRenderer> ());

		mesh = meshGO.transform;
	}

	void InitSkidSmoke ()
	{
		skidMark = FindObjectOfType<SkidMark> ();

		ParticleSystem ps = Resources.Load<ParticleSystem> ("Prefabs/SkidSmoke");

		for (int i = 0; i < skidSmokes.Length; i++) {
			skidSmokes [i] = Instantiate (ps, wheelsPos [i], Quaternion.Euler (0, -90, 0));
			skidSmokes [i].transform.SetParent (mesh.transform, false);
		}
	}

	void InitAddScoreTrigger ()
	{
		boxCol = GetComponent<BoxCollider> ();

		BoxCollider bc = gameObject.AddComponent<BoxCollider> ();
		bc.isTrigger = true;
		bc.size = new Vector3 (1, .5f, 3.5f);
		bc.center = boxCol.center.y * Vector3.up + Vector3.left;
	}

	ParticleSystem[] nitroEffects = new ParticleSystem[2];

	void InitNitro ()
	{
		ParticleSystem ps = Resources.Load<ParticleSystem> ("Prefabs/NitroEffect");

		for (int i = 0; i < nitroEffects.Length; i++) {
			nitroEffects [i] = Instantiate (ps, nitroPos [i], Quaternion.Euler (0, -90, 0));
			nitroEffects [i].transform.SetParent (mesh.transform, false);
		}

	}

	ParticleSystem[] gearShiftEffects = new ParticleSystem[2];

	void InitGearShiftEffect ()
	{
		ParticleSystem ps = Resources.Load<ParticleSystem> ("Prefabs/GearShiftEffect");

		for (int i = 0; i < gearShiftEffects.Length; i++) {
			gearShiftEffects [i] = Instantiate (ps, nitroPos [i], Quaternion.Euler (0, -90, 0));
			gearShiftEffects [i].transform.SetParent (mesh.transform, false);
		}

	}

	Vector3 oldPos;

	void Update ()
	{
		if (isDead || TimeManager.freezeTime || startingCountDown)
			return;
		
		Breaking ();
		SkidMarks ();
		UpdateInput ();
		AnimateCar ();
		UpdateScore ();
		Nitro ();

	}

	void FixedUpdate ()
	{
		if (isDead || TimeManager.freezeTime)
			return;

		if (!isDead) {
            CalcSpeed();

            ScoreController.Instance.distance += speed * Time.fixedDeltaTime / 10;
			oldPos = transform.position;

			if (ScoreController.Instance.averageSpeed < minSpeed)
				ScoreController.Instance.averageSpeed = minSpeed;

			totalSpeed += speed / 100;
			totalSpeedCount++;

			ScoreController.Instance.averageSpeed = totalSpeed / totalSpeedCount * 100;

		}

		Move ();
	}


	float lerpedX = 0;
	Vector2 targetInput;

	void UpdateInput ()
	{
		#if UNITY_EDITOR
		targetInput = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		if (targetInput == Vector2.zero)
		#endif
			
			targetInput = InputManager.MobileInput ();
		
		lerpedX = Mathf.Lerp (lerpedX, targetInput.x, Time.deltaTime * 5);
		input = new Vector2 (lerpedX, targetInput.y);

		
		if (usingNitro)
			input = new Vector2 (input.x, 1);

		if (speed == minSpeed) {
			input = new Vector2 (input.x, Mathf.Clamp (input.y, 0f, 1f));
		}

	}

	void UpdateCarStats ()
	{

		maxSpeed += Database.PlayerData.playerStats [Database.PlayerData.curCarIndex].boostLvlMaxSpeed * ShopController.upgradePercent * maxSpeed;
		acceleration += Database.PlayerData.playerStats [Database.PlayerData.curCarIndex].boostLvlAcceleration * ShopController.upgradePercent * acceleration;
		handling += Database.PlayerData.playerStats [Database.PlayerData.curCarIndex].boostLvlHandling * ShopController.upgradePercent * handling;
		breaking += Database.PlayerData.playerStats [Database.PlayerData.curCarIndex].boostLvlBreaking * ShopController.upgradePercent * breaking;
		nitroAcceleration += Database.PlayerData.playerStats [Database.PlayerData.curCarIndex].boostLvlNitro * ShopController.upgradePercent * nitroAcceleration;
		nitroTime += Database.PlayerData.playerStats [Database.PlayerData.curCarIndex].boostLvlNitro * ShopController.upgradePercent * nitroTime;
		deadSpeed += Database.PlayerData.playerStats [Database.PlayerData.curCarIndex].boostLvlDurability * ShopController.upgradePercent * deadSpeed;

	}


	void UpdateScore ()
	{
		if (speed > highSpeedForBonus) {
			ScoreController.Instance.score += Time.deltaTime * speed / 10;
			UIManager.Instance.ScoreText (ScoreController.Instance.score, true);
		} else {
			UIManager.Instance.ScoreText (ScoreController.Instance.score, false);
		}


		UIManager.Instance.SpeedText (speed);
		UIManager.Instance.DistanceText (ScoreController.Instance.distance);
	}

	float totalSpeed;
	int totalSpeedCount = 0;



    void Move()
    {

        if (startingCountDown) {

            InputManager.disableInput = true;

            MoveToTargetPos(CameraController.Instance.centerRoadPos);

            //return;

        } else {
            InputManager.disableInput = false;
        }

        /*if (rb.velocity != Vector3.zero) {
			rb.velocity = Vector3.zero;
		}*/

        if (rb.velocity != Vector3.zero)
            rb.velocity = Vector3.zero;

        Vector3 pos = rb.position;
        pos.x += speed / 4.5f * Time.fixedDeltaTime;

        pos.z -= handling * Time.fixedDeltaTime * input.x;

        pos.z = Mathf.Clamp(pos.z, -5, 5);
        
        rb.MovePosition (pos);


	}

	void MoveToTargetPos (Vector3 targetPos)
	{
		if (Mathf.Abs (rb.position.x - targetPos.x) < .1f && Time.timeSinceLevelLoad > .5f) {
			startingCountDown = false;
		}

		rb.position = Vector3.MoveTowards (rb.position, targetPos, Time.fixedDeltaTime * 10);

	}

	float _switchingGearLastTime;
	bool switchingGear = false;

	void CalcSpeed ()
	{
		
		float switchGearTime = ((usingNitro) ? switchingGearTime / 2 : switchingGearTime);

		if (_switchingGearLastTime + switchGearTime < Time.time && switchingGear)
			switchingGear = false;

		if (speed > gears [curGear].speedForSwitch && curGear < gears.Length - 1) {
			switchingGear = true;
			_switchingGearLastTime = Time.time;
			curGear++;
			SwitchGearUp ();

		} else {
			if (curGear > 0) {
				if (speed <= gears [curGear - 1].speedForSwitch) {
					curGear--;
				}
			}
		}

		if (!switchingGear) {
            if (input.y > 0)
                speed += (acceleration + gears[curGear].offsetAcceleration + ((usingNitro) ? nitroAcceleration : 0)) * Time.fixedDeltaTime;
            else if (input.y < 0)
            {
                speed -= breaking * Time.fixedDeltaTime;
            }
            else
            {
                speed -= breakingIdle * Time.fixedDeltaTime;
            }
		}

		speed = Mathf.Clamp (speed, minSpeed, gears [gears.Length - 1].speedForSwitch);



		if (ScoreController.Instance != null) {
			ScoreController.Instance.CalculateHighSpeedInfo (speed, highSpeedForBonus);
			ScoreController.Instance.CalculateDrivingOppositeInfo (transform.position.z);
		}
	}


	void SetDefaults ()
	{
		speed = minSpeed;
		curGear = 0;
		isDead = false;
		input = Vector2.zero;
		usingNitro = false;
		canUseNitro = true;
		switchingGear = false;
        rb.velocity = Vector3.zero;
	}

	public void Respawn ()
	{
		SetDefaults ();
		transform.position = CameraController.Instance.centerRoadPos;

		EventManager.OnPlayerRespawnedCall ();

		StartCoroutine ("UnDyingColorutine");
	}

	IEnumerator UnDyingColorutine ()
	{

		Shader origShader = materials [0].shader;

		Shader shader = Resources.Load ("Shaders/TransperentShader") as Shader;

		SetShader (shader);

		undying = true;
		boxCol.enabled = false;

		float t = 3;

		while (t > 0) {

			t -= Time.deltaTime;

			SetAlphaForMaterials (Mathf.PingPong (t / 2f, .2f) + .1f);

			yield return null;

		}

		SetAlphaForMaterials (1);

		SetShader (origShader);

		undying = false;
		boxCol.enabled = true;

	}

	void SetAlphaForMaterials (float a)
	{

		for (int i = 0; i < materials.Length; i++) {
			
			Color col = materials [i].color;

			col.a = a;

			materials [i].color = col;
		}
	}

	void SetShader (Shader shader)
	{

		for (int i = 0; i < materials.Length; i++) {
			materials [i].shader = shader;
		}
	}


	void SwitchGearUp ()
	{
		if (usingNitro)
			return;
		
		for (int i = 0; i < gearShiftEffects.Length; i++) {
			gearShiftEffects [i].Play ();
		}


	}

	bool canUseNitro;
	float lastUsingNitroTime = -1;

	void Nitro ()
	{

		#if UNITY_EDITOR
		//usingNitro = Input.GetKey (KeyCode.Space);
		#endif

		bool nitroInput = InputManager.nitroIsHolding (nitroAmount);

		if (nitroAmount <= 0 && nitroInput) {
			canUseNitro = false;
			usingNitro = false;
		} else if (!nitroInput && nitroAmount >= 0) {
			canUseNitro = true;
		}

		if (nitroInput && canUseNitro) {
			if (!usingNitro && lastUsingNitroTime + .5f < Time.time) {
				lastUsingNitroTime = Time.time;
				AudioManager.Instance.PlaySound ("Nitro", transform.position);
				usingNitro = true;
			}
		} else {
			usingNitro = false;
		}

		if (usingNitro) {

			nitroAmount -= Time.deltaTime / nitroTime;

			for (int i = 0; i < nitroEffects.Length; i++) {
				if (!nitroEffects [i].isPlaying) {
					nitroEffects [i].Play ();
					nitroEffects [i].transform.GetChild (0).gameObject.SetActive (true);
				}
			}
		} else {

			nitroAmount += Time.deltaTime / 30;

			for (int i = 0; i < nitroEffects.Length; i++) {
				if (nitroEffects [i].isPlaying) {
					nitroEffects [i].Stop ();
					nitroEffects [i].transform.GetChild (0).gameObject.SetActive (false);
				}
			}
		}

		nitroAmount = Mathf.Clamp01 (nitroAmount);

	}

	void OnCollisionEnter (Collision col)
	{
		if (col.transform.CompareTag ("Mob") && !isDead) {

			if (MobCarsController.Instance.roadLineInfo [0].dir == MoveDir.back && transform.position.z > -.1f) {
				Dead ();

			} else {
				
				if (speed >= deadSpeed) {
					Dead ();
				} else {
					AudioManager.Instance.PlaySound ("Crash Auto", transform.position);
				}
			}
		}
	}

	public void Dead (bool audio = true)
	{
		if (audio)
			AudioManager.Instance.PlaySound ("Crash Auto", transform.position);

		isDead = true;

		usingNitro = false;

		EventManager.OnPlayerDeathCall ();

		lifeCount--;
		if (IsInvoking ("Respawn"))
			CancelInvoke ("Respawn");
		
		if (lifeCount > 0) {
			Invoke ("Respawn", 2);
		} else {
			KScreenManager.Instance.ShowOneMoreChanceScreen ();
		}


	}

	float yAngle;

	void AnimateCar ()
	{
		Vector3 angles = Vector3.zero;
		float targetInput = (!switchingGear) ? input.y : 0;
		yAngle = Mathf.Lerp (yAngle, targetInput, Time.deltaTime * 7);

		angles.z += 4f * yAngle;
        
        if ((rb.position.z > 4.9 && input.x < 0) || (rb.position.z < -4.9 && input.x > 0))
        {
            input = new Vector2(0, input.y);
        }

        angles += Vector3.up * 3 * input.x + Vector3.right * 2 * input.x;

        mesh.transform.eulerAngles = angles;

	}

	Transform lastCar;

	void OnTriggerExit (Collider col)
	{
		if (col.transform.CompareTag ("Mob") && lastCar != col.transform && speed >= highSpeedForBonus) {
			lastCar = col.transform;
			Vector3 pos = ((col.transform.position - transform.position).z * Vector3.forward).normalized + transform.position;
			AddScoreText (pos);
			AudioManager.Instance.PlaySound ("Swish", pos);
		}
	}

	float lastTime;
	float addScore = 100;

	void AddScoreText (Vector3 pos)
	{
		if (isDead || undying)
			return;
		
		if (lastTime + 2 > Time.time) {
			addScore += 100;
		} else {
			addScore = 100;
		}

		ScoreController.Instance.score += addScore;

		lastTime = Time.time;
		ScoreController.Instance.dangerousOvertacing++;
		UIManager.Instance.AddScoreText (addScore, pos);
	}

	SkidMark skidMark;
	int[] lastPos = new int[2]{ -1, -1 };
	ParticleSystem[] skidSmokes = new ParticleSystem[2];
	float lastRotateTime = -1;
	int lastInput;

	void SkidMarks ()
	{

		bool needSkidMarks = true;

		if ((input.x > .9f || input.x < -.9f) && speed > 80) {

			/*if(lastInput)
			lastRotateTime = Time.time;*/
			int index = (input.x > .9f) ? 0 : 1;

			lastPos [index] = skidMark.AddSkidMark (transform.position + wheelsPos [index], Vector3.up, 1, lastPos [index]);
			

		} else if (input.y < 0) {
			for (int i = 0; i < wheelsPos.Length; i++) {
				lastPos [i] = skidMark.AddSkidMark (transform.position + wheelsPos [i] /*+ (speed / 4.5f * Time.deltaTime * Vector3.right)*/, Vector3.up, 1, lastPos [i]);
				if (!skidSmokes [i].isPlaying && speed < 70 && Random.Range (0, 2) == 0)
					skidSmokes [i].Play ();
			}

		} else
			needSkidMarks = false;


		if (!needSkidMarks) {
			for (int i = 0; i < wheelsPos.Length; i++) {
				lastPos [i] = -1;
				if (skidSmokes [i].isPlaying)
					skidSmokes [i].Stop ();
			}
		}

	}


	bool breakingMatChanged = false;

	void Breaking ()
	{
		if (input.y < 0) {
			if (!breakingMatChanged) {
				breakingMatChanged = true;

				ChangeBreakingMaterial (true);
			}

		} else if (breakingMatChanged) {
			breakingMatChanged = false;
			ChangeBreakingMaterial (false);
		}

	}

	Color origColor = Color.clear;

	void ChangeBreakingMaterial (bool isRedCol)
	{
		for (int i = 2; i < materials.Length; i++) {
			if (materials [i].name == "BreakLight (Instance)") {

				if (origColor == Color.clear)
					origColor = materials [i].color;

				materials [i].color = (isRedCol) ? Color.red : origColor;

			}
		}
	}

    public override void OnInit()
    {
        base.OnInit();

        Ins = this;
    }

    //	[Header ("Jump")]
    //	public float targetJumpingDistance;
    //	public float flyingTime = 1;
    //	public float jumpPower = 3;
    //
    //
    //	public float distanceBetweenRoad = 3f;
    //
    //	public enum PlayerState
    //	{
    //		Moving,
    //		Jumping,
    //		Switching
    //	}
    //
    //	public enum PlayerPos
    //	{
    //		Bottom,
    //		Center,
    //		Top
    //	}
    //
    //	public PlayerState curState = PlayerState.Moving;
    //	public PlayerPos curPlayerPos;
    //
    //	Vector3 startJumpingPos;
    //
    //
    //	void Start ()
    //	{
    //		startJumpingPos = transform.position;
    //	}
    //
    //	float t = 0;
    //
    //	void Update ()
    //	{
    //		if (Input.GetKeyDown (KeyCode.Space) && curState != PlayerState.Jumping) {
    //			SetJumpingState ();
    //		}
    //
    //	}
    //
    //	void FixedUpdate ()
    //	{
    //
    //		Move ();
    //		if (curState == PlayerState.Jumping && !movingUpDown) {
    //			Jump ();
    //			return;
    //		}
    //
    //		int curStateIndex = (int)curPlayerPos;
    //
    //		if (Input.GetKeyDown (KeyCode.S)) {
    //
    //			if (curStateIndex-- > 0 /*&& curState != PlayerState.Switching*/) {
    //				SetPosPlayer (curStateIndex--);
    //				print ("MoveUp");
    //			}
    //		} else if (Input.GetKeyDown (KeyCode.W)) {
    //
    //			if (System.Enum.GetNames (typeof(PlayerPos)).Length - 1 > curStateIndex++ /*&& curState != PlayerState.Switching*/) {
    //				SetPosPlayer (curStateIndex++);
    //				print ("MoveDown");
    //			}
    //		}
    //	}
    //
    //
    //
    //	void Jump ()
    //	{
    //		t += Time.fixedDeltaTime * movingSpeed / targetJumpingDistance;
    //
    //		if (t < 1) {
    //
    //			Vector3 lerpVector = transform.position;
    //
    //			//lerpVector.x = Mathf.Lerp (startJumpingPos.x, startJumpingPos.x + targetJumpingDistance, t);
    //
    //			lerpVector.y = startJumpingPos.y + Mathf.Sin (Mathf.PI * t) * jumpPower;
    //
    //			transform.position = lerpVector;
    //		} else {
    //			transform.position = startJumpingPos + Vector3.right * targetJumpingDistance;
    //			SetMovingState ();
    //		}
    //	}
    //

    //
    //	void SetJumpingState ()
    //	{
    //		curState = PlayerState.Jumping;
    //		t = 0;
    //		startJumpingPos.x = transform.position.x;
    //		startJumpingPos.z = transform.position.z;
    //	}
    //
    //	void SetJumpingState (float targetJumpPosOffset, float flyingTime, float jumpPower)
    //	{
    //		curState = PlayerState.Jumping;
    //		t = 0;
    //		this.flyingTime = flyingTime;
    //		this.targetJumpingDistance = targetJumpPosOffset;
    //		this.jumpPower = jumpPower;
    //		startJumpingPos.x = transform.position.x;
    //		startJumpingPos.z = transform.position.z;
    //	}
    //
    //	void SetMovingState ()
    //	{
    //		curState = PlayerState.Moving;
    //	}
    //
    //
    //
    //	void SetPosPlayer (int indexOfEnum)
    //	{
    //		curState = PlayerState.Switching;
    //
    //		StartCoroutine (MoveUpDownCoroutine ((int)curPlayerPos, (int)indexOfEnum));
    //
    //		curPlayerPos = (PlayerPos)indexOfEnum;
    //
    //
    //	}
    //
    //	bool movingUpDown = false;
    //
    //	IEnumerator MoveUpDownCoroutine (int startRoadIndex, int endRoadIndex)
    //	{
    //		movingUpDown = true;
    //		float f = 0;
    //
    //		while (f < 1) {
    //
    //			f += Time.fixedDeltaTime / movingUpDownTime;
    //
    //			float posZ = Mathf.Lerp ((startRoadIndex - 1) * distanceBetweenRoad, (endRoadIndex - 1) * distanceBetweenRoad, f);
    //
    //			Vector3 pos = transform.position;
    //			pos.z = posZ;
    //
    //			transform.position = pos;
    //
    //			yield return new WaitForFixedUpdate ();
    //		}
    //		movingUpDown = false;
    //		curState = PlayerState.Moving;
    //	}
}

[System.Serializable]
public class Gear {
	public float speedForSwitch;
	public float offsetAcceleration = .1f;
}
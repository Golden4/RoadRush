using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnviromentController : MonoBehaviour {
	public static EnviromentController Instance;

	public EnviromentInfo[] enviromentPrefabs;
	public float offsetBetweenObjects = 72f;
	GameObject[] enviromentList;
	public int enviromentCount = 5;
	int totalSpawnedEnv = 0;

	GameObject envHolder;

	void Awake ()
	{
		Instance = this;
	}

	void Start ()
	{
		envHolder = new GameObject ("Enviroments");
		SpawnEnviromentOnce ();
	}

	void FixedUpdate ()
	{
		if (CameraController.Instance.centerRoadPos.x + 70 > (totalSpawnedEnv - 1) * offsetBetweenObjects) {
			MoveEnviroment ();
		}
	}

	void MoveEnviroment ()
	{
		GameObject env = enviromentList [totalSpawnedEnv % enviromentCount];
		Vector3 pos = env.transform.position;
		pos.x = totalSpawnedEnv * offsetBetweenObjects;
		env.transform.position = pos;
		totalSpawnedEnv++;
	}

	void SpawnEnviromentOnce ()
	{
		enviromentList = new GameObject[enviromentCount];

		for (int i = 0; i < enviromentCount; i++) {
			GameObject env = Instantiate (enviromentPrefabs [(int)LevelInfo.enviroment].enviromentPrefabs [Random.Range (0, enviromentPrefabs [(int)LevelInfo.enviroment].enviromentPrefabs.Length)]);

			env.transform.SetParent (envHolder.transform);

			Vector3 pos = Vector3.zero;
			pos.x = offsetBetweenObjects * i;
			pos.y = -.5f;
			env.transform.position = pos;

			totalSpawnedEnv++;

			enviromentList [i] = env;
		}


	}

	[System.Serializable]
	public class EnviromentInfo {
		public LevelInfo.EnviromentType type;
		public GameObject[] enviromentPrefabs;
	}


}

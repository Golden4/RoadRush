using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobCarsController : SingletonGeneric<MobCarsController> {

	public MobCar[] mobPrefabs;

	public RoadLineInfo[] roadLineInfo = new RoadLineInfo[4];
	public int carsInOneLine = 4;

	Transform mobHolder;

	void Start ()
	{
		mobHolder = new GameObject ("Mobs").transform;

		if (LevelInfo.gameMode == LevelInfo.GameMode.OneSided) {
			roadLineInfo [0].dir = MoveDir.forward;
			roadLineInfo [1].dir = MoveDir.forward;
		} else {
			roadLineInfo [0].dir = MoveDir.back;
			roadLineInfo [1].dir = MoveDir.back;
		}

		CreateMobsPool ();

		SpawnMobs ();
	}

	/*	void SpawnMobsOnce ()
	{
		int randMobIndex = Random.Range (0, mobPrefabs.Length);

		for (int j = 0; j < roadLineInfo.Length; j++) {

			roadLineInfo [j].mobList = new GameObject[mobPrefabs.Length];

			for (int i = 0; i < mobPrefabs.Length; i++) {

				GameObject car = Instantiate (mobPrefabs [i]);

				Vector3 pos = Vector3.zero;
				pos.z = GameManager.Instance.RoadsCenterPointsByZ [j];
				pos.y = .56f;
				if (i > 0) {
					pos.x = roadLineInfo [j].mobList [i - 1].transform.position.x + Random.Range (10, 30);
				} else {
					pos.x = Random.Range (10, 30);
				}

				car.transform.position = pos;

				roadLineInfo [j].mobList [i] = car;

				if (mobPrefabs.Length - 1 == i) {
					roadLineInfo [j].lastCar = car;
				}

			}
		}

	}*/

	bool[] firstSpawn = new bool[4];

	void SpawnMobs ()
	{
		for (int i = 0; i < roadLineInfo.Length; i++) {
			
			for (int j = 0; j < carsInOneLine; j++) {
				SpawnMob (i);
			}

		}
	}

	public void SpawnMob (int line)
	{
		int index = Random.Range (0, mobPrefabs.Length);

		MobCar go = GetRandCarFromPool ();
		go.curLine = line;

		if (roadLineInfo [line].lastCar == null && !firstSpawn [line]) {
			firstSpawn [line] = true;
			Vector3 pos = Vector3.up * .5f;

			pos.z = roadLineInfo [line].roadCenterPos;
			pos.x = Random.Range (roadLineInfo [line].distance.x + 30, roadLineInfo [line].distance.y + 30);
			go.transform.position = pos;

			ChooseDirForCar (go, line);

		} else {

			go.transform.position = roadLineInfo [line].lastCar.transform.position - Random.Range (roadLineInfo [line].distance.x, roadLineInfo [line].distance.y) * Vector3.left;
			ChooseDirForCar (go, line);
		}

		go.CarSpeed = roadLineInfo [line].carSpeed;
		roadLineInfo [line].carsInLine.Add (go);
		roadLineInfo [line].lastCar = go.transform;

/*
		for (int i = 0; i < roadLineInfo.Length; i++) {
			for (int j = 0; j < roadLineInfo [i].carsInLine.Count; j++) {
				
				if (line == i)
					continue;

				if (roadLineInfo [i].dir == go.GetComponent<MobCar> ().moveDir) {
					if (Mathf.Abs (roadLineInfo [i].lastCar.transform.position.x - go.transform.position.x) < 15) {
						print (roadLineInfo [i].lastCar.transform.position.x - go.transform.position.x);
						go.transform.position -= Vector3.left * Random.Range (10, 20) * ((roadLineInfo [i].lastCar.transform.position.x - go.transform.position.x > 0) ? 1 : -1);
					}
				}
			}
		}*/




		/*for (int i = 0; i < roadLineInfo.Length; i++) {
			if (roadLineInfo [i].dir == go.moveDir && roadLineInfo [i].lastCar != null && line != i) {
				if (Mathf.Abs (roadLineInfo [i].lastCar.transform.position.x - go.transform.position.x) < 15) {
					go.transform.position -= Vector3.left * Random.Range (10, 20) * ((roadLineInfo [i].lastCar.transform.position.x - go.transform.position.x > 0) ? 1 : -1);
				}
			}
		}*/

	}

	void ChooseDirForCar (MobCar go, int line)
	{
		if (roadLineInfo [line].dir == MoveDir.back) {
			go.transform.eulerAngles = Vector3.up * 180;
			go.moveDir = MoveDir.back;
		} else {
			go.transform.eulerAngles = Vector3.zero;
			go.moveDir = MoveDir.forward;
		}
	}

	List<MobCar> reuseCarsList = new List<MobCar> ();

	void CreateMobsPool ()
	{
		for (int i = 0; i < mobPrefabs.Length; i++) {
			MobCar go = Instantiate (mobPrefabs [i]);
			reuseCarsList.Add (go);
			go.transform.SetParent (mobHolder);
			go.gameObject.SetActive (false);

		}

		for (int i = 0; i < roadLineInfo.Length * carsInOneLine; i++) {
			int index = Random.Range (0, mobPrefabs.Length);
			MobCar go = Instantiate (mobPrefabs [index]);
			reuseCarsList.Add (go);
			go.transform.SetParent (mobHolder);
			go.gameObject.SetActive (false);
		}
	}

	public void AddToReuse (MobCar car, int lineIndex)
	{
		roadLineInfo [lineIndex].carsInLine.Remove (car);
		reuseCarsList.Add (car);
		car.gameObject.SetActive (false);
	}

	MobCar GetRandCarFromPool ()
	{
		int index = Random.Range (0, reuseCarsList.Count);

		MobCar car = reuseCarsList [index];

		reuseCarsList.RemoveAt (index);

		car.gameObject.SetActive (true);

		return car;
	}


}

[System.Serializable]
public class RoadLineInfo {

	public int mobCount;
	public MoveDir dir;
	public float roadCenterPos;
	public Vector2 distance;
	public List<MobCar> carsInLine;
	public float carSpeed = 19;
	[HideInInspector]public Transform lastCar;

}
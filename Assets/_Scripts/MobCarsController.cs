using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobCarsController : SingletonGeneric<MobCarsController> {

	public MobCar[] mobPrefabs;

	public RoadLineInfo[] roadLineInfo = new RoadLineInfo[4];

    public enum TraficLevel
    {
        Low,
        Medium,
        High
    }

    public TraficLevel traficLevel;


    public int carsInOneLine = 4;
    
    public float visibleOffset = 50;
    float spawnLineOffset = 100;
    float maxForwardOffsetFromTarget = 150;
    float maxBackwardOffset = -25;

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

        carsInOneLine = 4 + (int) traficLevel;


        CreateMobsPool ();

        StartCoroutine(ModSpawner());

		//SpawnMobs ();
	}

    public float GetCurPlayerPosition()
    {
        if (PlayerCar.Ins != null)
            return PlayerCar.Ins.transform.position.x;
        else
            return 0;
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

    float GetRandomNextSpawnTime()
    {
        Vector2 clampTime = Vector2.zero;

        switch (traficLevel)
        {
            case TraficLevel.Low:
                clampTime = new Vector2(.2f, 4f);
                break;
            case TraficLevel.Medium:

                clampTime = new Vector2(.1f, 4f);
                break;
            case TraficLevel.High:
                clampTime = new Vector2(.05f, 3f);
                break;
            default:
                break;
        }
        
        return Random.Range(clampTime.x, clampTime.y);
    }

    IEnumerator ModSpawner()
    {
        while (true)
        {
            int lineIndex = Random.Range(0, 4);

            MobCar go = GetRandCarFromPool();

            if (go != null)
            {
                Vector3 pos = Vector3.up * .5f;

                pos.z = roadLineInfo[lineIndex].roadCenterPos;

                pos.x = GetCurPlayerPosition() + spawnLineOffset;

                go.transform.position = pos;

                go.SetDirection(roadLineInfo[lineIndex].dir);

                roadLineInfo[lineIndex].carsInLine.Add(go);
            }


            yield return new WaitForSeconds(GetRandomNextSpawnTime() * (30 / PlayerCar.Ins.speed));
        }
    }


    public void SpawnMob (int line)
	{
		int index = Random.Range (0, mobPrefabs.Length);

		MobCar go = GetRandCarFromPool ();
		go.curLine = line;

		if (roadLineInfo [line].GetLastCar() == null && !firstSpawn [line]) {

			firstSpawn [line] = true;
			Vector3 pos = Vector3.up * .5f;

			pos.z = roadLineInfo [line].roadCenterPos;
            
            pos.x = GetRandomNextSpawnTime();

            go.transform.position = pos;
            
		} else {

			//go.transform.position = roadLineInfo [line].GetLastCar().transform.position - Random.Range (roadLineInfo [line].distance.x, roadLineInfo [line].distance.y) * Vector3.left;
		}

        go.SetDirection(roadLineInfo[line].dir);
        
		roadLineInfo [line].carsInLine.Add (go);

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

        if (reuseCarsList.Count > 0)
        {
            MobCar car = reuseCarsList[index];

            reuseCarsList.RemoveAt(index);

            car.gameObject.SetActive(true);

            return car;
        }
        else
            return null;
	}


}

[System.Serializable]
public class RoadLineInfo {
	public MoveDir dir;
	public float roadCenterPos;
	public List<MobCar> carsInLine;

    public MobCar GetLastCar()
    {
        if(carsInLine.Count > 0)
            return carsInLine[0];
        else
            return null;
    }

}
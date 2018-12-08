using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCarController : SingletonGeneric<PlayerCarController> {

	public int curPlayerIndex = 0;
	public PlayerCar player;

	void Awake ()
	{
		SpawnPlayerToPos (Vector3.up * .56f);
	}

	public void SpawnPlayerToPos (Vector3 pos)
	{
		curPlayerIndex = Database.PlayerData.curCarIndex;

		PlayerCar car = Instantiate <GameObject> (Database.PlayerData.playerCarData [curPlayerIndex].playerPrefab.gameObject).GetComponent<PlayerCar> ();
		car.transform.position = pos;
		car.gameObject.AddComponent<AudioListener> ();

		car.GetComponent<MeshRenderer> ().materials [1].color = Database.PlayerData.playerCarData [curPlayerIndex].avaibleColorList [Database.PlayerData.playerStats [curPlayerIndex].colorIndex].color;

		player = car;
	}

	public IEnumerator BeforePauseCountDown ()
	{

		for (int i = 1; i <= 3; i++) {
			
			UIManager.Instance.BeforePauseCountDown (i);

			yield return new WaitForSeconds (1);
		}

	}

}

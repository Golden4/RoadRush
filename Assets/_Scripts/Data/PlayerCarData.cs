using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu ()]
public class PlayerCarData : ScriptableObject {
	
	public CarData[] playerCarData;

	public CarStats[] playerStats;

	public LevelsStats levelsData;

	public int curCarIndex;


}

[System.Serializable]
public class CarData {
	public PlayerCar playerPrefab;
	public int price;
	public int needLevelToUnlock;
	public int priceToUnlock;
	public CarColor[] avaibleColorList;
}

[System.Serializable]
public class CarStats {
	public bool bought = false;
	public bool unlocked = false;
	public int colorIndex = 0;

	public List<int> colorsIsAvaible = new List<int> ();

	public int boostLvlMaxSpeed = 0;
	public int boostLvlAcceleration = 0;
	public int boostLvlHandling = 0;
	public int boostLvlBreaking = 0;
	public int boostLvlNitro = 0;
	public int boostLvlDurability = 0;
}

[System.Serializable]
public class LevelsStats {
	public int oneSidedRecord = 0;
	public int twoSidedRecord = 0;
	public int inTimeRecord = 0;

	public List<int> locationsAvaible = new List<int> ();

}

[System.Serializable]
public class CarColor {
	public Color color;
	public int price;
}
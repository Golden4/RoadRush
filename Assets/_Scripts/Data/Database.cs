using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database {
	
	static PlayerCarData playerData;

	static bool loaded = false;

	public static PlayerCarData PlayerData {
		get {
			if (playerData == null) {
				playerData = Resources.Load ("Data/PlayerData") as PlayerCarData;

				if (!loaded) {
					loaded = true;

					if (!LoadAll ()) {
						SaveAll ();
					}
				}
			}

			return playerData;
		}
	}

	public static void SaveCar (int index)
	{
		//	MonoBehaviour.print ("saved" + index);

		PlayerPrefs.SetString ("Car" + index, SaveLoadHelper.ObjectToStr<CarStats> (PlayerData.playerStats [index]));
	}

	public static void SaveAll ()
	{
		for (int i = 0; i < PlayerData.playerStats.Length; i++) {
			SaveCar (i);
		}

		PlayerPrefs.SetInt ("CarIndex", Database.PlayerData.curCarIndex);
		PlayerPrefs.SetString ("LevelStats", SaveLoadHelper.ObjectToStr<LevelsStats> (PlayerData.levelsData));
	}

	public static bool LoadAll ()
	{

		//PlayerPrefs.DeleteAll ();

		PlayerData.playerStats = new CarStats[PlayerData.playerCarData.Length];
        
		bool hasLoad = true;

		for (int i = 0; i < PlayerData.playerCarData.Length; i++) {
			if (PlayerPrefs.HasKey ("Car" + i)) {
				PlayerData.playerStats [i] = SaveLoadHelper.StrToObject<CarStats> (PlayerPrefs.GetString ("Car" + i));
			}
			else {

				PlayerData.playerStats [i] = new CarStats ();
				PlayerData.playerStats [i].colorsIsAvaible = new List<int> ();
				PlayerData.playerStats [i].colorsIsAvaible.Add (0);

				if (i == 0) {
					PlayerData.playerStats [0].bought = true;
				}

				hasLoad = false;
			}
		}

		if (PlayerPrefs.HasKey ("CarIndex")) {
			Database.PlayerData.curCarIndex = PlayerPrefs.GetInt ("CarIndex");
		}
		else {
			Database.PlayerData.curCarIndex = 0;
		}

		if (PlayerPrefs.HasKey ("LevelStats")) {
			Database.PlayerData.levelsData = SaveLoadHelper.StrToObject<LevelsStats> (PlayerPrefs.GetString ("LevelStats"));
		}
		else {
			Database.PlayerData.levelsData.locationsAvaible = new List<int> ();
			Database.PlayerData.levelsData.locationsAvaible.Add (0);
		}

		return hasLoad;

	}

	public static int GetLevelRecord (LevelInfo.GameMode mode)
	{
		switch (mode) {
		case LevelInfo.GameMode.OneSided:
			return Database.PlayerData.levelsData.oneSidedRecord;
			break;
		case LevelInfo.GameMode.TwoSided:
			return  Database.PlayerData.levelsData.twoSidedRecord;
			break;
		case  LevelInfo.GameMode.InTime:
			return Database.PlayerData.levelsData.inTimeRecord;
			break;
		}

		return 0;

	}

	public static void SetLevelRecord (LevelInfo.GameMode mode, int score)
	{
		switch (mode) {
		case LevelInfo.GameMode.OneSided:
			Database.PlayerData.levelsData.oneSidedRecord = score;
			break;
		case LevelInfo.GameMode.TwoSided:
			Database.PlayerData.levelsData.twoSidedRecord = score;
			break;
		case  LevelInfo.GameMode.InTime:
			Database.PlayerData.levelsData.inTimeRecord = score;
			break;
		}


	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInfo {

	public static GameMode gameMode;

	public enum GameMode
	{
		OneSided,
		TwoSided,
		InTime
	}

	public static EnviromentType enviroment;

	public enum EnviromentType
	{
		City,
		Desert,
		Snow
	}
}

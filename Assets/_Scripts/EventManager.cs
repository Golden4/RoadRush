using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class EventManager {

	public static event Action OnPlayerDeathEvent;

	public static void OnPlayerDeathCall ()
	{
		if (OnPlayerDeathEvent != null) {
			OnPlayerDeathEvent ();
		}
	}

	public static event Action OnPlayerRespawnedEvent;

	public static void OnPlayerRespawnedCall ()
	{
		if (OnPlayerRespawnedEvent != null) {
			OnPlayerRespawnedEvent ();
		}
	}

	public static event Action<int, int> OnMoneyChangedEvent;

	public static void OnMoneyChangedCall (int startMoney, int finalMoney)
	{
		if (OnMoneyChangedEvent != null) {
			OnMoneyChangedEvent (startMoney, finalMoney);
		}
	}

	public static event Action<int, int> OnRubyChangedEvent;

	public static void OnRubyChangedCall (int startRuby, int finalRuby)
	{
		if (OnRubyChangedEvent != null) {
			OnRubyChangedEvent (startRuby, finalRuby);
		}
	}

}

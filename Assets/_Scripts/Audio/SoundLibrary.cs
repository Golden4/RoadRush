using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLibrary : SingletonGeneric<SoundLibrary> {

	public SoundGroup[] soundGroup;

	#if UNITY_EDITOR
	void OnValidate ()
	{
		for (int i = 0; i < soundGroup.Length; i++) {
			for (int j = 0; j < soundGroup [i].clips.Length; j++) {
				soundGroup [i].clips [j].name = soundGroup [i].groupName + (j + 1);
			}
		}
	}
	#endif

	public Sound GetRandomClipFromName (string groupName)
	{
		int index = System.Array.FindIndex (soundGroup, x => x.groupName == groupName);

		if (index == -1) {
			Debug.LogError ("Clips Not Found" + groupName);
			return null;
		}
		
		return soundGroup [index].clips [Random.Range (0, soundGroup [index].clips.Length)];
	}

	[System.Serializable]
	public class SoundGroup {
		public string groupName;

		public Sound[] clips;

	}
}


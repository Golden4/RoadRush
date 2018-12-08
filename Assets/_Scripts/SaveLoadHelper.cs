using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public static class SaveLoadHelper {

	public static void Save <T> (T[] list) where T : class
	{
		PlayerPrefs.SetString (typeof(T).Name, ObjectToStr<T[]> (list));

		MonoBehaviour.print ("StagesSaved: " + typeof(T).Name);

		PlayerPrefs.Save ();
	}

	public static T[] Load <T> () where T : class
	{
		T[] list = null;

		if (PlayerPrefs.HasKey (typeof(T).Name)) {
			
			list = StrToObject<T[]> (PlayerPrefs.GetString (typeof(T).Name));

		}

		return list;

	}


	public static string ObjectToStr<T> (T _saveMe) where T : class
	{
		BinaryFormatter bf = new BinaryFormatter ();
		MemoryStream memStream = new MemoryStream ();
		bf.Serialize (memStream, _saveMe);

		string info = Convert.ToBase64String (memStream.GetBuffer ());

		memStream.Close ();

		return info;

	}

	public static T StrToObject<T> (string data) where T : class
	{

		if (!string.IsNullOrEmpty (data)) {
			BinaryFormatter bf = new BinaryFormatter ();

			try {
				MemoryStream mem = new MemoryStream (Convert.FromBase64String (data));

				T obj = bf.Deserialize (mem) as T;

				mem.Close ();

				return obj;

			} catch (Exception ex) {
				throw new Exception (ex.Message);
			}
		} else {
			throw new Exception ("data is null or empty");
		}
	}

}

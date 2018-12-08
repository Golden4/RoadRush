using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public class SingletonGeneric<T> : MonoBehaviour where T: MonoBehaviour {

	static bool instanceChecked = false;

	static T instance;

	public static T Instance {
		get {
			return SingletonGeneric<T>.instance;
		}
	}

	public virtual void Awake ()
	{

		if (SingletonGeneric<T>.instance == null) {
			SingletonGeneric<T>.instance = FindObjectOfType<T> ();

		} else if (!SingletonGeneric<T>.instanceChecked) {

			T[] objects = FindObjectsOfType<T> ();

			if (objects.Length > 1) {

				for (int i = 0; i < objects.Length; i++) {
					if (objects [i] != SingletonGeneric<T>.instance)
						DestroyImmediate (objects [i]);
				}
			}

			SingletonGeneric<T>.instanceChecked = true;
		}

	}

}*/
public class SingletonGeneric<T> : MonoBehaviour where T : MonoBehaviour {
	private static T _instance;

	private static object _lock = new object ();

	public static T Instance {
		get {
			if (applicationIsQuitting) {
				Debug.LogWarning ("[Singleton] Instance '" + typeof(T) +
				"' already destroyed on application quit." +
				" Won't create again - returning null.");
				return null;
			}

			lock (_lock) {
				if (_instance == null) {
					_instance = (T)FindObjectOfType (typeof(T));

					T[] objects = FindObjectsOfType <T> ();

					if (objects.Length > 1) {
						Debug.LogError ("[Singleton] Something went really wrong " +
						" - there should never be more than 1 singleton!" +
						" Reopening the scene might fix it.");
						
						foreach (T obj in objects) {
							if (obj != _instance)
								DestroyImmediate (obj);
						}

						return _instance;
					}

					if (_instance == null) {
						GameObject singleton = new GameObject ();
						_instance = singleton.AddComponent<T> ();
						singleton.name = "(singleton) " + typeof(T).ToString ();

						Debug.Log ("[Singleton] An instance of " + typeof(T) +
						" is needed in the scene, so '" + singleton +
						"' was created with DontDestroyOnLoad.");
					}
					else {
						/*	Debug.Log ("[Singleton] Using instance already created: " +
						_instance.gameObject.name);*/
					}
				}

				return _instance;
			}
		}
	}

	private static bool applicationIsQuitting = false;
}

public class SingletonResource<T> : MonoBehaviour where T : MonoBehaviour {
	static T StaticInstance;

	static object _lock = new object ();

	public static T Instance {
		get {
			lock (_lock) {
				if (StaticInstance == null) {
					GameObject manager = Resources.Load ("Singleton/" + typeof(T).Name) as GameObject;
					T IstManager = Instantiate (manager).GetComponent<T> ();
					StaticInstance = IstManager;
					StaticInstance.GetComponent<SingletonResource<T>> ().Initialize ();
				}

				return StaticInstance;
			}
		}
	}

	public virtual void Initialize ()
	{
		
	}

	protected virtual void Start ()
	{
		if (StaticInstance == null) {
			DontDestroyOnLoad (gameObject);
			StaticInstance = this.GetComponent<T> ();
			Initialize ();
		}
		else if (this != StaticInstance) {
			Destroy (gameObject);
		}
	}


}


/*public static class SingletonResource<T> where T {
	private static T StaticInstance;

	public static T Instance ()
	{
		if (SingletonResource<T>.StaticInstance == null) {
			SingletonResource<T>.StaticInstance = Resources.Load<T> ("Singleton/" + typeof(T).Name);
			SingletonResource<T>.StaticInstance.Start ();
		}
		return SingletonResource<T>.StaticInstance;
	}
}*/


 


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class KMonoBehaviour : MonoBehaviour {

	public static bool isLoadingScene;

	bool isInitilized;

	public bool isSpawned{ get; private set; }


	void Awake ()
	{
		InitializeComponent ();
	}

	void Start ()
	{
		Spawn ();
	}

	public void InitializeComponent ()
	{

		if (!isInitilized) {

			try {
				OnInit ();

			} catch (Exception e) {

				Debug.LogError (e.ToString ());

			}


			isInitilized = true;
		}
	}

    void Spawn()
    {
        if (!isSpawned)
        {

            string name = base.GetType().Name;

            if (isInitilized)
            {
                isSpawned = true;
                
                try
                {
                    this.OnSpawn();
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());

                }
            }else
            {
                Debug.LogError(base.name + "." + name + " is not initilized.", null);
            }
        }
    }

	public virtual void OnInit ()
	{
	}

	public virtual void OnSpawn ()
	{
		
	}

	public virtual void OnCmpEnable ()
	{
		
	}

	public virtual void OnCmpDisable ()
	{
		
	}

	public virtual void OnCleanUp ()
	{
		
	}

	void OnDestroy ()
	{
		OnCleanUp ();
	}

	void OnEnable ()
	{
		OnCmpEnable ();
	}

	void OnDisable ()
	{
		OnCmpDisable ();
	}

}
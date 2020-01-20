using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SingletonResource<AudioManager> {

	public float masterVolumePercent = 1f;
	public float sfxVolumePercent = 1f;
	public float musicVolumePercent = 1f;

	public static bool musicEnabled = true;
	public static bool sfxEnabled = true;
    
	bool startedMusic;
	bool startedSFX;

	/*	private static AudioManager StaticInstance;

	public static AudioManager Instance {
		get {
			if (StaticInstance == null) {
				GameObject manager = Resources.Load ("Singleton/AudioManager") as GameObject;
				AudioManager audioManager = Instantiate (manager).GetComponent<AudioManager> ();
				DontDestroyOnLoad (audioManager);
				StaticInstance = audioManager;
				audioManager.Initialize ();
			}

			return StaticInstance;
		}
	}*/

	AudioSource musicSource;
	AudioSource sfxSource;

	/*	void Start ()
	{
		if (StaticInstance == null) {
			DontDestroyOnLoad (gameObject);
			StaticInstance = this;
			Initialize ();
		} else if (this != StaticInstance) {
			Destroy (gameObject);
		}
	}*/

	public override void Initialize ()
	{
		DontDestroyOnLoad (this);

		CreateMusicSourse ();
        CreateSfxSource();

        Subsribe ();
		ChangeMusic (UnityEngine.SceneManagement.SceneManager.GetActiveScene (), UnityEngine.SceneManagement.LoadSceneMode.Single);
	}

	public void PauseSounds ()
	{
        musicSource.Pause ();
		

		sfxSource.Pause ();
	}

	public void UnpauseSounds ()
	{
        musicSource.UnPause ();

		sfxSource.UnPause ();
	}

	public void StopSounds ()
	{
        musicSource.Stop ();

		sfxSource.Stop ();
	}

	public void StartSounds ()
	{
        musicSource.Play ();
		

		sfxSource.Play ();
	}

	void Update ()
	{

	}

	void CreateMusicSourse ()
	{
		GameObject go = new GameObject ("MusicSource");
		musicSource = go.AddComponent<AudioSource> ();
		musicSource.loop = true;
		musicSource.transform.parent = transform;
		
	}

    void CreateSfxSource()
    {
        GameObject newGO = new GameObject("SfxSource");
        sfxSource = newGO.AddComponent<AudioSource>();
        newGO.transform.parent = transform;
    }

	public void PlayMusic (AudioClip clip)
	{
		if (musicSource == null) {
			CreateMusicSourse ();
		}

		if (!musicEnabled)
			return;
        
		musicSource .clip = clip;
		musicSource.volume = masterVolumePercent * musicVolumePercent;
		musicSource.Play ();
	}

	public void PlayMusic (string name)
	{

		if (!musicEnabled)
			return;
               
        if (TryGetComponent<MusicManager>(out MusicManager mm)) {

            if (!mm.playMusic)
                return;

            if (name == "Shop") {
                PlayMusic(mm.shopTheme);
            }

            if (name == "Main") {
                PlayMusic(mm.mainThemes[Random.Range(0, mm.mainThemes.Length)]);
            }
        }
	}

	public void StopMusic ()
	{
		musicSource.Stop ();
		musicSource.clip = null;
	}

	public void PlaySound (AudioClip clip, Vector3 pos)
	{
		if (!sfxEnabled)
			return;

		AudioSource.PlayClipAtPoint (clip, pos, sfxVolumePercent * masterVolumePercent);
	}

	public void PlaySound (string clipName, Vector3 pos)
	{
		if (!sfxEnabled)
			return;
		PlayClipAtPoint (SoundLibrary.Instance.GetRandomClipFromName (clipName), pos, sfxVolumePercent * masterVolumePercent);

	}

	public void PlaySound2D (string clipName)
	{
		if (!sfxEnabled)
			return;
		SoundLibrary.Instance.GetRandomClipFromName (clipName).PlaySound (sfxSource);

		//sfxSource.PlayOneShot (SoundLibrary.Instance.GetRandomClipFromName (clipName).clip, sfxVolumePercent * masterVolumePercent);
	}


	public void PlaySound2D (Sound clip)
	{
		if (!sfxEnabled)
			return;
		clip.PlaySound (sfxSource);

		//sfxSource.PlayOneShot (SoundLibrary.Instance.GetRandomClipFromName (clipName).clip, sfxVolumePercent * masterVolumePercent);
	}

	public void PlayClipAtPoint (Sound sound, Vector3 pos, float volume)
	{
		if (!sfxEnabled)
			return;

		GameObject go = new GameObject ("Playing " + sound.name);
		go.transform.position = pos;

		AudioSource source = go.AddComponent<AudioSource> ();

		source.volume = volume;

		sound.PlaySound (source);

		Destroy (go, sound.clip.length);

	}

	void Subsribe ()
	{
		UnityEngine.SceneManagement.SceneManager.sceneLoaded += ChangeMusic;
		UnityEngine.SceneManagement.SceneManager.sceneUnloaded += StopMusicUnloadScene;
	}

	void StopMusicUnloadScene (UnityEngine.SceneManagement.Scene arg0)
	{
		StopMusic ();
	}

	public void Unsubsribe ()
	{
		UnityEngine.SceneManagement.SceneManager.sceneLoaded -= ChangeMusic;
		UnityEngine.SceneManagement.SceneManager.sceneUnloaded -= StopMusicUnloadScene;
	}

	void ChangeMusic (UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.LoadSceneMode arg1)
	{

		if (arg0.buildIndex == 1) {
			PlayMusic ("Shop");
		}

		if (arg0.buildIndex == 2) {
			PlayMusic ("Main");
		}
	}

}

[System.Serializable]
public class Sound {
	public string name;
	public AudioClip clip;

	[Range (0, 1.5f)]
	public float volume = 0.7f;

	[Range (0.5f, 1.5f)]
	public float pitch = 1f;

	[Range (0f, 0.5f)]
	public float randomVolume = 0.1f;

	[Range (0f, 0.5f)]
	public float randomPitch = 0.1f;

	AudioSource source;

	public void PlaySound (AudioSource source)
	{
		source.clip = clip;
		source.volume = volume * (1 + Random.Range (-randomVolume / 2f, randomVolume / 2f));
		source.pitch = pitch * (1 + Random.Range (-randomPitch / 2f, randomPitch / 2f));
		source.Play ();
	}

}

	

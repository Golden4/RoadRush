using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CarAudio : MonoBehaviour {

	public enum EngineAudioOptions
	{
		Simple,
		// Simple style audio
		FourChannel
		// four Channel audio
	}

	public EngineAudioOptions engineSoundStyle = EngineAudioOptions.FourChannel;
	// Set the default audio options to be four channel
	public AudioClip lowAccelClip;
	// Audio clip for low acceleration
	public AudioClip lowDecelClip;
	// Audio clip for low deceleration
	public AudioClip highAccelClip;
	// Audio clip for high acceleration
	public AudioClip highDecelClip;
	// Audio clip for high deceleration
	public AudioClip skidSoundClip;
	[Range (0, 1f)]
	public float volumeLevel;

	public float pitchMultiplier = 1f;
	// Used for altering the pitch of audio clips
	public float lowPitchMin = 1f;
	// The lowest possible pitch for the low sounds
	public float lowPitchMax = 6f;
	// The highest possible pitch for the low sounds
	public float highPitchMultiplier = 0.25f;
	// Used for altering the pitch of high sounds
	public float maxRolloffDistance = 500;
	// The maximum distance where rollof starts to take place
	public float dopplerLevel = 1;
	// The mount of doppler effect used in the audio
	bool useDoppler = false;
	// Toggle for using doppler

	private AudioSource m_LowAccel;
	// Source for the low acceleration sounds
	private AudioSource m_LowDecel;
	// Source for the low deceleration sounds
	private AudioSource m_HighAccel;
	// Source for the high acceleration sounds
	private AudioSource m_HighDecel;
	// Source for the high deceleration sounds
	private AudioSource m_Skid;
	private bool m_StartedSound = false;
	// flag for knowing if we have started sounds
	private PlayerCar m_CarController;
    // Reference to car we are controlling

    public float Revs { get; private set; }

    private void StartSound ()
	{
		// get the carcontroller ( this will not be null as we have require component)
		m_CarController = GetComponent<PlayerCar> ();

		// setup the simple audio source
		m_HighAccel = SetUpEngineAudioSource (highAccelClip);

		// if we have four channel audio setup the four audio sources
		if (engineSoundStyle == EngineAudioOptions.FourChannel) {
			m_LowAccel = SetUpEngineAudioSource (lowAccelClip);
			m_LowDecel = SetUpEngineAudioSource (lowDecelClip);
			m_HighDecel = SetUpEngineAudioSource (highDecelClip);
			m_Skid = SetUpEngineAudioSource (skidSoundClip);
		}

		// flag that we have started the sounds playing
		m_StartedSound = true;
	}


	private void StopSound ()
	{
		//Destroy all audio sources on this object:
		foreach (var source in GetComponents<AudioSource>()) {
			Destroy (source);
		}

		m_StartedSound = false;
	}

	void StopPlayingSounds ()
	{
		foreach (var source in GetComponents<AudioSource>()) {
			source.Pause ();
		}
	}

	void StartPlayingSounds ()
	{
		foreach (var source in GetComponents<AudioSource>()) {
			source.UnPause ();
		}
	}

	float inputLerp;
	float inputLerped;
	// Update is called once per frame
	private void Update ()
	{

		// get the distance to main camera
		float camDist = (Camera.main.transform.position - transform.position).sqrMagnitude;

		// stop sound if the object is beyond the maximum roll off distance
		if (m_StartedSound && camDist > maxRolloffDistance * maxRolloffDistance && !AudioManager.sfxEnabled) {
			StopSound ();
		}

		// start the sound if not playing and it is nearer than the maximum distance
		if (!m_StartedSound && camDist < maxRolloffDistance * maxRolloffDistance && AudioManager.sfxEnabled) {
			StartSound ();
		}

		if (m_StartedSound) {

            CalculateRevs();

            // The pitch is interpolated between the min and max values, according to the car's revs.
            float pitch = ULerp (lowPitchMin, lowPitchMax, Revs);

			// clamp to minimum pitch (note, not clamped to max for high revs while burning out)
			pitch = Mathf.Min (lowPitchMax, pitch);

			if (engineSoundStyle == EngineAudioOptions.Simple) {
				// for 1 channel engine sound, it's oh so simple:
				m_HighAccel.pitch = pitch * pitchMultiplier * highPitchMultiplier;
				m_HighAccel.dopplerLevel = useDoppler ? dopplerLevel : 0;
				m_HighAccel.volume = 1;
			}
			else {
				// for 4 channel engine sound, it's a little more complex:

				// adjust the pitches based on the multipliers
				m_LowAccel.pitch = pitch * pitchMultiplier;
				m_LowDecel.pitch = pitch * pitchMultiplier;
				m_HighAccel.pitch = pitch * highPitchMultiplier * pitchMultiplier;
				m_HighDecel.pitch = pitch * highPitchMultiplier * pitchMultiplier;
                

				inputLerped = Mathf.Lerp (inputLerped, m_CarController.input.y, Time.deltaTime * 3);

				// get values for fading the sounds based on the acceleration
				float accFade = Mathf.Clamp01 (inputLerped);
				float decFade = 1 - accFade;

				// get the high fade value based on the cars revs
				float highFade = Mathf.InverseLerp (0.2f, 0.8f, Revs);
				float lowFade = 1 - highFade;

				// adjust the values to be more realistic
				highFade = 1 - ((1 - highFade) * (1 - highFade));
				lowFade = 1 - ((1 - lowFade) * (1 - lowFade));
				accFade = 1 - ((1 - accFade) * (1 - accFade));
				decFade = 1 - ((1 - decFade) * (1 - decFade));


				// adjust the source volumes based on the fade values

				float soundVolumePersent = AudioManager.Instance.sfxVolumePercent * AudioManager.Instance.masterVolumePercent;

                float lowSoundVolume = .3f;
                
                m_LowAccel.volume = lowFade * accFade * soundVolumePersent * volumeLevel * lowSoundVolume;
				m_LowDecel.volume = lowFade * decFade * soundVolumePersent * volumeLevel * lowSoundVolume;
				m_HighAccel.volume = highFade * accFade * soundVolumePersent * volumeLevel;
				m_HighDecel.volume = highFade * decFade * soundVolumePersent * volumeLevel * .7f;


				// adjust the doppler levels
				m_HighAccel.dopplerLevel = useDoppler ? dopplerLevel : 0;
				m_LowAccel.dopplerLevel = useDoppler ? dopplerLevel : 0;
				m_HighDecel.dopplerLevel = useDoppler ? dopplerLevel : 0;
				m_LowDecel.dopplerLevel = useDoppler ? dopplerLevel : 0;

                print("Speed:" + m_CarController.speed + " Gear:" + m_CarController.curGear + " Pitch: la:" + m_LowAccel.pitch + "-ld:" + m_LowDecel.pitch + "-ha:" + m_HighAccel.pitch + "-hd:" + m_HighDecel.pitch + "   " + " voloume: la:" + m_LowAccel.volume + "-ld:" + m_LowDecel.volume + "-ha:" + m_HighAccel.volume + "-hd:" + m_HighDecel.volume);


                if (m_CarController.input.y < 0 || m_CarController.input.x > .95f || m_CarController.input.x < -.95f)
                    inputLerp = Mathf.Lerp(inputLerp, m_CarController.input.y, Time.deltaTime * 7);
                else
                {
                    inputLerp = Mathf.Lerp(inputLerp, 0, Time.deltaTime * 7);
                }

                m_Skid.volume = Mathf.Abs(inputLerp) / 1.5f;


            }
		}
	}


	// sets up and adds new audio source to the gane object
	private AudioSource SetUpEngineAudioSource (AudioClip clip)
	{
		// create the new audio source component on the game object and set up its properties
		AudioSource source = gameObject.AddComponent<AudioSource> ();
		source.clip = clip;
		source.volume = 0;
		source.loop = true;

		// start the clip from a random point
		source.time = Random.Range (0f, clip.length);
		source.Play ();
		source.minDistance = 5;
		source.maxDistance = maxRolloffDistance;
		source.dopplerLevel = 0;
		return source;
	}


	// unclamped versions of Lerp and Inverse Lerp, to allow value to exceed the from-to range
	float ULerp (float from, float to, float value)
	{
		return (1.0f - value) * from + value * to;

	}

	public void PauseSounds ()
	{
		if (!AudioManager.sfxEnabled && m_StartedSound)
			return;
		try {
			m_LowAccel.Pause ();
			m_LowDecel.Pause ();
			m_HighAccel.Pause ();
			m_HighDecel.Pause ();
			m_Skid.Pause ();
		} catch (Exception e) {
			
		}
	}

	public void UnpauseSounds ()
	{
		if (!AudioManager.sfxEnabled)
			return;

		m_LowAccel.UnPause ();
		m_LowDecel.UnPause ();
		m_HighAccel.UnPause ();
		m_HighDecel.UnPause ();
		m_Skid.UnPause ();
	}


    float m_GearFactor;

    void CalculateRevs()
    {

        CalculateGearFactor();

        float prevGearSpeed = ((m_CarController.curGear > 0) ? m_CarController.gears[m_CarController.curGear - 1].speedForSwitch : m_CarController.minSpeed);

        float revs = Mathf.InverseLerp(prevGearSpeed, m_CarController.gears[m_CarController.curGear].speedForSwitch, m_CarController.speed);

        float gearNumFactor = prevGearSpeed / m_CarController.gears[m_CarController.gears.Length - 1].speedForSwitch;

        var revsRangeMin = ULerp(0f, 1, CurveFactor(gearNumFactor));
        var revsRangeMax = ULerp(1, 1f, gearNumFactor);

        Revs = ULerp(revsRangeMin, revsRangeMax, m_GearFactor);
    }
    
    void CalculateGearFactor()
    {
        float prevGearSpeed = ((m_CarController.curGear > 0) ? m_CarController.gears[m_CarController.curGear - 1].speedForSwitch : m_CarController.minSpeed);

        float gearNumFactor = prevGearSpeed / m_CarController.gears[m_CarController.gears.Length - 1].speedForSwitch;

        var targetGearFactor = Mathf.InverseLerp(prevGearSpeed, m_CarController.gears[m_CarController.curGear].speedForSwitch, m_CarController.speed);

        m_GearFactor = Mathf.Lerp(m_GearFactor, targetGearFactor, Time.deltaTime * 5f);
    }
    
    float CurveFactor(float factor)
    {
        return 1 - (1 - factor) * (1 - factor);
    }

    void Awake ()
	{
		EventManager.OnPlayerDeathEvent += StopPlayingSounds;
		EventManager.OnPlayerRespawnedEvent += StartPlayingSounds;
	}

	void OnDestroy ()
	{
		EventManager.OnPlayerDeathEvent -= StopPlayingSounds;
		EventManager.OnPlayerRespawnedEvent -= StartPlayingSounds;
	}
}
	

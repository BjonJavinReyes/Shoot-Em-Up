using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour 
{
	#region Audio Clips
	[SerializeField] AudioClip Shotgun_Fire;
	[SerializeField] AudioClip Shotgun_Reload;
	[SerializeField] AudioClip Shotgun_Reload_Final;
	[SerializeField] AudioClip Shotgun_Out_Of_Ammo;
	#endregion
	
	public Dictionary<int, AudioClip> soundDictionary;
	
	public enum SoundClips
	{
		SHOTGUN_FIRE = 0,
		SHOTGUN_RELOAD,
		SHOTGUN_RELOAD_FINAL,
		SHOTGUN_OUT_OF_AMMO
	}
	
	void Awake()
	{
		// Set up sound dictionary
		soundDictionary = new Dictionary<int, AudioClip>();
		
		// Add audio clips to dictionary
		soundDictionary.Add((int)SoundClips.SHOTGUN_FIRE, Shotgun_Fire);
		soundDictionary.Add((int)SoundClips.SHOTGUN_RELOAD, Shotgun_Reload);
		soundDictionary.Add((int)SoundClips.SHOTGUN_RELOAD_FINAL, Shotgun_Reload_Final);
		soundDictionary.Add((int)SoundClips.SHOTGUN_OUT_OF_AMMO, Shotgun_Out_Of_Ammo);
	}
	
	public void PlayAudioClip(int key)
	{
		if( soundDictionary.ContainsKey(key) && soundDictionary[key])
			audio.PlayOneShot(soundDictionary[key], 0.7f);
	}
}

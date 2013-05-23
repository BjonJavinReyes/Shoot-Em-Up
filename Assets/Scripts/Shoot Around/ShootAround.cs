using UnityEngine;
using System.Collections;

public class ShootAround: MonoBehaviour 
{
	// Other Scripts
	private AudioManager audio_man;
	
	// Screen Resolution
	float screen_footer_height;						// Footer height
	Vector2 screen_ratio = new Vector2(800, 480);	// Default screen size
	
	// Appliaction Textures
	[SerializeField] GUITexture shotgun;
	[SerializeField] Texture2D texture_shotgun;
	
	// Application Text
	[SerializeField] GUIText[] text_reload;
	[SerializeField] GUIText text_ammo;
	[SerializeField] GUIText debug_text;
	private int reload_text_size = 50;
	private int ammo_text_size = 40;
	
	// Shotgun Fire Effects
	[SerializeField] ParticleSystem shotgun_particles;
	[SerializeField] Light[] shotgun_lights;
	private float light_intesity_max = 1.8f;
	
	// Ammunition
	private int maximum_ammo = 8;
	private int current_ammo;
	
	// Timers
	float time_between_shots = 0.2f;
	float time_between_no_ammo = 0.2f;
	float time_between_reload = 0.3f;
	float time_final_reload = 0.65f;
	float time_destroy_particle = 1.0f;
	
	// Booleans
	private bool can_fire_weapon = true;
	private bool no_ammuntion = false;
	private bool reloading_weapon = false;
	private bool user_fired_weapon = false;
	
	void Awake()
	{
		// Link other scripts
		audio_man = gameObject.GetComponent<AudioManager>();
		
		// Initalize Application
		Initialize();
	}
	
	void Initialize()
	{
		// Height for footer
		screen_footer_height = Screen.height - Screen.height/6;
		
		// Add Shotgun Texture
		shotgun.texture = texture_shotgun;
		shotgun.pixelInset = new Rect(-(texture_shotgun.width/2), -(texture_shotgun.height/2),
										texture_shotgun.width, texture_shotgun.height);
		
		// Scale shotgun texture to screen size
		Vector2 shotgun_size;
		shotgun_size.x = ( (shotgun.pixelInset.width * Screen.width) / screen_ratio.x);
		shotgun_size.y = ( (shotgun.pixelInset.height * Screen.height) / screen_ratio.y);
		shotgun.pixelInset = new Rect(-(shotgun_size.x/2), -(shotgun_size.y/2), shotgun_size.x, shotgun_size.y);
		
		// Scale reload text to screen size
		for (int i=0; i < text_reload.Length; i++)
			text_reload[i].fontSize = (int)( (reload_text_size * Screen.height) / screen_ratio.y);
		
		// Scale ammo text to screen size
		text_ammo.fontSize = (int)( (ammo_text_size * Screen.height) / screen_ratio.y);
		
		// Set current ammo to maximum ammuntion
		current_ammo = maximum_ammo;
		
		// Set ammunition text to default value;
		text_ammo.text = (current_ammo.ToString() + "/" + maximum_ammo.ToString());
		
		// Set debug text to null
		debug_text.text = "";
	}
	
	void OnGUI()
	{	
		// Make button background colors transparent
		GUI.backgroundColor = new Color(1,1,1,0);

		// Shotgun Button (consists of full screen minus footer)
		if (GUI.Button (new Rect(0, 0, Screen.width, screen_footer_height), ""))
			FireShotgun();
		
		// Reload Button
		if (GUI.Button (new Rect(0, screen_footer_height, Screen.width/4, Screen.height/6), ""))
			ReloadWeapon();
		
	}
	
	void Update()
	{
		ApplicationInput();	
	}
	
	void ApplicationInput ()
	{
		// Android Back Button
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			DebugText("Back Button");
			Application.Quit();
		}
		
		// Android Menu Button
		if (Input.GetKeyDown(KeyCode.Menu))
		{
			DebugText("Menu Button");
		}
		
		// Android Home Button
		if (Input.GetKeyDown(KeyCode.Home))
		{	
		}
	}
	
	#region Fire Weapon
	void FireShotgun()
	{
		// User pressed trigger
		user_fired_weapon = true;
		
		if ( current_ammo == 0)
		{
			if (!no_ammuntion)
			{
				// Play shotgun out of ammo audioclip
				audio_man.PlayAudioClip((int)AudioManager.SoundClips.SHOTGUN_OUT_OF_AMMO);
				
				// Start Delay Timer for Firing
				StartCoroutine( NoAmmoDelay() );
				
				// Debug Text
				DebugText("No Ammo");
			}
		}
		else
		{
		
			// If time delay is up, not reloading, and ammo is greater then 0... FIRE
			if (can_fire_weapon &&
				!reloading_weapon)
			{
				// Debug Text
				DebugText("Fire!");
				
				// Vibrate Device when shoot
				Handheld.Vibrate();
	
				// Play shotgun fire audioclip
				audio_man.PlayAudioClip((int)AudioManager.SoundClips.SHOTGUN_FIRE);
				
				// Deduct one bullet from ammunition and update text
				current_ammo--;
				UpdateAmmunitionText();
				
				//Add particles to shot
				ParticleSystem particle = Instantiate(shotgun_particles) as ParticleSystem;
				particle.GetComponent<DestroyParticle>().Destroy_Particle(time_destroy_particle);
				
				// Start Delay Timer for Firing
				StartCoroutine( FireWeaponDelay() );
			}
			// If can't shoot and not reloading
			else if (!can_fire_weapon &&
					!reloading_weapon)
			{
				// Debug Text
				DebugText("Must Wait to Shoot");		
			}
		}
	}
	
	IEnumerator NoAmmoDelay()
	{
		// Lock gun so can delay
		no_ammuntion = true;
		
		// Set time delay counter
		float time = 0;
		while (time < 1)
		{
			time += Time.deltaTime / time_between_no_ammo;
			yield return null;
		}
		
		// Unlock gun
		no_ammuntion = false;
	}
	
	IEnumerator FireWeaponDelay()
	{
		// Lock shooting (unavailable)
		can_fire_weapon = false;
	
		// Sreps for light fade
		int lightsteps = 0;
		
		// Set time delay counter
		float time_shots = 0;		// time between shots
		float time_intensity = 0;	// time for light intensity
		while (time_shots < 1)
		{
			
			// If not on final phase for fading lights fade lights
			if (lightsteps != 2)
			{
				if (time_intensity < 1)
				{
					for (int i=0; i < shotgun_lights.Length; i++)
					{
						// Calulate time, make lights fliker within half the time of fire rate
						time_intensity += Time.deltaTime / (time_between_shots/2);
					
						// Fade liades from 0 - max
						if (lightsteps == 0)
							shotgun_lights[i].intensity = Mathf.SmoothStep(0, light_intesity_max, time_intensity);
						// Fade lights from max - 0
						else
							shotgun_lights[i].intensity = Mathf.SmoothStep(light_intesity_max, 0, time_intensity);
						
						yield return null;
					}
				}
				else
				{
					// Reset timer to zero and increase step counter
					time_intensity = 0;
					lightsteps++;
				}
			}
			
			// Calculate time delay between shots
			time_shots += Time.deltaTime / time_between_shots;
			yield return null;
		}
		
		for (int i=0; i < shotgun_lights.Length; i++)
		{
			shotgun_lights[i].intensity = 0;	
			yield return null;	
		}
		
		// Unlock shooting (can shoot again)
		can_fire_weapon = true;
	}

	#endregion
	
	#region Reload Weapon
	void ReloadWeapon ()
	{
		// if not reloading and ammo is not maxed... reload
		if (!reloading_weapon &&
			current_ammo != maximum_ammo)
		{
			// Debug Text
			DebugText("Reloading");
			
			// Start reloading weapon
			StartCoroutine( ReloadingWeaponDelay() ); 
		}
	}
	
	IEnumerator ReloadingWeaponDelay()
	{
		// Release trigger
		user_fired_weapon = false;
		
		// Lock gun so can reload
		reloading_weapon = true;
		
		while (current_ammo != maximum_ammo && !user_fired_weapon)
		{	
			// Time till next reload clip
			float time = 0;
			while (time < 1)
			{
				time += Time.deltaTime / time_between_reload;
				yield return null;
			}
			
			// Play shotgun reload audioclip
			audio_man.PlayAudioClip((int)AudioManager.SoundClips.SHOTGUN_RELOAD);
			
			// Increase Ammunition by one and update text
			current_ammo++;
			UpdateAmmunitionText();
			
			if (user_fired_weapon)
				break;
			
			yield return null;	
		}
		
		StartCoroutine( FinalReload() );
		
	}
	
	IEnumerator FinalReload()
	{
		
		// Time till next reload clip
		float time = 0;
		while (time < 1)
		{
			time += Time.deltaTime / time_between_reload;
			yield return null;
		}
		
		// Play shotgun final reload audioclip
		audio_man.PlayAudioClip((int)AudioManager.SoundClips.SHOTGUN_RELOAD_FINAL);
		
		// Time till weapon can shoot again
		time = 0;
		while (time < 1)
		{
			time += Time.deltaTime / time_final_reload;
			yield return null;
		}
		
		// Unlock gun so can shoot
		reloading_weapon = false;
	}
	#endregion
	
	void UpdateAmmunitionText()
	{
		text_ammo.text = (current_ammo.ToString() + "/" + maximum_ammo.ToString());
	}
	
	// When App is interrupted, ex. Home button
	void OnApplicationPause()
	{
		debug_text.text = "RESET";
	}
	
	void DebugText(string text)
	{
		#if UNITY_EDITOR
				debug_text.text = text;
		#endif	
	}
	
	void Vibrate()
	{
		//using (AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) 
		using (AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.Ameropel.ShootEmUp")) 
	    {
	        using (AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) 
	        {
	            obj_Activity.CallStatic("Vibrator", "vibrate");
	        }
		}
	}
}

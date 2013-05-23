using UnityEngine;
using System.Collections;

public class Vibrator : MonoBehaviour 
{
	private AndroidJavaObject vibrator;
 
	void Start()
	{
//		currentActivity as AndroidJavaObject = unityPlayer.GetStatic[of AndroidJavaObject]("currentActivity")
//		viblator = currentActivity.Call[of AndroidJavaObject]("getSystemService","vibrator")
		
		
		//AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		//AndroidJavaObject currentActivity = new AndroidJavaObject(unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"));
		
		AndroidJavaClass unityPlayer = new AndroidJavaClass("com.Ameropel.ShootEmUp");
        var currentActivity = unityPlayer.Get<AndroidJavaObject>("currentActivity");
		
		//vibrator = currentActivity.Call("getSystemService","vibrator") as AndroidJavaObject;
	}

	void Vibator(long time)
	{
		vibrator.Call("vibrate", time);
	}
}

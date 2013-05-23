using UnityEngine;
using System.Collections;

public class DestroyParticle : MonoBehaviour 
{
	public void Destroy_Particle(float time)
	{
		Destroy(gameObject, time);
	}
}

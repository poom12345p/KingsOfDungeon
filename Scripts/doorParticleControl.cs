using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorParticleControl : MonoBehaviour {
	public ParticleSystem[] particle;
	public GameObject pair;
	// Use this for initialization
	void Start () {
		if (!pair.activeSelf)
		{
			this.gameObject.SetActive(false);
		}
		foreach (ParticleSystem p in particle)
		{
			p.Stop();
		}
	}
	public void startparticle()
	{
		foreach (ParticleSystem p in particle)
		{
			p.Play();
		}
	}
	public void stopparticle()
	{
		foreach (ParticleSystem p in particle)
		{
			p.Stop();
		}
	}
}

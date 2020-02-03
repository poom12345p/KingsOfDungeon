using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fogControl : MonoBehaviour {
	public ParticleSystem[] fog;
	public Collider hitbox;
	public bool visited = false;
	void Start()
	{
		hitbox = GetComponent<Collider>();
		hitbox.isTrigger = true;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player" && !visited)
		{
			visited = true;
			foreach (ParticleSystem f in fog) {
				f.Stop();

			}
		}
	}
}

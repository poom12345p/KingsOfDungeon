using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particleControl : MonoBehaviour
{
    public ParticleSystem particle;
    // Use this for initialization
    void Start()
    {
        Destroy(gameObject, particle.main.startLifetime.constant + 0.5f);
    }
    public void rfDestroy()
    {
        Destroy(particle.gameObject);
    }
    public void setParticle(Transform pos)
    {
        transform.position = pos.position;
        transform.position += new Vector3(0, 1, 0);
    }
}

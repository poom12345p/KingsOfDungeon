using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSoundManager : MonoBehaviour {

	public AudioSource audioSource;

	public AudioClip walkingClip;
	public AudioClip deadClip;

	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource>();
	}

	public void PlaySoundLoop(AudioClip clip){
		if(audioSource.isPlaying) audioSource.Stop();
		audioSource.loop = true;
		audioSource.clip = clip;
		audioSource.Play();
	}

	public void PlayWalkSound(){
		if(audioSource.clip == walkingClip && audioSource.isPlaying) return;
		PlaySoundLoop(walkingClip);
	}

	public void PlayDeadSound(){

	}

}

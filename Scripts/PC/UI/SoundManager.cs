using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public AudioSource LoopSource;
    public AudioSource sfxSource;
    public AudioClip menuBackground;

    private void Awake() {
        if(LoopSource == null)
        LoopSource = GetComponent<AudioSource>();
    }

	public void PlayBackgroundMusic(AudioClip music)
    {
        if(LoopSource.isPlaying) LoopSource.Stop();
        LoopSource.clip = music;
        LoopSource.Play();
    }

    public void PlaySFXOneShot(AudioClip clip){
        sfxSource.PlayOneShot(clip);
    }
	
}

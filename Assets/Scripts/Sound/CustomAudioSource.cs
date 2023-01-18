using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CustomAudioSource : Poolable
{
    private AudioSource source;
    private bool play;

    private void Update()
    {
        if (play && source.isPlaying == false)
        {
            source.Stop();
            SoundController.StopAudio(id, gameObject);
            play = false;
        }
    }

    public override bool MakePrefab(int id)
    {
        this.id = id;

        source = GetComponent<AudioSource>();
        source.volume = 1;
        source.loop = false;
        AudioClip clip = SoundManager.GetSound(id.ToString());
        if (clip == null) return false;
        source.clip = clip;
        source.Stop();
        return true;
    }

    private void OnEnable()
    {
        if (source != null)
        {
            source.Play();
            play = true;
        }
    }
}

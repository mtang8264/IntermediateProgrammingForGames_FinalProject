using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BreathingSoundController : MonoBehaviour
{
    public static BreathingSoundController instance;
    [SerializeField]
    List<AudioClip> inhalingClips, exhalingClips;
    AudioSource source;

    void Awake()
    {
        instance = this;
        source = GetComponent<AudioSource>();
    }

    void Update()
    {
        
    }

    public void StartInhaling()
    {
        if (source.isPlaying)
            source.Stop();
        int idx = Random.Range(0, inhalingClips.Count);
        source.clip = inhalingClips[idx];
        source.Play();
    }
    public void StartExhaling()
    {
        if (source.isPlaying)
            source.Stop();
        int idx = Random.Range(0, inhalingClips.Count);
        source.clip = exhalingClips[idx];
        source.Play();
    }
    public void StopInhaling()
    {
        if (source.isPlaying == false)
            return;
        if (inhalingClips.Contains(source.clip))
            source.Stop();
    }
    public void PauseExhaling()
    {
        if (source.isPlaying == false)
            return;
        if (exhalingClips.Contains(source.clip))
            source.Pause();
    }
    public void ResumeExhaling()
    {
        if (source.isPlaying)
            return;
        if (exhalingClips.Contains(source.clip))
            source.UnPause();
    }
}

using UnityEngine;
using System.Collections;

public class MusicLoop : MonoBehaviour
{
    public float loopLength; // should always be 32.7
    private float loopThreshold;
    private AudioSource audioSource;
    private AudioClip audioClip;

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioClip = audioSource.clip;
        loopThreshold = audioClip.length - 1;
    }

    public void Update()
    {
        if (audioSource.timeSamples >= loopThreshold * audioClip.frequency)
        {
            audioSource.timeSamples -= Mathf.RoundToInt(loopLength * audioClip.frequency);
        }
    }
}
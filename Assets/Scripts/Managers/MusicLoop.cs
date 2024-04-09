using UnityEngine;
using System.Collections;

public class MusicLoop : MonoBehaviour
{
    public float loopLength; // should always be 32.7 for scene 1 song // 188 for scene 2 
    private float loopThreshold;
    public AudioSource audioSource;
    private AudioClip audioClip;
    private float vol = 0.2f;

    public void Start()
    {
        audioClip = audioSource.clip;
        loopThreshold = audioClip.length - 1;
    }

    //public void Update()
    //{
    //    if (audioSource.timeSamples >= loopThreshold * audioClip.frequency)
    //    {
    //        audioSource.timeSamples -= Mathf.RoundToInt(loopLength * audioClip.frequency);
    //    }
    //}

    public void ChangeLoop(float num)
    {
        loopLength = num;
    }

    public void FadeIn()
    {
        StartCoroutine("FadeInIEnumerator");
    }

    private IEnumerator FadeInIEnumerator()
    {
        audioSource.volume = 0;
        audioSource.Play();
        while (audioSource.volume < vol)
        {
            audioSource.volume = audioSource.volume + 0.06f * Time.deltaTime;
            yield return null;
        }
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutIEnumerator());
    }

    public IEnumerator FadeOutIEnumerator()
    {
        StopCoroutine("FadeInIEnumerator");
        while (audioSource.volume > 0)
        {
            audioSource.volume = audioSource.volume - 0.06f * Time.deltaTime;
            yield return null;
        }
        audioSource.Stop();
    }
}
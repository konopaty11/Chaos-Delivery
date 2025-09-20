using System.Collections;
using UnityEngine;
using System;

public class CharacterVoiceControl : MonoBehaviour
{
    [SerializeField] PersonID author;
    [SerializeField] Clip[] clips;

    public PersonID Author => author;

    AudioSource audioSource;
    Coroutine waitCoroutine;

    void AudioSourceInit()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlayVoice(string id)
    {
        Clip clip = Array.Find(clips, v => v.ID == id);
        if (clip != null)
        {
            if (waitCoroutine != null)
            {
                StopCoroutine(waitCoroutine);
                audioSource.Stop();
            }

            AudioSourceInit();
            audioSource.PlayOneShot(clip.VoiceClip);
            waitCoroutine = StartCoroutine(WaitEndClip(clip.VoiceClip));
            
        }
    }

    IEnumerator WaitEndClip(AudioClip clip)
    {
        yield return new WaitForSeconds(clip.length);
        waitCoroutine = null;
    }
}

using UnityEngine;

[System.Serializable]
public class Clip
{
    [SerializeField] string id;
    [SerializeField] AudioClip voiceClip;

    public string ID => id;
    public AudioClip VoiceClip => voiceClip;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameEvent : MonoBehaviour
{
    public virtual int CountEvents { get; }

    protected GameObject player;

    Coroutine eventCoroutine;
    string playerTag = "Player";

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        player = other.gameObject;
        eventCoroutine = StartCoroutine(Event());
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag) || eventCoroutine == null) return;
        
        StopCoroutine(eventCoroutine);
    }

    public abstract IEnumerator Event(); 
}

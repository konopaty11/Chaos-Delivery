using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    [SerializeField] List<EventSerializable> events;

    [System.Serializable]
    class EventSerializable
    {
        public GameEvent gameEvent;
        public List<Transform> spawnPoints;
    }

    public static EventManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        SpawnEvent();
    }

    void SpawnEvent()
    {
        foreach (EventSerializable eventSerializable in events)
        {
            List<Transform> spawnPoints = eventSerializable.spawnPoints;
            GameEvent gameEvent = eventSerializable.gameEvent;

            for (int i = 0; i < gameEvent.CountEvents; i++)
            {
                if (spawnPoints.Count == 0) break;

                int index = Random.Range(0, spawnPoints.Count);
                Instantiate(gameEvent.gameObject, spawnPoints[index]);
                spawnPoints.RemoveAt(index);
            }
        }
    }
}

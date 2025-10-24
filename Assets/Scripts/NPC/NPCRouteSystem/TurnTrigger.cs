using System.Collections.Generic;
using UnityEngine;

public class TurnTrigger : MonoBehaviour
{
    [SerializeField] List<Transform> targetPoints;
    [SerializeField] float targetSpeed = 10f;

    [SerializeField] TrafficLight trafficLight;

    public float TargetSpeed => targetSpeed;
    public TrafficLight TrafficLight => trafficLight;

    public bool IsGreen()
    {
        if (trafficLight == null) return true;

        Debug.Log(trafficLight.IsGreen + " " + name);
        return trafficLight.IsGreen;
    }

    public Transform GetRandomTargetPoint()
    {
        return targetPoints[Random.Range(0, targetPoints.Count)];
    }

}

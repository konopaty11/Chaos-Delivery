using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NPCTransportController : MonoBehaviour
{
    [Header("Wheels")]
    [SerializeField] List<WheelCollider> wheelsF;
    [SerializeField] List<WheelCollider> wheelsR;

    [Header("Metrics")]
    [SerializeField] float motorToque = 1200f;
    [SerializeField] float brakeToque = 15000f;
    [SerializeField] float steerAngle = 35f;

    void Start()
    {
        
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        foreach (WheelCollider wheel in wheelsF)
            wheel.motorTorque = motorToque;
    }
}

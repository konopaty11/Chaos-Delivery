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
    [SerializeField] float brakeToque = 1000f;
    [SerializeField] float steerAngle = 35f;

    [Header("Physic")]
    [SerializeField] Rigidbody rg;

    string turnTag = "Turn";
    string trafficLightTag = "Traffic Light";

    Vector3 targetPoint = Vector3.zero;
    float targetSpeed = 10f;

    void Start()
    {
        rg = GetComponent<Rigidbody>();
    }

    void Update()
    {
        MoveToTarget();
    }

    void MoveToTarget()
    {
        if (rg.linearVelocity.magnitude - targetSpeed > 2)
        {
            foreach (WheelCollider wheel in wheelsF)
            {
                wheel.motorTorque = 0f;
                wheel.brakeTorque = GetBreakToque();
            }

            foreach (WheelCollider wheel in wheelsR)
            {
                wheel.motorTorque = 0f;
                wheel.brakeTorque = GetBreakToque();
            }
        }
        else if (rg.linearVelocity.magnitude - targetSpeed > 0)
            foreach (WheelCollider wheel in wheelsF)
                wheel.motorTorque = 0f;
        else
        {
            foreach (WheelCollider wheel in wheelsF)
                wheel.brakeTorque = 0f;

            foreach (WheelCollider wheel in wheelsR)
            {
                wheel.brakeTorque = 0f;
                wheel.motorTorque = GetMotorToque();
            }
        }

        foreach (WheelCollider wheel in wheelsF)
            wheel.steerAngle = GetSteeringAngle();
    }

    float GetBreakToque()
    {
        return brakeToque * (rg.linearVelocity.magnitude - targetSpeed) / 10;
    }

    float GetMotorToque()
    {
        if (targetPoint == Vector3.zero)
            return motorToque;

        float _distance = Vector3.Distance(transform.position, targetPoint);
        return motorToque;
    }

    float GetSteeringAngle()
    {
        if (targetPoint == Vector3.zero)
            return 0f;

        Vector3 direction = (targetPoint - transform.position).normalized;
        float targetAngle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);

        return Mathf.Clamp(targetAngle, -steerAngle, steerAngle);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(turnTag) && !other.CompareTag(trafficLightTag)) return;

        TurnTrigger turnTrigger = other.GetComponent<TurnTrigger>();

        if (!turnTrigger.IsGreen())
        {
            Debug.Log("sdf");
            targetSpeed = 0f;
        }

        Transform targetTransform = turnTrigger.GetRandomTargetPoint();

        if (targetPoint == other.transform.position)
        {
            targetSpeed = 10f;
            targetPoint = Vector3.zero;
        }
        else if (targetPoint == Vector3.zero)
        {
            targetSpeed = turnTrigger.TargetSpeed;
            targetPoint = targetTransform.position;
        }


    }
}

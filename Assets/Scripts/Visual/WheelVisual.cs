using UnityEngine;

public class WheelVisual : MonoBehaviour
{
    [SerializeField] WheelCollider wheelCollider;

    void Update()
    {
        wheelCollider.GetWorldPose(out Vector3 wheelPos, out Quaternion wheelRot);
        transform.SetPositionAndRotation(wheelPos, wheelRot);
    }
}

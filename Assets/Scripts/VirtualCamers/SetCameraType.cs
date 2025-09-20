using UnityEngine;

public class SetCameraType : MonoBehaviour
{
    [SerializeField] CameraType type;
    public CameraType Type { get; private set; }

    void Awake()
    {
        Type = type;
    }
}

using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] List<CinemachineVirtualCamera> virtualCameras;

    public static CameraManager Instance { get; private set; }

    Dictionary<CameraType, int> virtualCamerasIndexHash = new();
    CinemachineVirtualCamera previousCamera;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void TransitionCamera(CameraType type)
    {
        CinemachineVirtualCamera virtualCamera = null;

        if (virtualCamerasIndexHash.ContainsKey(type))
            virtualCamera = virtualCameras[virtualCamerasIndexHash[type]];
        else
            for (int i = 0; i < virtualCameras.Count; i++)
            {
                if (virtualCameras[i].GetComponent<SetCameraType>().Type == type)
                {
                    virtualCamerasIndexHash[type] = i;
                    virtualCamera = virtualCameras[i];
                }
            }

        if (previousCamera != null)
            previousCamera.Priority = 0;
        if (virtualCamera != null)
            virtualCamera.Priority = 10;

        previousCamera = virtualCamera;
    }

    public void SetTransition(CinemachineBlendDefinition.Style style, float blendTime = 0f)
    {
        CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
        brain.m_DefaultBlend.m_Style = style;
        brain.m_DefaultBlend.m_Time = blendTime;
    }

    public CinemachineVirtualCamera GetVirtualCamera(CameraType type)
    {
        if (virtualCamerasIndexHash.ContainsKey(type)) 
            return virtualCameras[virtualCamerasIndexHash[type]];

        for (int i = 0; i < virtualCameras.Count; i++)
            if (virtualCameras[i].GetComponent<SetCameraType>().Type == type)
            {
                virtualCamerasIndexHash[type] = i;
                return virtualCameras[i];
            }

        throw new NullReferenceException("Не найдена камера");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class camInteracting : MonoBehaviour
{
    public CinemachineVirtualCamera vcam;
    private Transform defaultLookAt;
    private CinemachinePOV pov;
    void Awake()
    {
        pov = vcam.GetCinemachineComponent<CinemachinePOV>();
    }
    void Start()
    {
        defaultLookAt = vcam.LookAt;
    }
    public void FocusOnNPC(Transform npcTarget)
    {
        vcam.LookAt = npcTarget; // olha para o NPC
    }

    public void RestoreCamera()
    {
        vcam.LookAt = defaultLookAt;
    }
    public void LockCamera()
    {
        pov.m_HorizontalAxis.m_MaxSpeed = 0;
        pov.m_VerticalAxis.m_MaxSpeed = 0;
    }

    public void UnlockCamera()
    {
        pov.m_HorizontalAxis.m_MaxSpeed = 200; // valor original
        pov.m_VerticalAxis.m_MaxSpeed = 200;     // valor original
    }
}

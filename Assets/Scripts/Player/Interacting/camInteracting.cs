using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class camInteracting : MonoBehaviour
{
    public CinemachineVirtualCamera vcam;
    private Transform defaultLookAt;
    private CinemachinePOV pov;

    private float savedHorizontalSpeed;
    private float savedVerticalSpeed;

    void Awake()
    {
        pov = vcam.GetCinemachineComponent<CinemachinePOV>();
    }

    void Start()
    {
        defaultLookAt = vcam.LookAt;

        // Salva a sensibilidade real configurada
        savedHorizontalSpeed = pov.m_HorizontalAxis.m_MaxSpeed;
        savedVerticalSpeed = pov.m_VerticalAxis.m_MaxSpeed;
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
        // Restaura a sensibilidade configurada pelo player
        pov.m_HorizontalAxis.m_MaxSpeed = savedHorizontalSpeed;
        pov.m_VerticalAxis.m_MaxSpeed = savedVerticalSpeed;
    }

    public void UpdateSensitivity(float value)
    {
        // Atualiza e salva
        savedHorizontalSpeed = value;
        savedVerticalSpeed = value;

        // Aplica imediatamente (se a câmera não estiver travada)
        if (pov.m_HorizontalAxis.m_MaxSpeed > 0)
        {
            pov.m_HorizontalAxis.m_MaxSpeed = value;
            pov.m_VerticalAxis.m_MaxSpeed = value;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public Transform cam;

    void Awake()
    {
        cam = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; 
    }
    public void camMove()
    {
        Vector3 cameraForward = cam.forward;
        cameraForward.y = 0f; // remove inclinação vertical
        cameraForward.Normalize();

        // Rotaciona o player para onde a câmera está olhando (no plano XZ)
        transform.forward = cameraForward;
    }
}

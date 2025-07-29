using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractController : MonoBehaviour
{
    private Transform cam;
    private IInteractable lastInteractable;
    public float distanceDetect;
    public bool detectInteract;
    public LayerMask interactLayer;

    void Awake()
    {
        cam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Interact();
        Debug.DrawLine(cam.transform.position, cam.transform.position + cam.transform.forward * distanceDetect, Color.blue);
    }
    void Interact()
    {
        RaycastHit hitInfo;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hitInfo, distanceDetect, interactLayer))
        {
            IInteractable obj = hitInfo.transform.GetComponent<IInteractable>();
            if (lastInteractable != obj)
            {
                if (lastInteractable != null)
                {
                    lastInteractable.ShowTxt(false);
                }
                lastInteractable = obj;
            }
            detectInteract = true;
            obj.ShowTxt(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                obj.Interact();
            }
        }
        else
        {
            detectInteract = false;
            if (lastInteractable != null)
            {
                lastInteractable.ShowTxt(false);
                lastInteractable = null;
            }
        }
    }
}

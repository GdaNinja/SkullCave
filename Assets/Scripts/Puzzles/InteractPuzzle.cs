using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractPuzzle : MonoBehaviour, IExtendedInteractable
{
    public GameObject txtInteract;
    Vector3 mousePosition;
    void Start()
    {
        txtInteract.SetActive(false);
    }
    public void Interact()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
    public void ShowTxt(bool show)
    {
        txtInteract.SetActive(show);
    }
    private Vector3 GetMousePos()
    {
        return Camera.main.WorldToScreenPoint(transform.position);
    }
    void OnMouseDown()
    {
        mousePosition = Input.mousePosition - GetMousePos();
    }
    void OnMouseDrag()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePosition);
    }
}

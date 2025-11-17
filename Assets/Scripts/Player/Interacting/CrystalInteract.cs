using UnityEngine;

public class CrystalInteract : MonoBehaviour, IInteractable
{
    public GameObject textInteract;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textInteract.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Interact()
    {
        Destroy(gameObject);
    }
    public void ShowTxt(bool show)
    {
        textInteract.SetActive(show);
    }
}

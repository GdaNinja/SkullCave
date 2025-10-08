using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BateryController : MonoBehaviour, IInteractable
{
    public float distanceDetect;
    public GameObject txtColectBatery;
    private Transform flashLight;

    // Start is called before the first frame update
    void Awake()
    {
        flashLight = GameObject.FindGameObjectWithTag("Flashlight").transform;
    }
    void Start()
    {
        txtColectBatery.SetActive(false);
    }

    public void Interact()
    {
        ColectBatery();
    }
    public void ShowTxt(bool show)
    {
        txtColectBatery.SetActive(show);
    }
    public void ColectBatery()
    {
        txtColectBatery.SetActive(true);
        flashLight.GetComponent<FlashlightController>().addBatery();
        txtColectBatery.SetActive(false);
        Destroy(gameObject);
    }
}

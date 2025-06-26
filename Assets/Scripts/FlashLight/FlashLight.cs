using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FlashLight : MonoBehaviour
{
    bool isLight;
    void Start()
    {
        isLight = false;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        showLight();
    }
    public void showLight()
    {
        if (Input.GetMouseButtonDown(0) && isLight)
        {
            gameObject.SetActive(true);
        }
    }
}

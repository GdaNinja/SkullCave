using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BateryController : MonoBehaviour
{
    public float distanceDetect;
    public GameObject txtColect;
    private Transform flashLight;
    // Start is called before the first frame update
    void Start()
    {
        flashLight = GameObject.FindGameObjectWithTag("Flashlight").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(flashLight.position, transform.position) <= distanceDetect)
        {
            txtColect.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                flashLight.GetComponent<FlashlightController>().addBatery();
                txtColect.SetActive(false);
                Destroy(gameObject);
            }
        }
        else
        {
            txtColect.SetActive(false);
        }
    }
}

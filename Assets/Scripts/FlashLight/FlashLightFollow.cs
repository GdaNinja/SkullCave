using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLightFollow : MonoBehaviour
{
    public float spdRotation;
    [SerializeField] Transform target;
    // Start is called before the first frame update
    void Start()
    {
        target = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position;    
        transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, spdRotation * Time.deltaTime);
    }
}


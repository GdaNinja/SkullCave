using System.Collections;
using UnityEngine;
public class InstancePrefab : MonoBehaviour
{
    public GameObject prefab, smokeEffect; // Reference to the prefab to be instantiated
    public Transform position;
    public float delay = 2f; // Delay before instantiation
    public AudioSource soundEffect; // Audio source for sound effect
    private bool hasInstantiated = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        smokeEffect.SetActive(false);
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hasInstantiated = true;
            StartCoroutine(InstantiatePrefab());
        }
    }
    IEnumerator InstantiatePrefab()
    {
        soundEffect.Play();
        smokeEffect.SetActive(true);
        yield return new WaitForSeconds(delay);
        Instantiate(prefab, position.position, position.rotation);
    }
}

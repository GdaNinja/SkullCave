using UnityEngine;
using System.Collections;

public class textScene : MonoBehaviour
{
    public GameObject textObject;
    private float time = 5;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            textObject.SetActive(true);
            StartCoroutine(textDestroy());
        }
    }
    IEnumerator textDestroy()
    {
        yield return new WaitForSeconds(time);
        textObject.SetActive(false);
        Destroy(gameObject);
    }
}

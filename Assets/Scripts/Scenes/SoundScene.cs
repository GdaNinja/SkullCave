using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundScene : MonoBehaviour
{
    public AudioSource sound;
    private float time = 15;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            sound.Play();
            StartCoroutine(soundDestroy());
        }
    }
    IEnumerator soundDestroy()
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}

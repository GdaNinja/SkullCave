using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FimJogoComFumaça : MonoBehaviour
{
    public PlayerStateManager playerState; // Referência ao seu PlayerStateManager

    public GameObject crystal1, crystal2, crystal3, crystal4, raio1, raio2, raio3, raio4;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        crystal1.SetActive(false);
        crystal2.SetActive(false);
        crystal3.SetActive(false);
        crystal4.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(delaySmoke());
        }
    }
    IEnumerator delaySmoke()
    {
        raio1.SetActive(false);
        raio2.SetActive(false);
        raio3.SetActive(false);
        raio4.SetActive(false);

        yield return new WaitForSeconds(2f);
        crystal1.SetActive(true);
        yield return new WaitForSeconds(2f);
        crystal2.SetActive(true);
        yield return new WaitForSeconds(2f);
        crystal3.SetActive(true);
        yield return new WaitForSeconds(2f);
        crystal4.SetActive(true);
        
        yield return new WaitForSeconds(4f);
        raio1.SetActive(true);
        raio2.SetActive(true);
        raio3.SetActive(true);
        raio4.SetActive(true);
        StartCoroutine(ChangeSceneAfterDelay(5f));
    }
        public IEnumerator ChangeSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Cena2");
    }
}

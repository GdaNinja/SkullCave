using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class trocaCena : MonoBehaviour
{

    public IEnumerator ChangeSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Cena2");
    }
}

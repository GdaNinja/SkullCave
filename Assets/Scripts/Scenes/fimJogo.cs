using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class fimJogo : MonoBehaviour
{
public string nextSceneName = "MenuInicial"; // Coloque o nome da próxima cena aqui

    void Start()
    {
        StartCoroutine(ChangeScene());
    }

    IEnumerator ChangeScene()
    {
        yield return new WaitForSeconds(8f);
        SceneManager.LoadScene(nextSceneName);
    }
}


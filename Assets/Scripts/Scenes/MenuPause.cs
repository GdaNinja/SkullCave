using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuPause : MonoBehaviour
{
    public GameObject menuPause, menuOptions;
    bool isPaused = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        menuPause.SetActive(false);
        menuOptions.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }
    public void Options()
    {
        menuPause.SetActive(false);
        menuOptions.SetActive(true);
    }
    public void ReturnMenu()
    {
        menuPause.SetActive(true);
        menuOptions.SetActive(false);
    }
    public void Pause()
    {
        isPaused = true;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        Time.timeScale = 0f;
        menuPause.SetActive(true);
    }
    public void Resume()
    {
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        menuPause.SetActive(false);
    }
    public void MenuInicial()
    {
         Time.timeScale = 1f;
        SceneManager.LoadScene("MenuInicial");
    }

}

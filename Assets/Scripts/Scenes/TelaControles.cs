using UnityEngine;

public class TelaControles : MonoBehaviour
{
    public GameObject telaControles;
    public bool controlesAtivos = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controlesAtivos = true;
        telaControles.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (controlesAtivos)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;
        }
    }
    public void fecharControles()
    {
        controlesAtivos = false;
        telaControles.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}

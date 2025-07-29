using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject menuInitial, menuOptions;
    void Start()
    {
        menuOptions.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void NewGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
    public void LoadGame()
    {

    }
    public void Options()
    {
        menuInitial.SetActive(false);
        menuOptions.SetActive(true);
    }
    public void ReturnMenu()
    {
        menuInitial.SetActive(true);
        menuOptions.SetActive(false);
    }
    public void Exit()
    {
        Application.Quit();
    }
}

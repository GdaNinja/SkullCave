using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MenuPause : MonoBehaviour
{
    public GameObject menuPause, menuOptions;
    public Dropdown qualityDropdown, resolutionDropdown;
    public Slider sensibilitySlider, musicSlider, soundEffectsSlider;
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
    public void Salvar()
    {
        SaveConfigs();
        ReturnMenu();
    }
    private void SaveConfigs()
    {
        var configs = new ConfigModel()
        {
            quality = (Quality)qualityDropdown.value,

            sensibility = sensibilitySlider.value,
            music = musicSlider.value,
            soundEffects = soundEffectsSlider.value
        };
        var resolution = new Resolution();
        switch (resolutionDropdown.value)
        {
            case 0:
                resolution.width = 1280;
                resolution.height = 720;
                break;
            case 1:
                resolution.width = 1920;
                resolution.height = 1080;
                break;
            case 2:
                resolution.width = 2560;
                resolution.height = 1440;
                break;
            case 3:
                resolution.width = 3840;
                resolution.height = 2160;
                break;
        }
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

using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("UI")]
    public Slider sensSlider;
    public Slider volumeSlider;

    [Header("Referências")]
    public camInteracting camScript;
    public AudioMixer mixer;

    void Start()
    {
        // Carrega se existir
        sensSlider.value = PlayerPrefs.GetFloat("sens", 500);
        volumeSlider.value = PlayerPrefs.GetFloat("volume", 0);

        ApplySensitivity(sensSlider.value);
        ApplyVolume(volumeSlider.value);

        sensSlider.onValueChanged.AddListener(ApplySensitivity);
        volumeSlider.onValueChanged.AddListener(ApplyVolume);
    }

    public void ApplySensitivity(float value)
    {
        camScript.UpdateSensitivity(value);
        PlayerPrefs.SetFloat("sens", value);
    }

    public void ApplyVolume(float value)
    {
        mixer.SetFloat("MasterVolume", value);
        PlayerPrefs.SetFloat("volume", value);
    }
}

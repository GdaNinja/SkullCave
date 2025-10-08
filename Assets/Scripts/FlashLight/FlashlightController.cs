using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FlashlightController : MonoBehaviour
{
    public FlashLightManager getFlashlight;
    PlayerStateManager player;
    public AudioSource turnSound;
    public GameObject Bar;
    public Image energyBarFill;
    public Image energyBarBorder;
    private Light lightController;

    [SerializeField] float delay; // Tempo entre ligar/desligar
    [SerializeField] float addValue;
    [SerializeField] bool canToggle = true; // Impede múltiplos cliques rápidos
    [SerializeField] bool energyBarActive;
    [SerializeField] Color standardColor = new Color(1, 1, 1, 1);
    [SerializeField] Color lowEnergyColor = new Color(1, 0, 0, 1);

    bool isFlickering = false; // impede múltiplos piscas
    float normalIntensity;

    void Awake()
    {
        lightController = GetComponentInChildren<Light>();
        player = FindFirstObjectByType<PlayerStateManager>();
    }

    void Start()
    {
        lightController.enabled = false;
        Bar.SetActive(false);
        normalIntensity = lightController.intensity;
    }

    void Update()
    {
        HasEnergy();

        if (Input.GetMouseButtonDown(0) && canToggle)
        {
            showEnergyBar(true);
            turnSound.Play();
            if (HasEnergy()) StartCoroutine(ToggleFlashlight());
        }

        StartCoroutine(EnergyFlashLight());
    }

    void showEnergyBar(bool show)
    {
        if (energyBarActive == show) return;
        energyBarActive = show;
        StartCoroutine(showEnergyUI(show));
    }

    public bool HasEnergy()
    {
        getFlashlight.hasEnergy = getFlashlight.energy > 0;
        return getFlashlight.hasEnergy;
    }

    IEnumerator showEnergyUI(bool show)
    {
        float duration = 1f;
        float time = 0f;
        float startAlpha = show ? 0f : 1f;
        float endAlpha = show ? 1f : 0f;

        if (show) Bar.SetActive(true);
        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, time / duration);
            energyBarFill.color = new Color(energyBarFill.color.r, energyBarFill.color.g, energyBarFill.color.b, alpha);
            energyBarBorder.color = new Color(energyBarBorder.color.r, energyBarBorder.color.g, energyBarBorder.color.b, alpha);
            yield return null;
        }

        if (!show) Bar.SetActive(false);
    }

    IEnumerator ToggleFlashlight()
    {
        canToggle = false;
        yield return new WaitForSeconds(0.4f);

        if (!player.isInteracting)
        {
            lightController.enabled = !lightController.enabled;
        }

        if (!lightController.enabled) showEnergyBar(false);

        yield return new WaitForSeconds(delay);
        canToggle = true;
    }

    IEnumerator EnergyFlashLight()
    {
        if (lightController.enabled)
        {
            getFlashlight.energy -= 0.25f * Time.deltaTime;

            // Cor da barra
            if (getFlashlight.energy <= 30)
            {
                energyBarFill.color = lowEnergyColor;
                energyBarBorder.color = lowEnergyColor;

                // começa a piscar quando está vermelha
                if (!isFlickering)
                    StartCoroutine(FlickerLight());
            }
            else
            {
                energyBarFill.color = standardColor;
                energyBarBorder.color = standardColor;
            }

            yield return null;

            // energia acabou
            if (getFlashlight.energy <= 0)
            {
                getFlashlight.energy = 0;
                getFlashlight.hasEnergy = false;
                lightController.enabled = false;
                showEnergyBar(false);
            }

            energyBarFill.fillAmount = getFlashlight.energy / getFlashlight.energyMax;
        }
    }

    IEnumerator FlickerLight()
    {
        isFlickering = true;

        // piscar mais rápido conforme a energia diminui
        while (lightController.enabled && getFlashlight.energy <= 30 && getFlashlight.energy > 0)
        {
            // intervalo baseado na porcentagem de energia restante
            float percent = getFlashlight.energy / 30f; // entre 0 e 1
            float offTime = Mathf.Lerp(0.02f, 0.1f, percent);  // quanto menor a energia, menor o tempo desligada
            float onTime = Mathf.Lerp(0.05f, 0.25f, percent);  // quanto menor a energia, menor o tempo ligada

            lightController.enabled = false;
            yield return new WaitForSeconds(offTime);
            lightController.enabled = true;
            yield return new WaitForSeconds(onTime);
        }

        isFlickering = false;
    }

    public void addBatery()
    {
        getFlashlight.energy += addValue;
        if (getFlashlight.energy >= getFlashlight.energyMax)
            getFlashlight.energy = getFlashlight.energyMax;

        energyBarFill.fillAmount = getFlashlight.energy / getFlashlight.energyMax;

        // se estava desligada por falta de energia, volta a funcionar normalmente
        if (getFlashlight.energy > 0 && !lightController.enabled && HasEnergy())
        {
            lightController.enabled = true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
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

    void Awake()
    {
        lightController = GetComponentInChildren<Light>();
        player = FindFirstObjectByType<PlayerStateManager>();
    }
    void Start()
    {
        lightController.enabled = false;
        Bar.SetActive(false);
    }

    void Update()
    {
        HasEnergy();
        if (Input.GetMouseButtonDown(0) && canToggle)
        {
            showEnergyBar(true);
            turnSound.Play(); // Toca o som primeiro
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
        float finalAlpha = endAlpha;
        energyBarFill.color = new Color(energyBarFill.color.r, energyBarFill.color.g, energyBarFill.color.b, finalAlpha);
        energyBarBorder.color = new Color(energyBarBorder.color.r, energyBarBorder.color.g, energyBarBorder.color.b, finalAlpha);
        if (!show) Bar.SetActive(false);

    }
    IEnumerator ToggleFlashlight()
    {
        canToggle = false; // Bloqueia novas trocas durante o delay
        yield return new WaitForSeconds(0.4f); // Espera o tempo definido
        if (!player.isInteracting)
        {
            lightController.enabled = !lightController.enabled; // Liga/desliga a luz
        }
        yield return new WaitForSeconds(delay); // Espera o tempo definido

        canToggle = true; // Libera novamente o clique
    }
    IEnumerator EnergyFlashLight()
    {
        if (lightController.enabled == true)
        {
            getFlashlight.energy -= .25f * Time.deltaTime;
            if (getFlashlight.energy <= 30)
            {
                energyBarFill.color = lowEnergyColor;
                energyBarBorder.color = lowEnergyColor;
            }
            else
            {
                energyBarFill.color = standardColor;
                energyBarBorder.color = standardColor;
            }
            yield return null; // Espera um frame pra voltar a agir
            if (getFlashlight.energy <= 0)
            {
                turnSound.Play(); // Toca o som primeiro
                getFlashlight.hasEnergy = false;
                getFlashlight.energy = 0;
                yield return new WaitForSeconds(delay);
                lightController.enabled = false;
            }
            if (getFlashlight.energy <= 0 || !lightController.enabled) showEnergyBar(false);

            energyBarFill.fillAmount = getFlashlight.energy / getFlashlight.energyMax;
            Debug.Log(getFlashlight.energy);
        }
    }
    public void addBatery()
    {
        getFlashlight.energy += addValue;
        if (getFlashlight.energy >= getFlashlight.energyMax) getFlashlight.energy = getFlashlight.energyMax;
        energyBarFill.fillAmount = getFlashlight.energy / getFlashlight.energyMax;
    }
}
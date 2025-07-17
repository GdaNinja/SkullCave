using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashlightController : MonoBehaviour
{
    public FlashLightManager getFlashlight;
    public AudioSource turnSound;
    public Image energyBar;
    [SerializeField] float delay = 1f; // Tempo entre ligar/desligar
    private Light lightController;
    [SerializeField] bool canToggle = true; // Impede múltiplos cliques rápidos

    void Start()
    {
        lightController = GetComponentInChildren<Light>();
        lightController.enabled = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canToggle)
        {
            turnSound.Play(); // Toca o som primeiro
            if (!getFlashlight.energyEnd)
            {
            StartCoroutine(ToggleFlashlight());
            }
        }
        EnergyFlashLight();
    }

    IEnumerator ToggleFlashlight()
    {
        canToggle = false; // Bloqueia novas trocas durante o delay

        lightController.enabled = !lightController.enabled; // Liga/desliga a luz
        yield return new WaitForSeconds(delay); // Espera o tempo definido

        canToggle = true; // Libera novamente o clique
    }
    void EnergyFlashLight()
    {
        if (lightController.enabled == true)
        {
            getFlashlight.energy -= .25f * Time.deltaTime;
            if (getFlashlight.energy <= 0)
            {
                getFlashlight.energyEnd = true;
                turnSound.Play(); // Toca o som primeiro
                getFlashlight.energy = 0;
                lightController.enabled = false;
            }
                
            energyBar.fillAmount = getFlashlight.energy / getFlashlight.energyMax;
            Debug.Log(getFlashlight.energy);
          
        }
    }
}
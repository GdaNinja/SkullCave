using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLightManager : MonoBehaviour
{
    public FlashLightSO flashlight;
    public float energy;
    public float energyMax;
    public bool hasEnergy;
    void Start()
    {
        energy = flashlight.energy;
        energyMax = flashlight.energyMax;
        hasEnergy = flashlight.hasEnergy;
    }

}

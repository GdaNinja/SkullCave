using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Itens/Flashlight")]
public class FlashLightSO : ScriptableObject
{
    public float energy;
    public float energyMax;
    public bool energyEnd;
}

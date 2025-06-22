using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Player", menuName ="Player")]
public class PlayerSO : ScriptableObject
{
    public float spd;
    public float runSpd;
    public float jumpForce;
    public float gravity;
    public float Hp;
    public float hpMax;
    public float stamina;
    public float staminaMax;
}

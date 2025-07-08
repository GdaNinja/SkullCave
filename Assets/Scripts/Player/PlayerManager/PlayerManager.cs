using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public PlayerSO player;
    public float spd;
    public float runSpd;
    public float crouchSpd;
    public float runCrounchSpd;
    public float jumpForce;
    public float gravity;
    public float Hp;
    public float hpMax;
    public float stamina;
    public float staminaMax;
    void Start()
    {
        spd = player.spd;
        runSpd = player.runSpd;
        crouchSpd = player.crouchSpd;
        runCrounchSpd = player.runCrounchSpd;
        jumpForce = player.jumpForce;
        gravity = player.gravity;
        Hp = player.Hp;
        hpMax = player.hpMax;
        stamina = player.stamina;
        staminaMax = player.staminaMax;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingState : PlayerBaseState
{
    public override PlayerState State => PlayerState.Jumping;
    public override void EnterState(PlayerStateManager player)
    {
        player.velocity.y = Mathf.Sqrt(player.getPlayer.jumpForce * -2f * player.getPlayer.gravity); // Calcula a for�a de pulo com base na f�rmula de f�sica: v = sqrt(2gh)
    }
    public override void UpdateState(PlayerStateManager player)
    {
        Vector2 input = player.InputMove;

        if (Input.GetKey(KeyCode.LeftShift) && player.CanRun())
        {
            player.ShowStaminaUI(true);
                player.getPlayer.stamina -= 10f * Time.deltaTime;
                if (player.getPlayer.stamina < 0) player.getPlayer.stamina = 0;
                player.UpdateStaminaBar();
            player.Move(input,player.getPlayer.runSpd);
        }
        else
        {
            player.StartRechargeStamina(); // inicia recarga
            player.Move(input,player.getPlayer.spd);
        }
        player.Gravity();
        player.CheckGround();
        if (player.isGrounded && player.velocity.y <= 0)
        {
            if (input == Vector2.zero)
            {
                player.SwitchState(player.idleState);
            }
            else if (Input.GetKey(KeyCode.LeftShift) && player.CanRun())
            {
                player.SwitchState(player.runningState);
            }
            else
            {
                player.SwitchState(player.walkingState);
            }
        }
    }
}

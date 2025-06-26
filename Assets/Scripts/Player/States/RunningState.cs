using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningState : PlayerBaseState
{
    public override PlayerState State => PlayerState.Running;
    public override void UpdateState(PlayerStateManager player)
    {
        Vector2 input = player.InputMove;
        if(!Input.GetKey(KeyCode.LeftShift) || player.getPlayer.stamina <= 0)
        {
            player.SwitchState(player.walkingState);
        } 
        if (input == Vector2.zero)
        {
            player.SwitchState(player.idleState);
            return;
        }
        player.ShowStaminaUI(true);
        player.getPlayer.stamina -= 10f * Time.deltaTime;
        player.UpdateStaminaBar();
        player.Move(input,player.getPlayer.runSpd);
        player.Gravity();
        player.CheckGround();

        if (Input.GetKeyDown(KeyCode.Space) && player.isGrounded)
        {
            player.SwitchState(player.jumpingState);
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            player.SwitchState(player.crouchingState);
            return;
        }
    }
    public override void ExitState(PlayerStateManager player)
    {
        player.StartRechargeStamina();
    }
}

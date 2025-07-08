using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : PlayerBaseState
{
    public override PlayerState State => PlayerState.Idle;
    public override void UpdateState(PlayerStateManager player)
    {
        Vector2 input = player.InputMove;
        player.Gravity();
        player.CheckGround();
        if (input != Vector2.zero)
        {
            player.SwitchState(player.walkingState);
        }
        if (Input.GetKey(KeyCode.LeftShift) && player.getPlayer.stamina > 0 && input != Vector2.zero)
        {
            player.SwitchState(player.runningState);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space) && player.isGrounded)
        {
            player.SwitchState(player.jumpingState);
            return;
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            player.SwitchState(player.crouchingState);
            return;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingState : PlayerBaseState
{
    public override PlayerState State => PlayerState.Walking;
    public override void UpdateState(PlayerStateManager player)
    {
        Vector2 input = player.InputMove;
        if (input == Vector2.zero )
        {
            player.SwitchState(player.idleState);
            return;
        }
        if (Input.GetKey(KeyCode.LeftShift) && player.CanRun())
        {
            player.SwitchState(player.runningState);
            return;
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            player.SwitchState(player.crouchingState);
            return;
        }
        
        player.Move(input, player.getPlayer.spd);
        player.Gravity();
        player.CheckGround();

        if (Input.GetKeyDown(KeyCode.Space) && player.isGrounded)
        {
            player.SwitchState(player.jumpingState);
            return;
        }
    }
}

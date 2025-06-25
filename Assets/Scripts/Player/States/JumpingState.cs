using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingState : PlayerBaseState
{
    public override PlayerState State => PlayerState.Jumping;
    public override void EnterState(PlayerStateManager player)
    {
        player.velocity.y = Mathf.Sqrt(player.getPlayer.jumpForce * -2f * player.getPlayer.gravity); // Calcula a força de pulo com base na fórmula de física: v = sqrt(2gh)
    }
    public override void UpdateState(PlayerStateManager player)
    {
        Vector2 input = player.InputMove;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            player.Move(input,player.getPlayer.runSpd);
        }
        else
        {
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
            else if (Input.GetKey(KeyCode.LeftShift) && player.getPlayer.stamina > 0)
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

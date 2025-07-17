using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningState : PlayerBaseState
{
    public override PlayerState State => PlayerState.Running;
    public override void EnterState(PlayerStateManager player)
    {
        if (!player.isRunning)
        {
            player.isRunning = true;
        }
    }
    public override void UpdateState(PlayerStateManager player)
    {
        Vector2 input = player.InputMove;
        if (!Input.GetKey(KeyCode.LeftShift) || player.CanRun())
        {
            player.SwitchState(player.walkingState);
        }
        if (input == Vector2.zero)
        {
            player.SwitchState(player.idleState);
            return;
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            player.SwitchState(player.crouchingState);
            return;
        }

        player.ShowStaminaUI(true);
        player.getPlayer.stamina -= 15f * Time.deltaTime;
        if (player.getPlayer.stamina <= 0)
            player.getPlayer.stamina = 0;
        player.UpdateStaminaBar();
        player.Move(input, player.getPlayer.runSpd);

        player.Gravity();
        player.CheckGround();

        if (Input.GetKeyDown(KeyCode.Space) && player.isGrounded)
        {
            player.SwitchState(player.jumpingState);
            return;
        }
    }
    public override void ExitState(PlayerStateManager player)
    {
        if (player.isRunning)
        {
            player.isRunning = false;
        player.StartRechargeStamina();
        }
    }
}

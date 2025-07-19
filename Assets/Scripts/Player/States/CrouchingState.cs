using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CrouchingState : PlayerBaseState
{
    public override PlayerState State => PlayerState.Crounching;
    public override void EnterState(PlayerStateManager player)
    {
        if (!player.isCrouching)
        {
            player.isCrouching = true;
            Vector3 scale = player.playerPivot.localScale;
            scale.y *= .5f;
            player.playerPivot.localScale = scale;
        }
    }
    public override void UpdateState(PlayerStateManager player)
    {
        Vector2 input = player.InputMove;
        // Sai do agachado ao apertar LeftControl
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (input == Vector2.zero)
            {
                player.SwitchState(player.idleState);
            }
            else if (player.CanRun())
            {
                player.SwitchState(player.runningState);
            }
            else
            {
                player.SwitchState(player.walkingState);
            }
            return;
        }

        // Movimentação agachada
        if (input != Vector2.zero)
        {
            if (player.CanRun())
            {
                player.ShowStaminaUI(true);
                player.getPlayer.stamina -= 15f * Time.deltaTime;
                if (player.getPlayer.stamina < 0) player.getPlayer.stamina = 0;
                player.UpdateStaminaBar();
                player.Move(input, player.getPlayer.runCrounchSpd);
            }
            else
            {
                player.StartRechargeStamina(); // inicia recarga
                player.Move(input, player.getPlayer.crouchSpd);
            }
        }
        else
        {
            player.StartRechargeStamina(); // parado: recarrega também
        }

        player.Gravity();
        player.CheckGround();
    }
    public override void ExitState(PlayerStateManager player)
    {
        if (player.isCrouching)
        {
            player.isCrouching = false;
            Vector3 scale = player.playerPivot.localScale;
            scale.y *= 2f;
            player.playerPivot.localScale = scale;
        }
    }
}

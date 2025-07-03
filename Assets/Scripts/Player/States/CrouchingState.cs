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
        player.Move(input,player.getPlayer.crouchSpd);
        player.Gravity();
        if (Input.GetKeyDown(KeyCode.LeftControl) && player.isCrouching && input == Vector2.zero)
        {
            player.SwitchState(player.idleState);
            return;
        }
        if (Input.GetKeyDown(KeyCode.LeftControl) && player.isCrouching && input != Vector2.zero)
        {
            player.SwitchState(player.walkingState);
            return;
        }
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractingState : PlayerBaseState
{
    public override PlayerState State => PlayerState.Interacting;
    public override void EnterState(PlayerStateManager player)
    {
        player.isInteracting = true;
        player.velocity = Vector3.zero;
        player.cam.cam.LookAt(player.npcTarget);
    }
    public override void UpdateState(PlayerStateManager player)
    {
        if (!player.isInteracting) player.SwitchState(player.idleState); 
    }
    public override void ExitState(PlayerStateManager player)
    {
        
    }
}

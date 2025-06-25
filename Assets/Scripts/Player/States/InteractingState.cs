using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractingState : PlayerBaseState
{
    public override PlayerState State => PlayerState.Interacting;
    public override void UpdateState(PlayerStateManager player)
    {

    }
}

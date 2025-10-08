using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBaseState 
{
    public abstract PlayerState State { get;}
    public virtual void EnterState(PlayerStateManager player)
    {
        Debug.Log("Entrou no estado: " + State.ToString());
    }
    public abstract void UpdateState(PlayerStateManager player);
    public virtual void ExitState(PlayerStateManager player)
    {
        Debug.Log("Saiu do estado: " + State.ToString());
    }
}

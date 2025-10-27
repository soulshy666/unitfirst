using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoraPatrolState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentenemy = enemy;
        Debug.Log("Ò°ÖíÑ²Âß×´Ì¬");
        currentenemy.currentSpeed = currentenemy.moveSpeed;
        currentenemy.canMove = true;
        currentenemy.currentStopTime = currentenemy.stopTime;
    }
    public override void LogicUpdate()
    {
        if (currentenemy.physicsCheck.CheckPlayer())
        {
            currentenemy.SwitchState(NPCState.Attack);
        }

        currentenemy.CheckWall();

    }
    public override void PhysicsUpdate()
    {

    }
    public override void OnExit()
    {
        currentenemy.lostTimeCounter = currentenemy.lostTime;
    }
}

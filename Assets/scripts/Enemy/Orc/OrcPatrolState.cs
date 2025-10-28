using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcPatrolState : BaseState
{
    

    public override void OnEnter(Enemy enemy)
    {
        currentenemy = enemy;
        currentenemy.canMove = true;
        currentenemy.currentStopTime = currentenemy.stopTime;
    }
    public override void LogicUpdate()
    {
        if (currentenemy.physicsCheck.CheckPlayer())
        {
            Debug.Log("������ң��л�׷��״̬");
            currentenemy.SwitchState(NPCState.Chase);
        }
        else if (currentenemy.physicsCheck.CheckBackPlayer())
        {
            if (Player.instance.isCrouching) { return; }
            if (Player.instance.crouchingAttack) { return; }
            Debug.Log("���������ң��л�׷��״̬");
            currentenemy.SwitchState(NPCState.Chase);
        }
        if (currentenemy.physicsCheck.IntAttack())
        {
            currentenemy.SwitchState(NPCState.Attack);
        }
        currentenemy.CheckWall();
    }
   

    public override void PhysicsUpdate()
    {
        currentenemy.currentSpeed = currentenemy.moveSpeed;
    } 
    public override void OnExit()
    {
        currentenemy.lostTimeCounter = currentenemy.lostTime;
    }
}

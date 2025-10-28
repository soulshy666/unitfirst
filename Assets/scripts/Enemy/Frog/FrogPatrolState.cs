using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogPatrolState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentenemy = enemy;
        Debug.Log("ÇàÍÜÑ²Âß×´Ì¬");
        currentenemy.canMove = true;
        currentenemy.currentStopTime = currentenemy.stopTime;
    }
    public override void LogicUpdate()
    {
        if (currentenemy.physicsCheck.CheckPlayer())
        {
            Debug.Log("·¢ÏÖÍæ¼Ò£¬ÇÐ»»×·»÷×´Ì¬");
            currentenemy.SwitchState(NPCState.Chase);
        }
        else if (currentenemy.physicsCheck.CheckBackPlayer())
        {
            if (Player.instance.isCrouching) { return; }
            if (Player.instance.crouchingAttack) { return; }
            Debug.Log("·¢ÏÖÉíºóÍæ¼Ò£¬ÇÐ»»×·»÷×´Ì¬");
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoraAttackState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        Debug.Log("Ò°Öí¹¥»÷×´Ì¬");
        currentenemy = enemy;
        currentenemy.currentSpeed = currentenemy.chaseSpeed;
        currentenemy.anim.SetBool("IsRun", true);
    }
    public override void LogicUpdate()
    {
        currentenemy.currentStopTime = 0;
        currentenemy.CheckWall();
        if (currentenemy.lostTimeCounter <= 0)
        {
            currentenemy.SwitchState(NPCState.Patrol);
        }
    }

    public override void PhysicsUpdate()
    {
    }
    public override void OnExit()
    {
        currentenemy.anim.SetBool("IsRun", false);
    }
}

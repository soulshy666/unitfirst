using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChameleonPatrolState : BaseState
{


    public override void OnEnter(Enemy enemy)
    {
        currentenemy = enemy;
        Debug.Log("变色龙巡逻状态");
        currentenemy.canMove = true;
        //Debug.Log("移动执行");
        currentenemy.currentStopTime = currentenemy.stopTime;
    }
    public override void LogicUpdate()
    {
        if (currentenemy.physicsCheck.CheckPlayer())
        {
            Debug.Log("发现玩家，切换追击状态");
            currentenemy.SwitchState(NPCState.Chase);
        }
        else if (currentenemy.physicsCheck.CheckBackPlayer())
        {
            if (Player.instance.isCrouching) { return; }
            if (Player.instance.crouchingAttack) { return; }
            Debug.Log("发现身后玩家，切换追击状态");
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

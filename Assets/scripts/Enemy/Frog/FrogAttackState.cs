using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class FrogAttackState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        Debug.Log("ÇàÍÜ¹¥»÷×´Ì¬");
        currentenemy = enemy;
        currentenemy.currentAttackDelay = 0;
        currentenemy.canMove =false;
    }
    public override void LogicUpdate()
    {
        if (currentenemy.currentAttackDelay <= 0)
        {
            currentenemy.currentAttackDelay = currentenemy.attackDelay;
            currentenemy.anim.SetTrigger("Attack");
        }
        currentenemy.currentAttackDelay -= Time.deltaTime;
        if (!currentenemy.physicsCheck.IntAttack() && !currentenemy.isAttack && currentenemy.currentAttackDelay <= 0)
        {
            currentenemy.SwitchState(NPCState.Chase);
        }

    }

    public override void PhysicsUpdate()
    {
        currentenemy.rb.velocity = Vector2.zero;
    }
    public override void OnExit()
    {
        currentenemy.canMove = true;
    }
}

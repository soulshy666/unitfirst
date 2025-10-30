using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcAttackState : BaseState
{
    public float attackCount;

    public override void OnEnter(Enemy enemy)
    {
        Debug.Log("¸ç²¼ÁÖ¹¥»÷×´Ì¬");
        currentenemy = enemy;
        currentenemy.currentAttackDelay = 0;
        currentenemy.canMove = false;
        
    }
    public override void LogicUpdate()
    {
        if (currentenemy.currentAttackDelay <= 0)
        {
            currentenemy.currentAttackDelay = currentenemy.attackDelay;
            if(attackCount<=1)
            {
                currentenemy.attack.begindamage = 10;
                currentenemy.anim.SetTrigger("Attack");
                attackCount++;
            }
            else
            {
                currentenemy.attack.begindamage = 20;
                currentenemy.anim.SetTrigger("Strike");
                attackCount = 0;
            }

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


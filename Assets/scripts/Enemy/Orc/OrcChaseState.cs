using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class OrcChaseState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {

        Debug.Log("哥布林追击状态");
        currentenemy = enemy;
        currentenemy.currentSpeed = currentenemy.chaseSpeed;
    } 
    public override void LogicUpdate()
    {
        currentenemy.currentStopTime = 0;
        if (currentenemy.lostTimeCounter <= 0)
        {
            currentenemy.SwitchState(NPCState.Patrol);
        }
        currentenemy.CheckWall();

        if (currentenemy.physicsCheck.IntAttack())
        {
            currentenemy.SwitchState(NPCState.Attack);
        }

    }
    public override void PhysicsUpdate()
    {
        // 获取当前敌人的PhysicsCheck组件（确保敌人对象上已挂载该组件)
        PhysicsCheck physicsCheck = currentenemy.GetComponent<PhysicsCheck>();
        if (physicsCheck == null)
        {
            Debug.LogError("敌人未挂载PhysicsCheck组件，无法获取intAttackOffset偏移量！");
            return;
        }

        // 计算适配朝向的攻击偏移量（与PhysicsCheck中偏移量处理逻辑保持一致）
        Vector2 scaledIntAttackOffset = new Vector2(
            physicsCheck.intAttackOffset.x * currentenemy.transform.localScale.x,
            physicsCheck.intAttackOffset.y
        );

        // 计算攻击范围参考位置（敌人位置 + 适配后的偏移量）
        Vector2 attackReferencePos = (Vector2)currentenemy.transform.position + scaledIntAttackOffset;

        // 基于参考位置判断玩家方位，设置敌人朝向（保留原地面检测条件）
        if ((currentenemy.shell != null && !currentenemy.shell.activeSelf)|| currentenemy.shell == null)
        {
            
            if (Player.instance.transform.position.x <= attackReferencePos.x && Player.instance.physicsCheck.Isground)
            {
                currentenemy.transform.localScale = new Vector2(1, 1);
            }
            else if (Player.instance.transform.position.x > attackReferencePos.x && Player.instance.physicsCheck.Isground)
            {
                currentenemy.transform.localScale = new Vector2(-1, 1);
            }
        }
        else if (currentenemy.shell != null && currentenemy.shell.activeSelf)
        {
            if (currentenemy.shell.transform.position.x <= attackReferencePos.x && Player.instance.physicsCheck.Isground)
            {
                currentenemy.transform.localScale = new Vector2(1, 1);
            }
            else if (currentenemy.shell.transform.position.x > attackReferencePos.x && Player.instance.physicsCheck.Isground)
            {
                currentenemy.transform.localScale = new Vector2(-1, 1);
            }
        }
    }
    public override void OnExit()
    {
    }
   
}

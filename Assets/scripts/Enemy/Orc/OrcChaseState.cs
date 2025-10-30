using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class OrcChaseState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {

        Debug.Log("�粼��׷��״̬");
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
        // ��ȡ��ǰ���˵�PhysicsCheck�����ȷ�����˶������ѹ��ظ����)
        PhysicsCheck physicsCheck = currentenemy.GetComponent<PhysicsCheck>();
        if (physicsCheck == null)
        {
            Debug.LogError("����δ����PhysicsCheck������޷���ȡintAttackOffsetƫ������");
            return;
        }

        // �������䳯��Ĺ���ƫ��������PhysicsCheck��ƫ���������߼�����һ�£�
        Vector2 scaledIntAttackOffset = new Vector2(
            physicsCheck.intAttackOffset.x * currentenemy.transform.localScale.x,
            physicsCheck.intAttackOffset.y
        );

        // ���㹥����Χ�ο�λ�ã�����λ�� + ������ƫ������
        Vector2 attackReferencePos = (Vector2)currentenemy.transform.position + scaledIntAttackOffset;

        // ���ڲο�λ���ж���ҷ�λ�����õ��˳��򣨱���ԭ������������
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

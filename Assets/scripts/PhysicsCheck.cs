using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    // ���������
    public bool Isground = true;
    public float checkdistance = 0.3f;
    public LayerMask groundLayer;
    public Vector2 groundOffset;
    [Header("ǽ��������")]
    // ǽ�ڼ�����
    public Vector2 wallOffset;
    public bool IsWall;

    // ��Ҽ����أ�ǰ����
    [Header("��Ҽ������")]
    public LayerMask playerLayer; // ������ڵĲ�
    public Vector2 playerCheckOffset; // ǰ����Ҽ���ƫ��
    public bool IsDetectPlayer; // �Ƿ��⵽ǰ�����


    // ������Ҽ�����
    [Header("������Ҽ������")]
    public Vector2 backPlayerCheckOffset; // ������Ҽ���ƫ��
    public bool IsDetectBackPlayer; // �Ƿ��⵽�������

    // ������������Χ��Ҽ�����
    [Header("������Χ��Ҽ������")]
    public Vector2 intAttackOffset; // ������Χ����ƫ�ƣ��������������λ�ã�
    public bool IsIntAttackDetected; // �Ƿ��ڹ�����Χ�ڼ�⵽���


    void Update()
    {
        CheckGround();
        CheckPlayer();
        CheckBackPlayer();
        IntAttack();
        CheckWall();
    }

    // �����⣨���䳯�򣬷��ؼ������
    public bool CheckGround()
    {
        Vector2 scaledGroundOffset = new Vector2(groundOffset.x * transform.localScale.x, groundOffset.y);
        bool isGroundDetected = Physics2D.OverlapCircle((Vector2)transform.position + scaledGroundOffset, checkdistance, groundLayer);
        Isground = isGroundDetected; // ���¹�������
        return isGroundDetected; // ���ؼ����
    }

    // ��ǽ��⣨���䳯�򣬷��ؼ������
    public bool CheckWall()
    {
        Vector2 scaledLeftOffset = new Vector2(wallOffset.x * transform.localScale.x, wallOffset.y);
        bool isWallDetected = Physics2D.OverlapCircle((Vector2)transform.position + scaledLeftOffset, checkdistance, groundLayer);
        IsWall = isWallDetected; // ���¹�������
        return isWallDetected; // ���ؼ����
    }


    // ǰ����Ҽ�⣨���䳯������Tag�жϣ�
    public bool CheckPlayer()
    {
        Vector2 scaledOffset = new Vector2(playerCheckOffset.x * transform.localScale.x, playerCheckOffset.y);
        Vector2 checkPos = (Vector2)transform.position + scaledOffset;
        // ��ȡ��⵽����ײ��
        Collider2D detectedCollider = Physics2D.OverlapCircle(checkPos, checkdistance, playerLayer);

        if (detectedCollider != null)
        {
            // ��⵽����ʱ���ж�Tag�Ƿ���������ͬ
            if (detectedCollider.tag == gameObject.tag)
            {
                IsDetectPlayer = false;
                return false;
            }
            // Tag��ͬ����Ϊ��Ч���
            IsDetectPlayer = true;
            return true;
        }
        // δ��⵽����
        IsDetectPlayer = false;
        return false;
    }

    // ������Ҽ�⣨���䳯������Tag�жϣ�
    public bool CheckBackPlayer()
    {
        Vector2 scaledBackOffset = new Vector2(backPlayerCheckOffset.x * transform.localScale.x, backPlayerCheckOffset.y);
        Vector2 backCheckPos = (Vector2)transform.position + scaledBackOffset;
        // ��ȡ��⵽����ײ��
        Collider2D detectedCollider = Physics2D.OverlapCircle(backCheckPos, checkdistance, playerLayer);

        if (detectedCollider != null)
        {
            // ��⵽����ʱ���ж�Tag�Ƿ���������ͬ
            if (detectedCollider.tag == gameObject.tag)
            {
                IsDetectBackPlayer = false;
                return false;
            }
            // Tag��ͬ����Ϊ��Ч���
            IsDetectBackPlayer = true;
            return true;
        }
        // δ��⵽����
        IsDetectBackPlayer = false;
        return false;
    }

    // ������Χ��Ҽ�⺯�������䳯��
    // ������Χ��Ҽ�⺯�������䳯������Tag�жϣ�
    public bool IntAttack()
    {
        Vector2 scaledAttackOffset = new Vector2(intAttackOffset.x * transform.localScale.x, intAttackOffset.y);
        Vector2 attackCheckPos = (Vector2)transform.position + scaledAttackOffset;
        // ��ȡ��⵽����ײ��
        Collider2D detectedCollider = Physics2D.OverlapCircle(attackCheckPos, checkdistance, playerLayer);

        if (detectedCollider != null)
        {
            // ��⵽����ʱ���ж�Tag�Ƿ���������ͬ
            if (detectedCollider.tag == gameObject.tag)
            {
                IsIntAttackDetected = false;
                return false;
            }
            // Tag��ͬ����Ϊ��Ч���
            IsIntAttackDetected = true;
            return true;
        }
        // δ��⵽����
        IsIntAttackDetected = false;
        return false;
    }

    // ���ӻ����м�ⷶΧ
    private void OnDrawGizmosSelected()
    {
        // �����ⷶΧ����ɫ�����䳯��
        Gizmos.color = Color.green;
        Vector2 gizmoGroundOffset = new Vector2(groundOffset.x * transform.localScale.x, groundOffset.y);
        Gizmos.DrawWireSphere((Vector2)transform.position + gizmoGroundOffset, checkdistance);
        //ǽ���ⷶΧ����ɫ�����䳯��
        Gizmos.color = Color.blue;
        Vector2 gizmoLeftOffset = new Vector2(wallOffset.x * transform.localScale.x, wallOffset.y);
        Gizmos.DrawWireSphere((Vector2)transform.position + gizmoLeftOffset, checkdistance);

        // ǰ����Ҽ�ⷶΧ����ɫ��
        Gizmos.color = Color.yellow;
        Vector2 gizmoPlayerOffset = new Vector2(playerCheckOffset.x * transform.localScale.x, playerCheckOffset.y);
        Gizmos.DrawWireSphere((Vector2)transform.position + gizmoPlayerOffset, checkdistance);

        // ������Ҽ�ⷶΧ����ɫ��
        Gizmos.color = Color.black;
        Vector2 gizmoBackOffset = new Vector2(backPlayerCheckOffset.x * transform.localScale.x, backPlayerCheckOffset.y);
        Gizmos.DrawWireSphere((Vector2)transform.position + gizmoBackOffset, checkdistance);

        // ������Χ��⣨��ɫ��
        Gizmos.color = Color.red;
        Vector2 gizmoAttackOffset = new Vector2(intAttackOffset.x * transform.localScale.x, intAttackOffset.y);
        Gizmos.DrawWireSphere((Vector2)transform.position + gizmoAttackOffset, checkdistance);
    }
}
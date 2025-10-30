using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    // 地面检测相关
    public bool Isground = true;
    public float checkdistance = 0.3f;
    public LayerMask groundLayer;
    public Vector2 groundOffset;
    [Header("墙体检测设置")]
    // 墙壁检测相关
    public Vector2 wallOffset;
    public bool IsWall;

    // 玩家检测相关（前方）
    [Header("玩家检测设置")]
    public LayerMask playerLayer; // 玩家所在的层
    public Vector2 playerCheckOffset; // 前方玩家检测点偏移
    public bool IsDetectPlayer; // 是否检测到前方玩家


    // 背后玩家检测相关
    [Header("背后玩家检测设置")]
    public Vector2 backPlayerCheckOffset; // 背后玩家检测点偏移
    public bool IsDetectBackPlayer; // 是否检测到背后玩家

    // 新增：攻击范围玩家检测相关
    [Header("攻击范围玩家检测设置")]
    public Vector2 intAttackOffset; // 攻击范围检测点偏移（相对于物体自身位置）
    public bool IsIntAttackDetected; // 是否在攻击范围内检测到玩家


    void Update()
    {
        CheckGround();
        CheckPlayer();
        CheckBackPlayer();
        IntAttack();
        CheckWall();
    }

    // 地面检测（适配朝向，返回检测结果）
    public bool CheckGround()
    {
        Vector2 scaledGroundOffset = new Vector2(groundOffset.x * transform.localScale.x, groundOffset.y);
        bool isGroundDetected = Physics2D.OverlapCircle((Vector2)transform.position + scaledGroundOffset, checkdistance, groundLayer);
        Isground = isGroundDetected; // 更新公共变量
        return isGroundDetected; // 返回检测结果
    }

    // 左墙检测（适配朝向，返回检测结果）
    public bool CheckWall()
    {
        Vector2 scaledLeftOffset = new Vector2(wallOffset.x * transform.localScale.x, wallOffset.y);
        bool isWallDetected = Physics2D.OverlapCircle((Vector2)transform.position + scaledLeftOffset, checkdistance, groundLayer);
        IsWall = isWallDetected; // 更新公共变量
        return isWallDetected; // 返回检测结果
    }


    // 前方玩家检测（适配朝向，新增Tag判断）
    public bool CheckPlayer()
    {
        Vector2 scaledOffset = new Vector2(playerCheckOffset.x * transform.localScale.x, playerCheckOffset.y);
        Vector2 checkPos = (Vector2)transform.position + scaledOffset;
        // 获取检测到的碰撞体
        Collider2D detectedCollider = Physics2D.OverlapCircle(checkPos, checkdistance, playerLayer);

        if (detectedCollider != null)
        {
            // 检测到物体时，判断Tag是否与自身相同
            if (detectedCollider.tag == gameObject.tag)
            {
                IsDetectPlayer = false;
                return false;
            }
            // Tag不同则视为有效检测
            IsDetectPlayer = true;
            return true;
        }
        // 未检测到物体
        IsDetectPlayer = false;
        return false;
    }

    // 背后玩家检测（适配朝向，新增Tag判断）
    public bool CheckBackPlayer()
    {
        Vector2 scaledBackOffset = new Vector2(backPlayerCheckOffset.x * transform.localScale.x, backPlayerCheckOffset.y);
        Vector2 backCheckPos = (Vector2)transform.position + scaledBackOffset;
        // 获取检测到的碰撞体
        Collider2D detectedCollider = Physics2D.OverlapCircle(backCheckPos, checkdistance, playerLayer);

        if (detectedCollider != null)
        {
            // 检测到物体时，判断Tag是否与自身相同
            if (detectedCollider.tag == gameObject.tag)
            {
                IsDetectBackPlayer = false;
                return false;
            }
            // Tag不同则视为有效检测
            IsDetectBackPlayer = true;
            return true;
        }
        // 未检测到物体
        IsDetectBackPlayer = false;
        return false;
    }

    // 攻击范围玩家检测函数（适配朝向）
    // 攻击范围玩家检测函数（适配朝向，增加Tag判断）
    public bool IntAttack()
    {
        Vector2 scaledAttackOffset = new Vector2(intAttackOffset.x * transform.localScale.x, intAttackOffset.y);
        Vector2 attackCheckPos = (Vector2)transform.position + scaledAttackOffset;
        // 获取检测到的碰撞体
        Collider2D detectedCollider = Physics2D.OverlapCircle(attackCheckPos, checkdistance, playerLayer);

        if (detectedCollider != null)
        {
            // 检测到物体时，判断Tag是否与自身相同
            if (detectedCollider.tag == gameObject.tag)
            {
                IsIntAttackDetected = false;
                return false;
            }
            // Tag不同则视为有效检测
            IsIntAttackDetected = true;
            return true;
        }
        // 未检测到物体
        IsIntAttackDetected = false;
        return false;
    }

    // 可视化所有检测范围
    private void OnDrawGizmosSelected()
    {
        // 地面检测范围（绿色，适配朝向）
        Gizmos.color = Color.green;
        Vector2 gizmoGroundOffset = new Vector2(groundOffset.x * transform.localScale.x, groundOffset.y);
        Gizmos.DrawWireSphere((Vector2)transform.position + gizmoGroundOffset, checkdistance);
        //墙面检测范围（蓝色，适配朝向）
        Gizmos.color = Color.blue;
        Vector2 gizmoLeftOffset = new Vector2(wallOffset.x * transform.localScale.x, wallOffset.y);
        Gizmos.DrawWireSphere((Vector2)transform.position + gizmoLeftOffset, checkdistance);

        // 前方玩家检测范围（黄色）
        Gizmos.color = Color.yellow;
        Vector2 gizmoPlayerOffset = new Vector2(playerCheckOffset.x * transform.localScale.x, playerCheckOffset.y);
        Gizmos.DrawWireSphere((Vector2)transform.position + gizmoPlayerOffset, checkdistance);

        // 背后玩家检测范围（黑色）
        Gizmos.color = Color.black;
        Vector2 gizmoBackOffset = new Vector2(backPlayerCheckOffset.x * transform.localScale.x, backPlayerCheckOffset.y);
        Gizmos.DrawWireSphere((Vector2)transform.position + gizmoBackOffset, checkdistance);

        // 攻击范围检测（红色）
        Gizmos.color = Color.red;
        Vector2 gizmoAttackOffset = new Vector2(intAttackOffset.x * transform.localScale.x, intAttackOffset.y);
        Gizmos.DrawWireSphere((Vector2)transform.position + gizmoAttackOffset, checkdistance);
    }
}
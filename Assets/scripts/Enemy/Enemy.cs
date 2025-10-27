using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("状态机")]
    public  BaseState currentState;
    public BaseState patrolState;
    public BaseState chaseState;
    public BaseState deathState;
    public BaseState attackState;
    public BaseState hurtState;

    public Rigidbody2D rb;
    public Animator anim;
    public float moveSpeed;
    public float chaseSpeed =250;
    public float currentSpeed ;
    public float hurtForce =10f;
    public Vector3 faceDir;
    public PhysicsCheck physicsCheck;
    public float stopTime =2;
    public float currentStopTime;
    public bool canMove = true;
    public bool isHurt;
    public Transform attacker;
    [Header("敌人攻击")]
    public bool isAttack =false;//是否正在攻击，防止攻击的时候转向
    public float attackDelay =4f;//攻击间隔
    public float currentAttackDelay;
    
    public bool isDeath = false;
    public bool stop;
    public float lostTimeCounter;//转化为正常巡逻状态
    public float lostTime = 3f;


    // Start is called before the first frame update
    protected virtual void Awake()
    {
        currentAttackDelay = attackDelay;
        currentSpeed = moveSpeed;
        physicsCheck =GetComponent<PhysicsCheck>();
        lostTimeCounter = lostTime;
        currentStopTime = stopTime; 
        Character character = GetComponent<Character>();
    }

    public void OnEnable()
    {
        currentState = patrolState;
        currentState.OnEnter(this);
    }
    // Update is called once per frame
    void Update()
    {
        Back();
        currentState.LogicUpdate();
    }
    private void FixedUpdate()
    {
        faceDir.x= -Mathf.Sign(transform.localScale.x);
        if (canMove && !isHurt && !isDeath)
        {
            Move();
        }
        else if(stop)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
            currentState.PhysicsUpdate();
    }
    private void OnDisable()
    {
        currentState.OnExit();
    }
    public  virtual void Move()
    {
        rb.velocity =new Vector2(currentSpeed*faceDir.x,rb.velocity.y);
    }
    public void CheckWall()
    {
        float faceDir = Mathf.Sign(transform.localScale.x); // 确保方向始终是±1，不受缩放大小影响

        if (physicsCheck.CheckWall())
        {
            currentState = patrolState;//碰到墙壁强制切换巡逻状态,防止穿墙检测人物
            if (canMove)
                {
                    canMove = false;
                }
                // 停止状态下，持续减少计时
                currentStopTime -= Time.deltaTime;
                stop= true;
            // 计时结束，执行翻转
            if (currentStopTime <= 0)
            { 
                stop= false;
                transform.localScale = new Vector3(-faceDir, transform.localScale.y, transform.localScale.z);
                canMove = true; // 恢复移动
                currentStopTime = stopTime; // 重置计时器    
            }

        }
        if (!physicsCheck.CheckGround())
        {
            currentState = patrolState;
            // 计算当前朝向的方向向量（x为±1，基于localScale.x的符号；y固定为1）

            Vector2 dirVector = new Vector2(faceDir, 1);

            canMove = false; // 离开地面时禁止移动
            currentStopTime -= Time.deltaTime;
            stop = true;
            // 暂停时间结束后，翻转朝向并恢复移动
            if (currentStopTime <= 0)
            {
                stop= false;
                // 翻转朝向（修改localScale.x需要整体赋值，不能直接改成员）
                transform.localScale = new Vector3(-faceDir, transform.localScale.y, transform.localScale.z);
                // 重置暂停时间
                currentStopTime = stopTime;
                // 恢复移动能力
                canMove = true;
            }
        }
    }
    public void Back()//没发现玩家的话返回倒计时结束恢复巡逻状态,发现玩家重置计时器
    {
        if (!physicsCheck.CheckPlayer()&& lostTimeCounter > 0)
        {
            lostTimeCounter -= Time.deltaTime;
        }
        if(physicsCheck.CheckPlayer())//发现玩家，重置计时器
        {
            lostTimeCounter = lostTime;
        }
    }

    public void SwitchState(NPCState state) 
    { var newState = state switch 
       {
        NPCState.Patrol => patrolState, 
        NPCState.Chase => chaseState,
        NPCState.Death => deathState,
        NPCState.Attack => attackState,
           _ => null
        }; 
        currentState.OnExit(); 
        currentState = newState; 
        currentState.OnEnter(this); 
    }
    public void OnTakeDamage(Transform attackTrans)
    {
        attacker = attackTrans;
        rb.velocity = Vector2.zero;
        isHurt = true;
        anim.SetTrigger("IsHurt");
        Vector2 dir = new Vector2(transform.position.x - attackTrans.position.x, 0).normalized;
        rb.velocity = new Vector2(0, rb.velocity.y);
        StartCoroutine(OnHurt(dir,attacker));       
    }
    IEnumerator OnHurt(Vector2 dir ,Transform attackTrans)
    {
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.7f);
        //转身
        if (attackTrans.position.x - transform.position.x > 0)
            transform.localScale = new Vector3(-1, 1, 1);
        if (attackTrans.position.x - transform.position.x < 0)
            transform.localScale = new Vector3(1, 1, 1);
        isHurt = false;
        SwitchState(NPCState.Patrol);
    }
    public void DeathMove()
    {
        Vector2 dir = new Vector2(transform.position.x - Player.instance.transform.position.x, 0).normalized;
        rb.velocity = new Vector2(0, rb.velocity.y);
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
        canMove = false;
        stop = true;
    }

}



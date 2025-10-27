using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("״̬��")]
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
    [Header("���˹���")]
    public bool isAttack =false;//�Ƿ����ڹ�������ֹ������ʱ��ת��
    public float attackDelay =4f;//�������
    public float currentAttackDelay;
    
    public bool isDeath = false;
    public bool stop;
    public float lostTimeCounter;//ת��Ϊ����Ѳ��״̬
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
        float faceDir = Mathf.Sign(transform.localScale.x); // ȷ������ʼ���ǡ�1���������Ŵ�СӰ��

        if (physicsCheck.CheckWall())
        {
            currentState = patrolState;//����ǽ��ǿ���л�Ѳ��״̬,��ֹ��ǽ�������
            if (canMove)
                {
                    canMove = false;
                }
                // ֹͣ״̬�£��������ټ�ʱ
                currentStopTime -= Time.deltaTime;
                stop= true;
            // ��ʱ������ִ�з�ת
            if (currentStopTime <= 0)
            { 
                stop= false;
                transform.localScale = new Vector3(-faceDir, transform.localScale.y, transform.localScale.z);
                canMove = true; // �ָ��ƶ�
                currentStopTime = stopTime; // ���ü�ʱ��    
            }

        }
        if (!physicsCheck.CheckGround())
        {
            currentState = patrolState;
            // ���㵱ǰ����ķ���������xΪ��1������localScale.x�ķ��ţ�y�̶�Ϊ1��

            Vector2 dirVector = new Vector2(faceDir, 1);

            canMove = false; // �뿪����ʱ��ֹ�ƶ�
            currentStopTime -= Time.deltaTime;
            stop = true;
            // ��ͣʱ������󣬷�ת���򲢻ָ��ƶ�
            if (currentStopTime <= 0)
            {
                stop= false;
                // ��ת�����޸�localScale.x��Ҫ���帳ֵ������ֱ�Ӹĳ�Ա��
                transform.localScale = new Vector3(-faceDir, transform.localScale.y, transform.localScale.z);
                // ������ͣʱ��
                currentStopTime = stopTime;
                // �ָ��ƶ�����
                canMove = true;
            }
        }
    }
    public void Back()//û������ҵĻ����ص���ʱ�����ָ�Ѳ��״̬,����������ü�ʱ��
    {
        if (!physicsCheck.CheckPlayer()&& lostTimeCounter > 0)
        {
            lostTimeCounter -= Time.deltaTime;
        }
        if(physicsCheck.CheckPlayer())//������ң����ü�ʱ��
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
        //ת��
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



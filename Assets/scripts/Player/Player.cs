using System;
using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    public SpriteRenderer spriteRenderer;
    public Character character;
    public Ctrl input;
    public Vector2 inputDir;
    public Animator playerAnimation;
    public bool isLeft = false; 
    [Header("����������")]
    public GameObject AttackParent;
    [Header("��Ч")]
    public GameObject Desheffect;
    public GameObject jumpeffect;
    public GameObject jumpeffect2;
    public PhysicsCheck physicsCheck; 
    [Header("����ƶ�����")]
    public bool CanMove = true;
    public float currentSpeed;
    public float moveSpeed = 200.0f;
    public Rigidbody2D rb;
    float xMove = 0 ;
    [Header("�����Ծ����")]
    public float jumpForce = 16.5f;
    public bool isJump = false;
    [Header("���Dash����")]
    public float DeshForce = 10.0f;
    public bool isDash =false;
    public bool isAttack3=false;
    public bool isDeshRight = false;//��¼��̷���ʱ��������������Ӱ�ĳ���
    public float DeshSta = 70f;//�����������
    [Header("��ҷ���")]
    public bool isRoll = false;
    public float RoolSpeed = 800f;
    public float RollSta =70f;//������������
    [Header("��Ҷ��²���")]
    public bool isCrouching = false;
    public float crouchSpeed = 100f;
    public bool crouchingAttack = false;
    [Header("��Ҿٶܲ���")]
    public bool isShield = false;
    public float ShieldHurt=80;
    public bool shieldAttack=false;
    [Header("�����ǽ����")]
    public bool isWallMove = false;
    [Header("�ܵ��˺�")]
    public bool isGetHurt=false;
    public float beginhurtForce=5;
    public float currenthurtForce=5;
    public static Player instance;
    [Header("����")]
    public bool Isdeath =  false;
    [Header("����")]
    public bool isAttack = false;
    [Header("UI����")]
    public Canvas targetCanvas;
    [Header("���״̬")]
    public PlayerState State = PlayerState.isPlayer; // ʹ��ö����������ʾ���״̬
    [Header("Ұ����")]
    public bool isUp;
    public float uptime;//����ʱ��
    public bool isgo;
    public float BoraDeshSpeed = 700f;



    // ����bool���鲢��������������
    public enum PlayerState
    {
       isPlayer,
       isBora,
       isFrog
    }
    private void Awake()
    {
        instance = this;
        currentSpeed = moveSpeed;
        input = new Ctrl();
        input.Player.Jump.started += Jump;
        input.Player.Skill.started += Skill;
        input.Player.Skill.performed += OnSkillPerformed;
        input.Player.Skill.canceled += OnSkillCanceled; 
        input.Player.Attack.started += AttackCobe;
        input.Player.Change.started +=Change;
        input.Player.Rool.started += PlayerRoll;
        input.Player.Crouching.started += Crouching;
        input.Player.Shield.performed += Shield;
        input.Player.Shield.canceled += (context) => { isShield = false;if (!isGetHurt) { rb.velocity = Vector3.zero; } };//������Ǳ����˻��˵��Լ����ᱻ�������� 
        input.Player.WallMove.performed += WallMove;
        
        character = GetComponent<Character>();
    }
    
    void Start()
    {
        inputDir = Vector2.zero;
        rb.velocity = Vector2.zero;
    }
    // Update is called once per frame
    void Update()
    {
        if (physicsCheck.Isground)
        {
            isJump = false;
        }
        xMove = Mathf.Abs(inputDir.x) > 0.1f ? inputDir.x : 0;// �������������ƶ�����
        if (isUp)
        {
            uptime += Time.deltaTime;
        }
        inputDir = input.Player.Move.ReadValue<Vector2>();
        if (physicsCheck.CheckGround())
        {
            isWallMove = false;
            rb.gravityScale = 4;
        }
        if (!physicsCheck.IsWall)
        {
            isWallMove = false;
            rb.gravityScale = 4;
        }
        
    }
    private void FixedUpdate()
    {
        if(CanMove)
        {
            Move();
        }
        //StopDesh();
        if (isDash)
        {
            Desh();
            // ���ģ�����ڼ�����y�᲻������ֻ����ƽ�ƻ��½���
            if (rb.velocity.y > 0)
            {
                // ��y���������ϵ��ٶȣ�ǿ�����㣨��ֹ������
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }
        }
        if(isgo)
        {
          BoragoDesh();
        }
        if(isRoll)
        {
          Roll();
        }
        if (isDash)
        {
          Desh();
        }
    }
    #region �°����ϵͳִ�еĺ���
    private void Jump(InputAction.CallbackContext context)
    {
        if (isCrouching) { return; }
        if (isRoll) { return; }
        if (isShield) { return; }
        if (isJump && State == PlayerState.isFrog)
        {
            TwoJump();
        }
        StartCoroutine(OneJump());

    }

    IEnumerator OneJump()
    {
        if (physicsCheck.Isground)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            StartCoroutine(JumpEffect());
            yield return new WaitForSeconds(0.1f);
            isJump = true;
        }
    }
    private void TwoJump()
    {
        Player_FrogAnimation.instance.DoubleJump();
        rb.velocity = Vector2.zero;
        rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        isJump = false;
        StartCoroutine(JumpEffect2());
    }
    IEnumerator JumpEffect()
    {
        jumpeffect.SetActive(true);
        yield return new WaitForSeconds(0.4f);
        jumpeffect.SetActive(false);
    }
    IEnumerator JumpEffect2()
    {
        jumpeffect2.SetActive(true);
        yield return new WaitForSeconds(0.4f);
        jumpeffect2.SetActive(false);

    }


    private void Skill(InputAction.CallbackContext context)
    {
        if (isCrouching) { return; }
        switch (State)
        {
            case PlayerState.isPlayer:

                if (Bar.instance.CurrentStamina < DeshSta) { return; }
                if (isShield) { return; }

                if (isAttack3)
                {
                    character.skillinvulnerable = true;
                }
                else
                {
                    playerAnimation.SetBool("IsDesh", true);
                }
                CanMove = false;
                isDash = true;
                if (transform.localScale.x == 1)
                {
                    isDeshRight = true;
                }
                else if (transform.localScale.x == -1)
                {
                    isDeshRight = false;
                }
                Bar.instance.CurrentStamina -= DeshSta;
                break;
            default:
                break;
        }
    }
    private void OnSkillPerformed(InputAction.CallbackContext context)
    {
        switch (State)
        {
            case PlayerState.isBora:
                if (Mathf.Abs(inputDir.x) < 0.1f)
                {
                    CanMove = false;
                    isUp = true;
                    Debug.Log("Ұ������");
                }
                break;
            default:
                break;
        }
    }
    private void OnSkillCanceled(InputAction.CallbackContext context)
    {
        switch (State)
        {
            case PlayerState.isBora:
                if (uptime > 2)
                {
                    isgo = true;
                    isUp = false;
                    Debug.Log("Ұ����");
                    uptime = 0;
                }
                else if (uptime <= 2)
                {
                    CanMove = true;
                    isUp = false;
                    uptime = 0;
                }
                break;
            default:
                break;
        }
    }
    private void AttackCobe(InputAction.CallbackContext context)
    {
        // �ؼ��޸ģ�����״̬����������ִ�й���
        if (isCrouching)
        {
            Crouching(context); // ������������ȡ������״̬
            StartCoroutine(ExecuteAttackAfterStandUp()); // �ӳ�ִ�й�����ȷ��״̬�л����
            return; // �����ظ�ִ�й����߼�
        }
        if (isShield)
        {
            AttackParent.gameObject.SetActive(false);//�ܷ�ʱ�ر���ͨ������ײ
            PlayerAnimation.instance.PlayerAttack();
            shieldAttack = true;
            isAttack = true;
            return;
        }

        // ԭ������״̬�жϣ�������
        if (isRoll || isDash || isJump || Isdeath)
        {
            return;
        }
        switch (State)
        {
            case PlayerState.isPlayer:
                PlayerAnimation.instance.PlayerAttack();
                isAttack = true;
                break;
            case PlayerState.isBora:
                // ���ڴ����Ұ����̬�Ĺ����߼�
                break;
            case PlayerState.isFrog:
                Player_FrogAnimation.instance.PlayerAttack();
                isAttack = true;
                break;
            default:
                break;
        }
        // ����״̬��ֱ��ִ�й���

    }

    // ����Э�̣��ӳ�һִ֡�й�����ȷ������״̬��Ч
    private IEnumerator ExecuteAttackAfterStandUp()
    {
        isAttack3 = true;//�޵�״̬
        crouchingAttack = true;
        // �ȴ�һ֡��ȷ��Crouchingup�е�״̬�޸�ͬ������ǰ֡��
        yield return new WaitForSeconds(0.3f);

        // �ٴμ���Ƿ��ѳɹ�������ֹ�쳣�����
        if (!isCrouching && !isRoll && !isDash && !isJump && !Isdeath)
        {
            switch (State)
            {
                case PlayerState.isPlayer:
                    PlayerAnimation.instance.PlayerAttack();//��Ҷ���Ǳ�е����󹥻�
                    isAttack = true;
                    break;
                default:
                    break;
            }
        }
        yield return new WaitForSeconds(0.5f);
        crouchingAttack = false;
        isAttack3 = false;

    }

    private void Change(InputAction.CallbackContext context)
    {
        ToggleCanvas();
    }
    public void ToggleCanvas()
    {
        // ��黭���Ƿ����
        if (targetCanvas != null)
        {
            // �л�����״̬��ȡ����ǰ״̬��
            targetCanvas.gameObject.SetActive(!targetCanvas.gameObject.activeSelf);
        }
        else
        {
            Debug.LogError("Ŀ�껭�������ڣ��޷��л�״̬", this);
        }
    }
    private void PlayerRoll(InputAction.CallbackContext context)
    {
        if (isCrouching|| isWallMove) { return; }
        switch (State)
        {
            case PlayerState.isPlayer:
                if (isShield) { return; }
                if (!physicsCheck.Isground || Bar.instance.CurrentStamina < RollSta)
                {
                    return;
                }
                rb.velocity = Vector2.zero;
                isRoll = true;
                CanMove = false;
                Bar.instance.CurrentStamina -= RollSta;
                break;
            case PlayerState.isBora:

                break;
            default:
                break;
        }
    }
    private void Crouching(InputAction.CallbackContext context)
    {
        switch (State)
        {
            case PlayerState.isPlayer:
                if (!isRoll && !isAttack && !isDash && !isCrouching)
                {
                    isCrouching = true;
                    currentSpeed = crouchSpeed;
                }
                else if (isCrouching)
                {
                    isCrouching = false;
                    currentSpeed = moveSpeed;
                }
                break;
            default:
                break;
        }
    }
    private void Shield(InputAction.CallbackContext context)
    {
        if (Bar.instance.CurrentStamina <= ShieldHurt)
        {
            return;
        }

        switch (State)
        {
            case PlayerState.isPlayer:
                if (!isRoll && !isAttack && !isDash && !isCrouching)
                {
                    isShield = true;
                    CanMove = false;
                    if (!isGetHurt)
                    {
                        rb.velocity = Vector3.zero;
                    }

                }
                break;
            default:
                break;
        }
    }
    private void WallMove(InputAction.CallbackContext context)
    {
        if (physicsCheck.CheckGround())
        {
            return;
        }
        if (!physicsCheck.IsWall)
        {
            return;
        }
        switch (State)
        {
            case PlayerState.isPlayer:
                isWallMove = true;
                rb.gravityScale = 0.7f;
                rb.velocity = Vector2.zero;
                break;
            default:
                break;
        }
    }
    #endregion




    private void OnEnable()
    {
        input.Enable();//��������ϵͳ
    }
    private void OnDisable()
    {
        input.Disable();//����������������ϵͳ
    }

    public void Move()
    {
        rb.velocity = new Vector2(inputDir.x * Time.deltaTime * currentSpeed, rb.velocity.y);
        if (xMove < 0)
        {
            // �����ƶ�ʱ��x��������Ϊ-1����ת��
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            isLeft = true;
        }
        else if (xMove > 0)
        {
            // �����ƶ�ʱ��x��������Ϊ1����������
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            isLeft = false;
        }

    }    
    public void EndRunEffect()
    {
        Desheffect.gameObject.SetActive(false);
    }
    #region ����뷭����Ұ����
    public void BoragoDesh()
    {
        this.tag = "Player";
        character.skillinvulnerable = true;
        float dashDirection = transform.localScale.x > 0 ? 1 : -1;
        rb.velocity = new Vector2(dashDirection * Time.deltaTime * BoraDeshSpeed, rb.velocity.y);
        Invoke("EndBoraDesh", 1.5f);

    }
    public void EndBoraDesh()
    {
        isgo = false;
        CanMove = true;
        character.skillinvulnerable = false;
    }
    public void  Roll()
    {
        AttackParent.gameObject.SetActive(false);//����ʱ�رչ�����ײ
        character.skillinvulnerable = true;
        float dashDirection = transform.localScale.x > 0 ? 1 : -1;
        rb.velocity = new Vector2(dashDirection * Time.deltaTime * RoolSpeed, 0);
        Invoke("EndRoll", 0.4f);
        Desheffect.gameObject.SetActive(true);
        Invoke("EndRunEffect", 0.3f);
    }
    public void EndRoll()
    {
        isRoll = false;
        CanMove = true;
        character.skillinvulnerable = false;
        AttackParent.gameObject.SetActive(true);
    }

    private void Desh()
    {
        if(physicsCheck.IsWall && transform.localScale.x == -1)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        Desheffect.gameObject.SetActive(true);
        rb.gravityScale =0;
        Invoke("EndRunEffect", 0.2f);
        float dashDirection = transform.localScale.x > 0 ? 1 : -1;
        rb.AddForce(new Vector2(dashDirection * DeshForce, 0), ForceMode2D.Impulse);
        ShadowPool.instance.GetFormPool();
        Invoke("Enddesh", 0.15f);
    }
    public void Enddesh()
    {
        CanMove = true;
        isDash = false;
        playerAnimation.SetBool("IsDesh", false);
        rb.gravityScale = 4;

    }
#endregion
    public void GetHurt(Transform attacker)//�ܵ��˺�������
    {
        CanMove = false;//�ڶ������濪���ƶ�
        rb.velocity = Vector3.zero;
        Vector2 dir =new Vector2((transform.position.x- attacker.position.x),0).normalized;
        rb.AddForce(dir*currenthurtForce, ForceMode2D.Impulse);
        if(isShield && isGetHurt)
        {
            Invoke("StopHurtMove", 0.5f);
        }
    }
    public void StopHurtMove()//������һ���Ϳ��Կ�ʼ�ƶ�
    {
        rb.velocity= Vector3.zero;
        Player.instance.isGetHurt = false;
    }
    public void PlayeDead()
    {
        Debug.Log("��ɫ����");
        input.Player.Disable();
        if (State == PlayerState.isPlayer)
        {
            Isdeath = true;
        }
        else if (State == PlayerState.isBora)
        {
            ChangeForm.instance.ChangeToPlayer();
            Invoke("Dead", 0.4f);
        }
        else if (State == PlayerState.isFrog)
        {
            ChangeForm.instance.ChangeToPlayer();
            Invoke("Dead", 0.4f);
        }
    }
        public void Dead()
        {
         Isdeath = true;
        }

    public void GetDamage()//���Ŷ�Ӧ���˶���
    {
        if(isShield) {return;}
        if(State== PlayerState.isPlayer)
        {
            PlayerAnimation.instance.PlayerHurt();
        }
        else if(State == PlayerState.isBora)
        {
            Player_BoraAnimation.instance.PlayerHurt();
        }
        else if(State == PlayerState.isFrog)
        {
            Player_FrogAnimation.instance.PlayerHurt();
        }
    }
}

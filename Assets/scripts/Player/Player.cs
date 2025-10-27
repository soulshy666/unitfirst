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
    [Header("攻击父物体")]
    public GameObject AttackParent;
    [Header("特效")]
    public GameObject Desheffect;
    public GameObject jumpeffect;
    public GameObject jumpeffect2;
    public PhysicsCheck physicsCheck; 
    [Header("玩家移动参数")]
    public bool CanMove = true;
    public float currentSpeed;
    public float moveSpeed = 200.0f;
    public Rigidbody2D rb;
    float xMove = 0 ;
    [Header("玩家跳跃参数")]
    public float jumpForce = 16.5f;
    public bool isJump = false;
    [Header("玩家Dash参数")]
    public float DeshForce = 10.0f;
    public bool isDash =false;
    public bool isAttack3=false;
    public bool isDeshRight = false;//记录冲刺方向时朝向，用于生成虚影的朝向
    public float DeshSta = 70f;//冲刺消耗体力
    [Header("玩家翻滚")]
    public bool isRoll = false;
    public float RoolSpeed = 800f;
    public float RollSta =70f;//翻滚消耗体力
    [Header("玩家蹲下参数")]
    public bool isCrouching = false;
    public float crouchSpeed = 100f;
    public bool crouchingAttack = false;
    [Header("玩家举盾参数")]
    public bool isShield = false;
    public float ShieldHurt=80;
    public bool shieldAttack=false;
    [Header("玩家上墙参数")]
    public bool isWallMove = false;
    [Header("受到伤害")]
    public bool isGetHurt=false;
    public float beginhurtForce=5;
    public float currenthurtForce=5;
    public static Player instance;
    [Header("死亡")]
    public bool Isdeath =  false;
    [Header("攻击")]
    public bool isAttack = false;
    [Header("UI画布")]
    public Canvas targetCanvas;
    [Header("玩家状态")]
    public PlayerState State = PlayerState.isPlayer; // 使用枚举类型来表示玩家状态
    [Header("野猪冲刺")]
    public bool isUp;
    public float uptime;//蓄力时间
    public bool isgo;
    public float BoraDeshSpeed = 700f;



    // 创建bool数组并放入这两个参数
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
        input.Player.Shield.canceled += (context) => { isShield = false;if (!isGetHurt) { rb.velocity = Vector3.zero; } };//如果不是被敌人击退的自己不会被主动击退 
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
        xMove = Mathf.Abs(inputDir.x) > 0.1f ? inputDir.x : 0;// 处理输入人物移动方向
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
            // 核心：冲刺期间限制y轴不上升（只允许平移或下降）
            if (rb.velocity.y > 0)
            {
                // 若y方向有向上的速度，强制清零（禁止上升）
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
    #region 新版操作系统执行的函数
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
                    Debug.Log("野猪蓄力");
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
                    Debug.Log("野猪冲刺");
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
        // 关键修改：蹲下状态下先起身，再执行攻击
        if (isCrouching)
        {
            Crouching(context); // 调用起身函数，取消蹲下状态
            StartCoroutine(ExecuteAttackAfterStandUp()); // 延迟执行攻击，确保状态切换完成
            return; // 避免重复执行攻击逻辑
        }
        if (isShield)
        {
            AttackParent.gameObject.SetActive(false);//盾反时关闭普通攻击碰撞
            PlayerAnimation.instance.PlayerAttack();
            shieldAttack = true;
            isAttack = true;
            return;
        }

        // 原有其他状态判断（保留）
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
                // 可在此添加野猪形态的攻击逻辑
                break;
            case PlayerState.isFrog:
                Player_FrogAnimation.instance.PlayerAttack();
                isAttack = true;
                break;
            default:
                break;
        }
        // 正常状态下直接执行攻击

    }

    // 辅助协程：延迟一帧执行攻击，确保起身状态生效
    private IEnumerator ExecuteAttackAfterStandUp()
    {
        isAttack3 = true;//无敌状态
        crouchingAttack = true;
        // 等待一帧（确保Crouchingup中的状态修改同步到当前帧）
        yield return new WaitForSeconds(0.3f);

        // 再次检查是否已成功起身（防止异常情况）
        if (!isCrouching && !isRoll && !isDash && !isJump && !Isdeath)
        {
            switch (State)
            {
                case PlayerState.isPlayer:
                    PlayerAnimation.instance.PlayerAttack();//玩家蹲下潜行到背后攻击
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
        // 检查画布是否存在
        if (targetCanvas != null)
        {
            // 切换激活状态（取反当前状态）
            targetCanvas.gameObject.SetActive(!targetCanvas.gameObject.activeSelf);
        }
        else
        {
            Debug.LogError("目标画布不存在，无法切换状态", this);
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
        input.Enable();//启用输入系统
    }
    private void OnDisable()
    {
        input.Disable();//人物死亡禁用输入系统
    }

    public void Move()
    {
        rb.velocity = new Vector2(inputDir.x * Time.deltaTime * currentSpeed, rb.velocity.y);
        if (xMove < 0)
        {
            // 向左移动时，x轴缩放设为-1（翻转）
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            isLeft = true;
        }
        else if (xMove > 0)
        {
            // 向右移动时，x轴缩放设为1（正常方向）
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            isLeft = false;
        }

    }    
    public void EndRunEffect()
    {
        Desheffect.gameObject.SetActive(false);
    }
    #region 冲刺与翻滚，野猪冲刺
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
        AttackParent.gameObject.SetActive(false);//翻滚时关闭攻击碰撞
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
    public void GetHurt(Transform attacker)//受到伤害被击退
    {
        CanMove = false;//在动画里面开启移动
        rb.velocity = Vector3.zero;
        Vector2 dir =new Vector2((transform.position.x- attacker.position.x),0).normalized;
        rb.AddForce(dir*currenthurtForce, ForceMode2D.Impulse);
        if(isShield && isGetHurt)
        {
            Invoke("StopHurtMove", 0.5f);
        }
    }
    public void StopHurtMove()//被击退一点点就可以开始移动
    {
        rb.velocity= Vector3.zero;
        Player.instance.isGetHurt = false;
    }
    public void PlayeDead()
    {
        Debug.Log("角色死亡");
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

    public void GetDamage()//播放对应受伤动画
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

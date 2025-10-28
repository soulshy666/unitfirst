using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Player;

public class Character : MonoBehaviour
{

    // Start is called before the first frame update
    [Header("基本属性")]
    public float HealthMax;
    public float CurrentHealth;
    public bool skillinvulnerable = false;
    [Header("无敌帧")]
    public float invulnerableDuration = 1f;
    public float invulnerableCounter;
    public bool invulnerable;
    public UnityEvent<Transform> OnTakeDamage;
    public UnityEvent OnDead;
    public UnityEvent OnDamage;
    public Enemy enemy;
    [Header("冻结帧")]
    public float ffTimer, ffTimerTotal;
    public PhysicsCheck physicsCheck;
    [Header("悬崖死亡设置")]
    public float deathYThreshold = -20f; // 触发死亡的Y轴阈值，可在Inspector调整
    [Header("坠落死亡设置")]
    public float deathFallSpeed = -18f; // 向下的速度阈值（负号表示向下，值越小要求坠落速度越快）
    public bool isImmuneToFallDamage = false; // 是否免疫坠落伤害
    private bool hasCheckedFallDeath = false; // 防止同一落地动作多次触发死亡检测
    [SerializeField] private float fallDamageMultiplier = 2f; // 每单位超阈值速度的伤害系数
    public Rigidbody2D rb;
    private bool lastIsGround; // 记录上一帧是否在地面

    public Animator animator;
    public GameObject attackEffect;
    public bool crouchingAttack = false;
    void Start()
    {
        CurrentHealth = HealthMax;
        enemy = GetComponent<Enemy>();
        physicsCheck = GetComponent<PhysicsCheck>();
    }

    // Update is called once per frame
    void Update()
    {
        if (invulnerable)
        {
            invulnerableCounter -= Time.deltaTime;
            if (invulnerableCounter <= 0)
            {
                invulnerable = false;
            }
        }
        if (ffTimer > 0)
        {
            ffTimer -= Time.deltaTime;
            Time.timeScale = Mathf.Lerp(0f, 1f, (1 - (ffTimer / ffTimerTotal)));
        }
        else if (ffTimer <= 0 && enemy != null && enemy.isHurt)
        {
            Time.timeScale = 1f;
        }
        CheckFallDeath();
        CheckFall();
    }
    #region 坠落死亡检测（思路一：基于速度）
    /// <summary>
    /// 检测是否因高速坠落导致死亡
    /// </summary>
    private void CheckFall()
    {
        // 排除死亡、无敌、免疫坠落伤害的情况
        if ( skillinvulnerable || isImmuneToFallDamage)
        {
            hasCheckedFallDeath = false;
            return;
        }

        // 在空中时重置检测标记，准备落地时判断
        if (!physicsCheck.Isground)
        {
            hasCheckedFallDeath = false;
            // 调试信息：输出当前下落速度
            //Debug.Log($"[坠落检测] 空中下落速度：{rb.velocity.y:F2}", this);
            return;
        }

        // 落地且未检测过此次落地时，判断速度是否超过阈值
        if (physicsCheck.Isground && !lastIsGround && !hasCheckedFallDeath)
        {
            // 调试信息：输出落地瞬间速度和阈值
            //Debug.Log($"[坠落检测] 落地瞬间速度：{rb.velocity.y:F2}，死亡阈值：{deathFallSpeed}", this);
            if (rb.velocity.y < deathFallSpeed) // 例如：-15 < -10（下落更快，触发伤害）
            {
                // 1. 计算超出阈值的速度（因都是负值，直接用阈值减当前速度，得到正数的超出量）
                // 例：deathFallSpeed=-10，当前速度=-15 → 超出量 = (-10) - (-15) = 5
                float excessSpeed = deathFallSpeed - rb.velocity.y;
                // 确保超出量不为负（避免极端情况，如下落速度反而比阈值慢）
                excessSpeed = Mathf.Max(excessSpeed, 0);

                // 2. 根据超出速度计算伤害（速度越快，excessSpeed越大，伤害越高）
                float fallDamage = excessSpeed * fallDamageMultiplier;

                // 3. 扣血（保证生命值不低于0）
                CurrentHealth -= fallDamage;
                if(CurrentHealth > 0)
                {
                    Teiggerinvulnerable();
                    DamageNumber1.instance.ShowDamamgeNumber1(this, fallDamage);
                    // 调试信息：展示原始速度、超出量和伤害
                    Debug.LogWarning($"[坠落伤害] 下落速度：{rb.velocity.y:F2}，超出阈值速度：{excessSpeed:F2}，受到伤害：{fallDamage:F2}，剩余生命值：{CurrentHealth:F2}", this);
                }
                // 4. 若生命值归0，触发死亡逻辑
                else if (CurrentHealth <= 0)
                {
                    CurrentHealth = 0;
                    OnDead?.Invoke();
                    DamageNumber1.instance.ShowDamamgeNumber2(this, fallDamage);
                    ChangeCurrentObjectLayer();
                    Teiggerinvulnerable();
                }
            }
        }
        hasCheckedFallDeath = true; // 标记已检测，同一落地过程不再触发
    }
    #endregion
    private void CheckFallDeath()
    {
        // 当角色Y坐标低于阈值且尚未死亡时触发死亡
        if (transform.position.y < deathYThreshold && CurrentHealth > 0)
        {
            CurrentHealth = 0;
            OnDead?.Invoke();
            ChangeCurrentObjectLayer();
            Debug.Log($"{gameObject.name} 掉下悬崖死亡");

            // 如果是敌人，执行死亡击退效果
            if (enemy != null)
            {
                enemy.isDeath = true;
                enemy.DeathMove();
            }
        }
    }
    public void TakeDamage(Attack attacker)//普通的攻击
    {
        if (invulnerable || skillinvulnerable)
        {
            return;
        }
        // 新增：判断玩家是否面朝攻击者
        bool isFacingAttacker = false;
        if (Player.instance != null)
        {
            // 获取玩家朝向（scale.x为1时朝右，-1时朝左）
            float playerScaleX = Player.instance.transform.localScale.x;
            // 玩家与攻击者的X轴位置差
            float posXDiff = attacker.transform.position.x - Player.instance.transform.position.x;
            // 朝右时（scale.x=1），攻击者在右侧（posXDiff>0）则面朝；朝左时（scale.x=-1），攻击者在左侧（posXDiff<0）则面朝
            isFacingAttacker = (playerScaleX > 0 && posXDiff > 0) || (playerScaleX < 0 && posXDiff < 0);
        }

        // 原有条件基础上增加面朝攻击者的判断
        if (attacker.GetComponent<Player>() == null && Player.instance != null && Player.instance.isShield && isFacingAttacker)
        {
            // 定义标志变量，标记护盾是否失效（耐力不足）
            bool isShieldBroken = false;
            Bar.instance.CurrentStamina -= Player.instance.ShieldHurt;
            if (Bar.instance.CurrentStamina < 0)//破盾耐力不足
            {
                // 耐力不足，标记护盾失效
                isShieldBroken = true;
                // 避免耐力出现负数，强制设为0（可选，根据需求调整）
                Bar.instance.CurrentStamina = 0;
                Player.instance.isShield = false;
                if (!Player.instance.isGetHurt)
                {
                    Player.instance.rb.velocity = Vector3.zero;
                }
            }

            // 如果护盾未失效，才执行return退出当前逻辑；否则继续执行下方判断
            if (!isShieldBroken)
            {
                Player.instance.currenthurtForce = Player.instance.beginhurtForce - 3;//举着盾的时候受击力降低
                Player.instance.isGetHurt = true;
                Teiggerinvulnerable();
                DamageNumber1.instance.ShowDamamgeNumber1(this, 0);
                OnTakeDamage?.Invoke(attacker.transform);
                return;
            }
        }
        if (CurrentHealth - attacker.damage > 0)
        {
            Teiggerinvulnerable();
            CurrentHealth -= attacker.damage;
            DamageNumber1.instance.ShowDamamgeNumber1(this, attacker.damage);
            Player.instance.currenthurtForce = Player.instance.beginhurtForce;
            OnTakeDamage?.Invoke(attacker.transform);
            if (enemy != null && Player.instance.State == PlayerState.isPlayer)//生成敌人受击特效，并且随机特效生成的方向
            {
                float randomZRotation = Random.Range(0f, 360f);
                Vector3 newRotation = attackEffect.transform.rotation.eulerAngles;
                newRotation.z = randomZRotation;
                attackEffect.transform.rotation = Quaternion.Euler(newRotation);
                attackEffect.SetActive(true);
                Invoke("OverAttackEffect", 0.3f);
            }

        }
        else if (CurrentHealth - attacker.damage <= 0)
        {
            Debug.Log("死亡");
            ChangeCurrentObjectLayer();
            CurrentHealth = 0;
            CurrentHealth -= attacker.damage;
            DamageNumber1.instance.ShowDamamgeNumber1(this, attacker.damage);
            OnDead?.Invoke();
            if (enemy != null && Player.instance.State == PlayerState.isPlayer)
            {
                float randomZRotation = Random.Range(0f, 360f);
                Vector3 newRotation = attackEffect.transform.rotation.eulerAngles;
                newRotation.z = randomZRotation;
                attackEffect.transform.rotation = Quaternion.Euler(newRotation);
                attackEffect.SetActive(true);
                Invoke("OverAttackEffect", 0.3f);
            }
        }
    }


    public void TakeDamage2(Attack attacker)//带护盾受击，产生的是放大的暴击伤害数字
    {
        if (invulnerable || skillinvulnerable)
        {
            return;
        }
        // 新增：判断玩家是否面朝攻击者
        bool isFacingAttacker = false;
        if (Player.instance != null)
        {
            // 获取玩家朝向（scale.x为1时朝右，-1时朝左）
            float playerScaleX = Player.instance.transform.localScale.x;
            // 玩家与攻击者的X轴位置差
            float posXDiff = attacker.transform.position.x - Player.instance.transform.position.x;

            // 朝右时（scale.x=1），攻击者在右侧（posXDiff>0）则面朝；朝左时（scale.x=-1），攻击者在左侧（posXDiff<0）则面朝
            isFacingAttacker = (playerScaleX > 0 && posXDiff > 0) || (playerScaleX < 0 && posXDiff < 0);
        }

        // 原有条件基础上增加面朝攻击者的判断
        if (attacker.GetComponent<Player>() == null && Player.instance != null && Player.instance.isShield && isFacingAttacker)
        {
            // 定义标志变量，标记护盾是否失效（耐力不足）
            bool isShieldBroken = false;
            Bar.instance.CurrentStamina -= Player.instance.ShieldHurt;
            if (Bar.instance.CurrentStamina < 0)//破盾耐力不足
            {
                // 耐力不足，标记护盾失效
                isShieldBroken = true;
                // 避免耐力出现负数，强制设为0（可选，根据需求调整）
                Bar.instance.CurrentStamina = 0;
                Player.instance.isShield = false;
                if (!Player.instance.isGetHurt)
                {
                    Player.instance.rb.velocity = Vector3.zero;
                }
            }

            // 如果护盾未失效，才执行return退出当前逻辑；否则继续执行下方判断
            if (!isShieldBroken)
            {
                Player.instance.currenthurtForce = Player.instance.beginhurtForce - 3;//举着盾的时候受击力降低
                Player.instance.isGetHurt = true;
                Teiggerinvulnerable();
                DamageNumber1.instance.ShowDamamgeNumber1(this, 0);
                OnTakeDamage?.Invoke(attacker.transform);
                return;
            }
        }

        if (CurrentHealth - attacker.damage > 0)
        {
            Teiggerinvulnerable();
            CurrentHealth -= attacker.damage;
            DamageNumber1.instance.ShowDamamgeNumber2(this, attacker.damage);
            Player.instance.currenthurtForce = Player.instance.beginhurtForce;
            OnTakeDamage?.Invoke(attacker.transform);
            if (enemy != null && Player.instance.State == PlayerState.isPlayer)//生成敌人受击特效，并且随机特效生成的方向
            {
                float randomZRotation = Random.Range(0f, 360f);
                Vector3 newRotation = attackEffect.transform.rotation.eulerAngles;
                newRotation.z = randomZRotation;
                attackEffect.transform.rotation = Quaternion.Euler(newRotation);
                attackEffect.SetActive(true);
                Invoke("OverAttackEffect", 0.3f);
            }

        }
        else if (CurrentHealth - attacker.damage <= 0)
        {
            Debug.Log("死亡");
            ChangeCurrentObjectLayer();
            CurrentHealth = 0;
            CurrentHealth -= attacker.damage;
            DamageNumber1.instance.ShowDamamgeNumber2(this, attacker.damage);
            OnDead?.Invoke();
            if (enemy != null && Player.instance.State == PlayerState.isPlayer)
            {
                float randomZRotation = Random.Range(0f, 360f);
                Vector3 newRotation = attackEffect.transform.rotation.eulerAngles;
                newRotation.z = randomZRotation;
                attackEffect.transform.rotation = Quaternion.Euler(newRotation);
                attackEffect.SetActive(true);
                Invoke("OverAttackEffect", 0.3f);
            }
        }
    }
    public void TakeDamage3(Attack attacker)//背后突袭的伤害，产生的是暴击伤害数字
    {
        if (invulnerable || skillinvulnerable)
        {
            return;
        }
        if (CurrentHealth - attacker.damage > 0)
        {
            Teiggerinvulnerable();
            CurrentHealth -= attacker.damage;
            DamageNumber1.instance.ShowDamamgeNumber2(this, attacker.damage);
            Player.instance.currenthurtForce = Player.instance.beginhurtForce;
            OnTakeDamage?.Invoke(attacker.transform);
            if (enemy != null && Player.instance.State == PlayerState.isPlayer)//生成敌人受击特效，并且随机特效生成的方向
            {
                float randomZRotation = Random.Range(0f, 360f);
                Vector3 newRotation = attackEffect.transform.rotation.eulerAngles;
                newRotation.z = randomZRotation;
                attackEffect.transform.rotation = Quaternion.Euler(newRotation);
                attackEffect.SetActive(true);
                Invoke("OverAttackEffect", 0.3f);
            }

        }
        else if (CurrentHealth - attacker.damage <= 0)
        {
            Debug.Log("死亡");
            ChangeCurrentObjectLayer();
            Teiggerinvulnerable();
            CurrentHealth = 0;
            CurrentHealth -= attacker.damage;
            DamageNumber1.instance.ShowDamamgeNumber2(this, attacker.damage);
            OnDead?.Invoke();
            if (enemy != null && Player.instance.State == PlayerState.isPlayer)
            {
                float randomZRotation = Random.Range(0f, 360f);
                Vector3 newRotation = attackEffect.transform.rotation.eulerAngles;
                newRotation.z = randomZRotation;
                attackEffect.transform.rotation = Quaternion.Euler(newRotation);
                attackEffect.SetActive(true);
                Invoke("OverAttackEffect", 0.3f);
            }
        }
    }

    public void OverAttackEffect()
    {
        attackEffect.SetActive(false);
        SpriteRenderer effectRenderer = attackEffect.GetComponent<SpriteRenderer>();
        if (effectRenderer != null)
        {
            // 设置为白色（RGB均为1，透明度1）
            effectRenderer.color = Color.white;
        }

    }
    public  void Teiggerinvulnerable()
    {
        if (!invulnerable)
        {
            invulnerable = true;
            invulnerableCounter = invulnerableDuration;
        }
    }
    public void ChangeCurrentObjectLayer()//修改Layer为Death让敌人无法检测到玩家（防止触发多次DangerNumber）
    {
        this.gameObject.layer = 3;
    }
    public void CrouchingAttack()
    {
        crouchingAttack = true;
        // 若要延迟执行带参数的FrameFrozen，可改用协程
        StartCoroutine(DelayFrameFrozen(0.1f, 0.025f));
        OnDamage?.Invoke(); // 空安全调用，避免空引用异常
        SpriteRenderer effectRenderer = attackEffect.GetComponent<SpriteRenderer>();
        if (effectRenderer != null)
        {
            // 设置为白色（RGB均为1，透明度1）
            effectRenderer.color = Color.red;
        } 
    }

    // 协程实现延迟调用带参数的方法
    IEnumerator DelayFrameFrozen(float delay, float time)
    {
        yield return new WaitForSeconds(delay);
        FrameFrozen(time);
    }

    public void FrameFrozen(float time)
    {
        ffTimer = time;
        ffTimerTotal = time;
    }
}

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
    [Header("��������")]
    public float HealthMax;
    public float CurrentHealth;
    public bool skillinvulnerable = false;
    [Header("�޵�֡")]
    public float invulnerableDuration = 1f;
    public float invulnerableCounter;
    public bool invulnerable;
    public UnityEvent<Transform> OnTakeDamage;
    public UnityEvent OnDead;
    public UnityEvent OnDamage;
    public Enemy enemy;
    [Header("����֡")]
    public float ffTimer, ffTimerTotal;
    public PhysicsCheck physicsCheck;
    [Header("������������")]
    public float deathYThreshold = -20f; // ����������Y����ֵ������Inspector����
    [Header("׹����������")]
    public float deathFallSpeed = -18f; // ���µ��ٶ���ֵ�����ű�ʾ���£�ֵԽСҪ��׹���ٶ�Խ�죩
    public bool isImmuneToFallDamage = false; // �Ƿ�����׹���˺�
    private bool hasCheckedFallDeath = false; // ��ֹͬһ��ض�����δ����������
    [SerializeField] private float fallDamageMultiplier = 2f; // ÿ��λ����ֵ�ٶȵ��˺�ϵ��
    public Rigidbody2D rb;
    private bool lastIsGround; // ��¼��һ֡�Ƿ��ڵ���

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
    #region ׹��������⣨˼·һ�������ٶȣ�
    /// <summary>
    /// ����Ƿ������׹�䵼������
    /// </summary>
    private void CheckFall()
    {
        // �ų��������޵С�����׹���˺������
        if ( skillinvulnerable || isImmuneToFallDamage)
        {
            hasCheckedFallDeath = false;
            return;
        }

        // �ڿ���ʱ���ü���ǣ�׼�����ʱ�ж�
        if (!physicsCheck.Isground)
        {
            hasCheckedFallDeath = false;
            // ������Ϣ�������ǰ�����ٶ�
            //Debug.Log($"[׹����] ���������ٶȣ�{rb.velocity.y:F2}", this);
            return;
        }

        // �����δ�����˴����ʱ���ж��ٶ��Ƿ񳬹���ֵ
        if (physicsCheck.Isground && !lastIsGround && !hasCheckedFallDeath)
        {
            // ������Ϣ��������˲���ٶȺ���ֵ
            //Debug.Log($"[׹����] ���˲���ٶȣ�{rb.velocity.y:F2}��������ֵ��{deathFallSpeed}", this);
            if (rb.velocity.y < deathFallSpeed) // ���磺-15 < -10��������죬�����˺���
            {
                // 1. ���㳬����ֵ���ٶȣ����Ǹ�ֵ��ֱ������ֵ����ǰ�ٶȣ��õ������ĳ�������
                // ����deathFallSpeed=-10����ǰ�ٶ�=-15 �� ������ = (-10) - (-15) = 5
                float excessSpeed = deathFallSpeed - rb.velocity.y;
                // ȷ����������Ϊ�������⼫��������������ٶȷ�������ֵ����
                excessSpeed = Mathf.Max(excessSpeed, 0);

                // 2. ���ݳ����ٶȼ����˺����ٶ�Խ�죬excessSpeedԽ���˺�Խ�ߣ�
                float fallDamage = excessSpeed * fallDamageMultiplier;

                // 3. ��Ѫ����֤����ֵ������0��
                CurrentHealth -= fallDamage;
                if(CurrentHealth > 0)
                {
                    Teiggerinvulnerable();
                    DamageNumber1.instance.ShowDamamgeNumber1(this, fallDamage);
                    // ������Ϣ��չʾԭʼ�ٶȡ����������˺�
                    Debug.LogWarning($"[׹���˺�] �����ٶȣ�{rb.velocity.y:F2}��������ֵ�ٶȣ�{excessSpeed:F2}���ܵ��˺���{fallDamage:F2}��ʣ������ֵ��{CurrentHealth:F2}", this);
                }
                // 4. ������ֵ��0�����������߼�
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
        hasCheckedFallDeath = true; // ����Ѽ�⣬ͬһ��ع��̲��ٴ���
    }
    #endregion
    private void CheckFallDeath()
    {
        // ����ɫY���������ֵ����δ����ʱ��������
        if (transform.position.y < deathYThreshold && CurrentHealth > 0)
        {
            CurrentHealth = 0;
            OnDead?.Invoke();
            ChangeCurrentObjectLayer();
            Debug.Log($"{gameObject.name} ������������");

            // ����ǵ��ˣ�ִ����������Ч��
            if (enemy != null)
            {
                enemy.isDeath = true;
                enemy.DeathMove();
            }
        }
    }
    public void TakeDamage(Attack attacker)//��ͨ�Ĺ���
    {
        if (invulnerable || skillinvulnerable)
        {
            return;
        }
        // �������ж�����Ƿ��泯������
        bool isFacingAttacker = false;
        if (Player.instance != null)
        {
            // ��ȡ��ҳ���scale.xΪ1ʱ���ң�-1ʱ����
            float playerScaleX = Player.instance.transform.localScale.x;
            // ����빥���ߵ�X��λ�ò�
            float posXDiff = attacker.transform.position.x - Player.instance.transform.position.x;
            // ����ʱ��scale.x=1�������������ҲࣨposXDiff>0�����泯������ʱ��scale.x=-1��������������ࣨposXDiff<0�����泯
            isFacingAttacker = (playerScaleX > 0 && posXDiff > 0) || (playerScaleX < 0 && posXDiff < 0);
        }

        // ԭ�����������������泯�����ߵ��ж�
        if (attacker.GetComponent<Player>() == null && Player.instance != null && Player.instance.isShield && isFacingAttacker)
        {
            // �����־��������ǻ����Ƿ�ʧЧ���������㣩
            bool isShieldBroken = false;
            Bar.instance.CurrentStamina -= Player.instance.ShieldHurt;
            if (Bar.instance.CurrentStamina < 0)//�ƶ���������
            {
                // �������㣬��ǻ���ʧЧ
                isShieldBroken = true;
                // �����������ָ�����ǿ����Ϊ0����ѡ���������������
                Bar.instance.CurrentStamina = 0;
                Player.instance.isShield = false;
                if (!Player.instance.isGetHurt)
                {
                    Player.instance.rb.velocity = Vector3.zero;
                }
            }

            // �������δʧЧ����ִ��return�˳���ǰ�߼����������ִ���·��ж�
            if (!isShieldBroken)
            {
                Player.instance.currenthurtForce = Player.instance.beginhurtForce - 3;//���Ŷܵ�ʱ���ܻ�������
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
            if (enemy != null && Player.instance.State == PlayerState.isPlayer)//���ɵ����ܻ���Ч�����������Ч���ɵķ���
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
            Debug.Log("����");
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


    public void TakeDamage2(Attack attacker)//�������ܻ����������ǷŴ�ı����˺�����
    {
        if (invulnerable || skillinvulnerable)
        {
            return;
        }
        // �������ж�����Ƿ��泯������
        bool isFacingAttacker = false;
        if (Player.instance != null)
        {
            // ��ȡ��ҳ���scale.xΪ1ʱ���ң�-1ʱ����
            float playerScaleX = Player.instance.transform.localScale.x;
            // ����빥���ߵ�X��λ�ò�
            float posXDiff = attacker.transform.position.x - Player.instance.transform.position.x;

            // ����ʱ��scale.x=1�������������ҲࣨposXDiff>0�����泯������ʱ��scale.x=-1��������������ࣨposXDiff<0�����泯
            isFacingAttacker = (playerScaleX > 0 && posXDiff > 0) || (playerScaleX < 0 && posXDiff < 0);
        }

        // ԭ�����������������泯�����ߵ��ж�
        if (attacker.GetComponent<Player>() == null && Player.instance != null && Player.instance.isShield && isFacingAttacker)
        {
            // �����־��������ǻ����Ƿ�ʧЧ���������㣩
            bool isShieldBroken = false;
            Bar.instance.CurrentStamina -= Player.instance.ShieldHurt;
            if (Bar.instance.CurrentStamina < 0)//�ƶ���������
            {
                // �������㣬��ǻ���ʧЧ
                isShieldBroken = true;
                // �����������ָ�����ǿ����Ϊ0����ѡ���������������
                Bar.instance.CurrentStamina = 0;
                Player.instance.isShield = false;
                if (!Player.instance.isGetHurt)
                {
                    Player.instance.rb.velocity = Vector3.zero;
                }
            }

            // �������δʧЧ����ִ��return�˳���ǰ�߼����������ִ���·��ж�
            if (!isShieldBroken)
            {
                Player.instance.currenthurtForce = Player.instance.beginhurtForce - 3;//���Ŷܵ�ʱ���ܻ�������
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
            if (enemy != null && Player.instance.State == PlayerState.isPlayer)//���ɵ����ܻ���Ч�����������Ч���ɵķ���
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
            Debug.Log("����");
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
    public void TakeDamage3(Attack attacker)//����ͻϮ���˺����������Ǳ����˺�����
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
            if (enemy != null && Player.instance.State == PlayerState.isPlayer)//���ɵ����ܻ���Ч�����������Ч���ɵķ���
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
            Debug.Log("����");
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
            // ����Ϊ��ɫ��RGB��Ϊ1��͸����1��
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
    public void ChangeCurrentObjectLayer()//�޸�LayerΪDeath�õ����޷���⵽��ң���ֹ�������DangerNumber��
    {
        this.gameObject.layer = 3;
    }
    public void CrouchingAttack()
    {
        crouchingAttack = true;
        // ��Ҫ�ӳ�ִ�д�������FrameFrozen���ɸ���Э��
        StartCoroutine(DelayFrameFrozen(0.1f, 0.025f));
        OnDamage?.Invoke(); // �հ�ȫ���ã�����������쳣
        SpriteRenderer effectRenderer = attackEffect.GetComponent<SpriteRenderer>();
        if (effectRenderer != null)
        {
            // ����Ϊ��ɫ��RGB��Ϊ1��͸����1��
            effectRenderer.color = Color.red;
        } 
    }

    // Э��ʵ���ӳٵ��ô������ķ���
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

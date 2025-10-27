using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerAnimation : MonoBehaviour
{
    
    // Start is called before the first frame update
    public Animator animator;
    public Rigidbody2D rb;
    public PhysicsCheck physicsCheck;
    public static PlayerAnimation instance;
    public Character character;
    public GameObject attackGameobject;
    private void Awake()
    {
       instance = this;
    }
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetFloat("Run", 0); // 初始化跑步参数为0
    }

    // Update is called once per frame
    void Update()
    {
        SetAnimator();
    }
    public void SetAnimator()
    {
        animator.SetFloat("Run",Mathf.Abs(rb.velocity.x));
        animator.SetFloat("CrouchingRun", Mathf.Abs(rb.velocity.x));
        animator.SetBool("IsGround",physicsCheck.Isground);
        animator.SetBool("IsDeath",Player.instance.Isdeath );
        animator.SetBool("IsAttack",Player.instance.isAttack);
        animator.SetBool("IsRool", Player.instance.isRoll);
        animator.SetBool("CrouchingIdle", Player.instance.isCrouching);
        animator.SetBool("IsShield", Player.instance.isShield);
        animator.SetBool("IsWallside", Player.instance.isWallMove);

    }
    public void PlayerHurt()
    {
        animator.SetTrigger("IsHurt");
    }
    public void PlayerAttack()
    {
        animator.SetTrigger("Attack");
    }
    public void IsAttack3true()
    {
      Player.instance.isAttack3 = true;
    }
    public void IsAttack3false()
    {
        Player.instance.isAttack3 = false;
        character.skillinvulnerable = false;
    }

    public void EndShield()//在动画结束时调用,受盾后摇结束
    {
        Player.instance.isShield = false;
        Player.instance.CanMove = true;
    }
    
    public void EndShieldAttack()
    {
        Player.instance.CanMove = true;
        Player.instance.shieldAttack = false;
        Player.instance.AttackParent.gameObject.SetActive(true);

    }
    public void BeginShieldAttack()
    {
        Player.instance.isShield = false;
    }



}

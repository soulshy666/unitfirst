using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_FrogAnimation : MonoBehaviour
{
    public Animator animator;
    public static Player_FrogAnimation instance;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        instance = this;
    }
    void Start()
    {
        animator.SetFloat("Walk", 0); // 初始化跑步参数为0
    }

    // Update is called once per frame
    void Update()
    {
        SetAnimator();
    }
    public void SetAnimator()
    {
        animator.SetFloat("Walk", Mathf.Abs(Player.instance.rb.velocity.x));
        animator.SetBool("IsGround", Player.instance.physicsCheck.Isground);
    }
    public void PlayerHurt()
    {
        animator.SetTrigger("IsHurt");
    }
    public void DoubleJump()
    {
        animator.SetTrigger("DoubleJump");
    }
    public void PlayerAttack()
    {
        Player.instance.tag = "Player";
        animator.SetTrigger("Attack");
    }
}

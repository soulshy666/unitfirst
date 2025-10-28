using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_BatAnimation : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D rb;
    public static Player_BatAnimation instance;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        SetAnimator();
    }
    public void SetAnimator()
    {

    }
    public void PlayerHurt()
    {
        animator.SetTrigger("IsHurt");
    }
    public void PlayerAttack()
    {
        Player.instance.tag = "Player";
        animator.SetTrigger("Attack");
    }
    /*public void PlayerDead()
    {
        animator.SetBool("IsDeath", false);
    }*/
    public void DestroyAfterAnimation()//在死亡动画里调用
    {
        // 获取当前物体的父物体（使用空值传播运算符处理父物体为null的情况）
        GameObject parentObject = this.transform.parent?.gameObject;

        // 销毁当前物体
        Destroy(this.gameObject);

        // 如果父物体存在，销毁父物体
        if (parentObject != null)
        {
            Destroy(parentObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_BoraAnimation : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D rb;
    public static Player_BoraAnimation instance;
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
      animator.SetFloat("Run", Mathf.Abs(rb.velocity.x));
      animator.SetBool("IsDeath", Player.instance.Isdeath);
      animator.SetBool("IsUp", Player.instance.isUp);
      animator.SetBool("IsDesh", Player.instance.isgo);
    }
    public void PlayerHurt()
    {
      animator.SetTrigger("IsHurt");
    }
    public void PlayerDead()
    {
        animator.SetBool("IsDeath", false);
    }
    public void DestroyAfterAnimation()
    {
        Debug.Log("shanchu");
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

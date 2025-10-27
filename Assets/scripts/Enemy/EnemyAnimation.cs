using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator animator;
    public Enemy enemy;
    void Start()
    {
        animator =GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Idle();
    }
    public void Idle()
    {

       animator.SetBool("IsWalk", enemy.canMove);

    }
    public void Death()//在事件中调用
    {
        enemy.isDeath = true;
        animator.SetBool("IsDeath", true);
    }
    public void DestroyAfterAnimation()//放在死亡动画中，当作事件触发
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
    public void Hurt()//在怪物攻击画事件中调用
    {
        enemy.isAttack = true;
    }

    public void HurtEnd()//在怪物攻击动画事件中调用
    {
        enemy.isAttack = false;
    }
}

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
    public void DestroyAfterAnimation()//���������������
    {
        // ��ȡ��ǰ����ĸ����壨ʹ�ÿ�ֵ�����������������Ϊnull�������
        GameObject parentObject = this.transform.parent?.gameObject;

        // ���ٵ�ǰ����
        Destroy(this.gameObject);

        // �����������ڣ����ٸ�����
        if (parentObject != null)
        {
            Destroy(parentObject);
        }
    }
}

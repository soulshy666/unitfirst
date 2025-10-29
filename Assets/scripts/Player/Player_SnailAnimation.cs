using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_SnailAnimation : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D rb;
    public static Player_SnailAnimation instance;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetFloat("Walk", 0); // ��ʼ���ܲ�����Ϊ0
    }

    // Update is called once per frame
    void Update()
    {
        SetAnimator();
    }
    public void SetAnimator()
    {
        animator.SetFloat("Walk", Mathf.Abs(rb.velocity.x));
        animator.SetBool("IsDiscard", Player.instance.isDiscard);
    }
    public void PlayerHurt()
    {
        animator.SetTrigger("IsHurt");
    }
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

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
        animator.SetFloat("Run", 0); // ��ʼ���ܲ�����Ϊ0
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

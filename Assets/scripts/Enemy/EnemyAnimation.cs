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
    public void Death()//���¼��е���
    {
        enemy.isDeath = true;
        animator.SetBool("IsDeath", true);
    }
    public void DestroyAfterAnimation()//�������������У������¼�����
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
    public void Hurt()//�ڹ��﹥�����¼��е���
    {
        enemy.isAttack = true;
    }

    public void HurtEnd()//�ڹ��﹥�������¼��е���
    {
        enemy.isAttack = false;
    }
}

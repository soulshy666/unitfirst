using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("����")]
    public float damage = 10;
    public float begindamage =10;
    public float attackRange;
    public float attackRate;
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        
    }
    public void OnTriggerStay2D(Collider2D other)
    {
        float randomOffset = Random.Range(-0.3f, 0.3f);
        // ���������˺��������˺� * (1 + ���ƫ����)��
        damage = begindamage * (1 + randomOffset);
        if (other.tag != this.tag)//���˲�����ͬ�࣬��ҿ��Թ���ͬ��
        {
            if(Player.instance.crouchingAttack && other.GetComponent<Player>() == null && other.GetComponent<Enemy>()!=null)
            {
                Enemy enemy = other.GetComponent<Enemy>();
                if (enemy.currentState == enemy.patrolState || enemy.currentState == enemy.hurtState)
                {
                    other.GetComponent<Character>()?.CrouchingAttack();
                    other.GetComponent<Character>()?.TakeDamage3(this);
                    damage *= 2;
                    return;
                } 
            }
            if(Player.instance.shieldAttack && other.GetComponent<Player>() == null && other.GetComponent<Enemy>()!=null)
            {
                Enemy enemy = other.GetComponent<Enemy>();
                if (enemy.isAttack == true)
                {
                    damage *= 2f;
                    other.GetComponent<Character>()?.CrouchingAttack();
                    other.GetComponent<Character>()?.TakeDamage2(this);
                    return;
                }
                else if(enemy.isAttack == false)
                {
                    return;
                }
            }
            else if((!Player.instance.isShield && !Player.instance.shieldAttack) || other.GetComponent<Player>() != null)//
            {
                other.GetComponent<Character>()?.TakeDamage(this);
            }
                
        }
    }

}

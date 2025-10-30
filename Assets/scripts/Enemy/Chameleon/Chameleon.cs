using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chameleon : Enemy
{
    public override void Move()
    {
        
        base.Move();
    }

    protected override void Awake()
    {
        base.Awake();
        patrolState = new ChameleonPatrolState();
        attackState = new ChameleonAttackState();
        chaseState = new ChameleonChaseState();
       
    }
}

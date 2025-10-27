using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bora : Enemy
{
    // Start is called before the first frame update
    public override void Move()
    {
        base.Move();
    }
    protected override void Awake()
    {
        base.Awake();
        patrolState = new BoraPatrolState();
        attackState = new BoraAttackState();
        hurtState   = new BoraHurtState();
    }
}

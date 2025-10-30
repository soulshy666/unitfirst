using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orc : Enemy
{

    public override void Move()
    {
        base.Move();
    }

    protected override void Awake()
    {
        base.Awake();
        patrolState = new OrcPatrolState();
        attackState = new OrcAttackState();
        chaseState = new OrcChaseState();
        hurtState = new OrcHurtState();
    }
}

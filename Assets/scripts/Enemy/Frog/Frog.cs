using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frog : Enemy
{
    public override void Move()
    {
        base.Move();
    }
    protected override void Awake()
    {
        base.Awake();
        patrolState = new FrogPatrolState();
        chaseState = new FrogChaseState();
        attackState = new FrogAttackState();

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState_Melee : EnemyState
{
    private Enemy_Melee enemy; // ���� ���°� �����ϴ� Enemy_Melee ��ü

    // ���� �ʱ�ȭ �� Enemy_Melee ���� ����
    public IdleState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee; // Enemy�� Enemy_Melee�� ĳ����
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = enemy.idleTime; // ��� ���� ���� �ð� ����
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer < 0) // ��� �ð��� ����Ǹ�
            stateMachine.ChangeState(enemy.moveState); // Move ���·� ��ȯ
    }
}

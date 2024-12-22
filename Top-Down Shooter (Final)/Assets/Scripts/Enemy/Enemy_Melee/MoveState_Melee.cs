using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveState_Melee : EnemyState
{
    private Enemy_Melee enemy; // ���� ���°� �����ϴ� Enemy_Melee ��ü
    private Vector3 destination; // �̵� ������

    // ���� �ʱ�ȭ �� Enemy_Melee ���� ����
    public MoveState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee; // Enemy�� Enemy_Melee�� ĳ����
    }

    public override void Enter()
    {
        base.Enter();

        enemy.agent.speed = enemy.walkSpeed; // �� �̵� �ӵ� ����

        destination = enemy.GetPatrolDestination(); // ���� ������ ����
        enemy.agent.SetDestination(destination); // �׺���̼� ������ ����
    }

    public override void Update()
    {
        base.Update();

        enemy.FaceTarget(GetNextPathPoint()); // ���� ���� �̵� ������ �ٶ󺸵��� ����

        // �������� �����ϸ� ���¸� Idle ���·� ����
        if (enemy.agent.remainingDistance <= enemy.agent.stoppingDistance + .05f)
            stateMachine.ChangeState(enemy.idleState);
    }
}

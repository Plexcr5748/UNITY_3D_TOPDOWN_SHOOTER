using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���Ÿ� ���� �̵��ϴ� ���¸� �����ϴ� Ŭ����
public class MoveState_Range : EnemyState
{
    private Enemy_Range enemy; // ���� ���°� �����ϴ� Enemy_Range ��ü
    private Vector3 destination; // ��ǥ �̵� ����

    public MoveState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range; // Enemy�� Enemy_Range�� ĳ����
    }

    public override void Enter()
    {
        base.Enter();

        // �̵� �ӵ� ����
        enemy.agent.speed = enemy.walkSpeed;

        // ��ǥ ���� ����
        destination = enemy.GetPatrolDestination();
        enemy.agent.SetDestination(destination); // �׺���̼� ������Ʈ�� ��ǥ ���� ����
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        // ��ǥ �������� ���� ���ϵ��� ����
        enemy.FaceTarget(GetNextPathPoint());

        // ��ǥ ������ ���������� ��� ���·� ��ȯ
        if (enemy.agent.remainingDistance <= enemy.agent.stoppingDistance + .05f)
            stateMachine.ChangeState(enemy.idleState);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���Ÿ� ���� ���� �������� �޷����� ���¸� �����ϴ� Ŭ����
public class RunToCoverState_Range : EnemyState
{
    private Enemy_Range enemy; // ���� ���°� �����ϴ� Enemy_Range ��ü
    private Vector3 destination; // ��ǥ ���� ����

    public float lastTimeTookCover { get; private set; } // ���������� ���� ������ ������ �ð�

    public RunToCoverState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range; // Enemy�� Enemy_Range�� ĳ����
    }

    public override void Enter()
    {
        base.Enter();

        // ��ǥ ���� ���� ����
        destination = enemy.currentCover.transform.position;

        // IK Ȱ��ȭ�Ͽ� ���� �� ĳ������ ��ġ ����
        enemy.visuals.EnableIK(true, false);

        // �̵� �غ�
        enemy.agent.isStopped = false; // �̵� ���� ����
        enemy.agent.speed = enemy.runSpeed; // �޸��� �ӵ� ����
        enemy.agent.SetDestination(destination); // ��ǥ ���� ����
    }

    public override void Exit()
    {
        base.Exit();

        // ���� ������ ������ �ð��� ���
        lastTimeTookCover = Time.time;
    }

    public override void Update()
    {
        base.Update();

        // ��ǥ �������� ���� ���ϵ��� ����
        enemy.FaceTarget(GetNextPathPoint());

        // ��ǥ ������ ���������� ���� ���·� ��ȯ
        if (Vector3.Distance(enemy.transform.position, destination) < .8f)
            stateMachine.ChangeState(enemy.battleState);
    }
}

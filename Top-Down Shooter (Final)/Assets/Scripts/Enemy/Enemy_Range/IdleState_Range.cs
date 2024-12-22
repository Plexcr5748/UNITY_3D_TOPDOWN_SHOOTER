using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���Ÿ� ���� ����ϴ� ���¸� �����ϴ� Ŭ����
public class IdleState_Range : EnemyState
{
    private Enemy_Range enemy; // ���� ���°� �����ϴ� Enemy_Range ��ü

    public IdleState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range; // Enemy�� Enemy_Range�� ĳ����
    }

    public override void Enter()
    {
        base.Enter();

        // ��� �ִϸ��̼� �ε��� ���� (0~3���� ���� ����)
        enemy.anim.SetFloat("IdleAnimIndex", Random.Range(0, 3));

        // IK Ȱ��ȭ (���� ���� �� ���� ��ġ ����)
        enemy.visuals.EnableIK(true, false);

        // ���Ⱑ ������ ��� IK ��Ȱ��ȭ
        if (enemy.weaponType == Enemy_RangeWeaponType.Pistol)
            enemy.visuals.EnableIK(false, false);

        // ��� ���� Ÿ�̸� ����
        stateTimer = enemy.idleTime;
    }

    public override void Update()
    {
        base.Update();

        // ��� �ð��� ������ �̵� ���·� ��ȯ
        if (stateTimer < 0)
            stateMachine.ChangeState(enemy.moveState);
    }
}

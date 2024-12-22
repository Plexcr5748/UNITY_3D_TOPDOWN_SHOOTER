using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���Ÿ� ���� ����ź�� ������ ���¸� �����ϴ� Ŭ����
public class ThrowGrenadeState_Range : EnemyState
{
    private Enemy_Range enemy; // ���� ���°� �����ϴ� Enemy_Range ��ü
    public bool finishedThrowingGrenade { get; private set; } = true; // ����ź ������ �Ϸ� ����

    public ThrowGrenadeState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range; // Enemy�� Enemy_Range�� ĳ����
    }

    public override void Enter()
    {
        base.Enter();

        finishedThrowingGrenade = false; // ����ź ������ ����

        // ����ź ������ �غ�
        enemy.visuals.EnableWeaponModel(false); // ���� �� ��Ȱ��ȭ
        enemy.visuals.EnableIK(false, false); // IK ��Ȱ��ȭ
        enemy.visuals.EnableSeconoderyWeaponModel(true); // ���� ���� �� Ȱ��ȭ
        enemy.visuals.EnableGrenadeModel(true); // ����ź �� Ȱ��ȭ
    }

    public override void Update()
    {
        base.Update();

        // �÷��̾� ��ġ�� ����
        Vector3 playerPos = enemy.player.position + Vector3.up;
        enemy.FaceTarget(playerPos); // �÷��̾ �ٶ󺸵��� ����
        enemy.aim.position = playerPos; // ���� ��ġ ����

        if (triggerCalled) // Ʈ���Ű� ȣ��Ǹ� ���� ���·� ��ȯ
            stateMachine.ChangeState(enemy.battleState);
    }

    public override void AbilityTrigger()
    {
        base.AbilityTrigger();
        finishedThrowingGrenade = true; // ����ź ������ �Ϸ�
        enemy.ThrowGrenade(); // ����ź ������ ����
    }
}

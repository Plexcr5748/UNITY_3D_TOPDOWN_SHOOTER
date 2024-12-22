using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ������ ��� ���¸� ó���ϴ� Ŭ����
public class DeadState_Boss : EnemyState
{
    private Enemy_Boss enemy; // ���� ��ü ����
    private bool interactionDisabled; // ��ȣ�ۿ� ��Ȱ��ȭ ����

    public DeadState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Boss; // Enemy�� Enemy_Boss�� ĳ����
    }

    public override void Enter()
    {
        base.Enter();

        // ��� ���� ���� �� ȭ�� ���� ��Ȱ��ȭ
        enemy.abilityState.DisableFlamethrower();

        interactionDisabled = false; // ��ȣ�ۿ� �ʱ�ȭ
        stateTimer = 1.5f; // ���� ���� �ð� ����
    }

    public override void Update()
    {
        base.Update();
        // �ּ� ���� ��, ��ȣ�ۿ� ��Ȱ��ȭ ó��
        // DisableInteractionIfShould();
    }

    // ��ȣ�ۿ� ��Ȱ��ȭ ó��
    private void DisableInteractionIfShould()
    {
        if (stateTimer < 0 && !interactionDisabled)
        {
            interactionDisabled = true;
            enemy.ragdoll.RagdollActive(false); // ���׵� ��Ȱ��ȭ
            enemy.ragdoll.CollidersActive(false); // �浹ü ��Ȱ��ȭ
        }
    }
}

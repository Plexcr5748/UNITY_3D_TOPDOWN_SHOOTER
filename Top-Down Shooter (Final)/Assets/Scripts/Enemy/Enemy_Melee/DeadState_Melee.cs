using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� ������� ���� ���¸� �����ϴ� Ŭ����
public class DeadState_Melee : EnemyState
{
    private Enemy_Melee enemy; // ���� ���°� �����ϴ� Enemy_Melee ��ü
    private bool interactionDisabled; // ��ȣ�ۿ� ��Ȱ��ȭ ����

    public DeadState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee; // Enemy�� Enemy_Melee�� ĳ����
    }

    public override void Enter()
    {
        base.Enter();

        interactionDisabled = false; // �ʱ�ȭ: ��ȣ�ۿ� Ȱ�� ���� ����
        stateTimer = 1.5f; // ���� ���� �ð� ����
    }

    public override void Exit()
    {
        base.Exit(); // �⺻ Exit ���� ȣ��
    }

    public override void Update()
    {
        base.Update();

        // �ʿ��� ��� ��ȣ�ۿ��� ��Ȱ��ȭ
        // DisableInteractionIfShould(); 
    }

    private void DisableInteractionIfShould()
    {
        // ��� ���¿��� ��ȣ�ۿ� ��Ȱ��ȭ
        if (stateTimer < 0 && interactionDisabled == false)
        {
            interactionDisabled = true;
            enemy.ragdoll.RagdollActive(false); // ���׵� ��Ȱ��ȭ
            enemy.ragdoll.CollidersActive(false); // �浹ü ��Ȱ��ȭ
        }
    }
}

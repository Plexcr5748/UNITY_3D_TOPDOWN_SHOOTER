using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// ���� ���¸� �����ϴ� �⺻ Ŭ����
public class EnemyState
{
    protected Enemy enemyBase; // �� ��ü
    protected EnemyStateMachine stateMachine; // ���� ��� ��ü

    protected string animBoolName; // �ִϸ��̼��� ���� bool ���� �̸�
    protected float stateTimer; // ������ Ÿ�̸� (�ð� ���� ����)

    protected bool triggerCalled; // �ִϸ��̼� Ʈ���� ȣ�� ����

    // ������: ���¿� �ʿ��� �� ��ü, ���� ���, �ִϸ��̼� bool �̸� �ʱ�ȭ
    public EnemyState(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName)
    {
        this.enemyBase = enemyBase;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    // ���� ���� �� ȣ��Ǵ� �޼���
    public virtual void Enter()
    {
        enemyBase.anim.SetBool(animBoolName, true); // �ִϸ��̼��� bool ���� true�� �����Ͽ� ���� �ִϸ��̼� ����
        triggerCalled = false; // Ʈ���� �ʱ�ȭ
    }

    // ���� ������Ʈ �� ȣ��Ǵ� �޼���
    public virtual void Update()
    {
        stateTimer -= Time.deltaTime; // ���� Ÿ�̸� ����
    }

    // ���� ���� �� ȣ��Ǵ� �޼���
    public virtual void Exit()
    {
        enemyBase.anim.SetBool(animBoolName, false); // �ִϸ��̼��� bool ���� false�� �����Ͽ� ���� �ִϸ��̼� ����
    }

    // �ִϸ��̼� Ʈ���Ÿ� ȣ���ϴ� �޼���
    public void AnimationTrigger() => triggerCalled = true;

    // �ɷ� �ߵ� �� ȣ��Ǵ� �޼��� (�⺻������ �����, �ʿ� �� �������̵� ����)
    public virtual void AbilityTrigger()
    {

    }

    // ����� ���� ������ ��� �޼���
    protected Vector3 GetNextPathPoint()
    {
        NavMeshAgent agent = enemyBase.agent; // �׺���̼� ������Ʈ
        NavMeshPath path = agent.path; // ���� ���

        // ����� �ڳʰ� 2�� �̸��̸� ������ ��ȯ
        if (path.corners.Length < 2)
            return agent.destination;

        // ��θ� ���󰡸� �� ������ ������ ������ ���� �������� �̵�
        for (int i = 0; i < path.corners.Length; i++)
        {
            if (Vector3.Distance(agent.transform.position, path.corners[i]) < 1) // ���� ������ ������ ���
                return path.corners[i + 1]; // ���� ���� ��ȯ
        }

        return agent.destination; // ������ ��ȯ
    }
}

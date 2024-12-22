using UnityEngine;

// ������ �̵� ���¸� ó���ϴ� Ŭ����
public class MoveState_Boss : EnemyState
{
    private Enemy_Boss enemy; // ���� �� ����
    private Vector3 destination; // �̵� ��ǥ ����

    private float actionTimer; // �ൿ Ÿ�̸�
    private float timeBeforeSpeedUp = 5; // �ӵ� ���� �� ��� �ð�

    private bool speedUpActivated; // �ӵ� ���� Ȱ��ȭ ����

    public MoveState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Boss; // Enemy�� Enemy_Boss�� ĳ����
    }

    public override void Enter()
    {
        base.Enter();

        SpeedReset(); // �̵� �ӵ� �ʱ�ȭ
        enemy.agent.isStopped = false; // �̵� ����

        destination = enemy.GetPatrolDestination(); // ���� ������ ����
        enemy.agent.SetDestination(destination);

        actionTimer = enemy.actionCooldown; // �ൿ Ÿ�̸� �ʱ�ȭ
    }

    // �̵� �ӵ� �� �ִϸ��̼� �ʱ�ȭ
    private void SpeedReset()
    {
        speedUpActivated = false;
        enemy.anim.SetFloat("MoveAnimSpeedMultiplier", 1);
        enemy.anim.SetFloat("MoveAnimIndex", 0);
        enemy.agent.speed = enemy.walkSpeed;
    }

    public override void Update()
    {
        base.Update();

        actionTimer -= Time.deltaTime; // �ൿ Ÿ�̸� ����
        enemy.FaceTarget(GetNextPathPoint()); // �̵� �������� ȸ��

        if (enemy.inBattleMode)
        {
            if (ShouldSpeedUp())
                SpeedUp(); // �ӵ� ���� ó��

            Vector3 playerPos = enemy.player.position;
            enemy.agent.SetDestination(playerPos); // �÷��̾ ��ǥ�� �̵�

            if (actionTimer < 0)
            {
                PerformRandomAction(); // ���� �ൿ ����
            }
            else if (enemy.PlayerInAttackRange())
                stateMachine.ChangeState(enemy.attackState); // ���� ���·� ��ȯ
        }
        else
        {
            if (Vector3.Distance(enemy.transform.position, destination) < .25f)
                stateMachine.ChangeState(enemy.idleState); // ���� �� ��� ���·� ��ȯ
        }
    }

    // �ӵ� ���� ó��
    private void SpeedUp()
    {
        enemy.agent.speed = enemy.runSpeed; // �ӵ� ����
        enemy.anim.SetFloat("MoveAnimIndex", 1); // �̵� �ִϸ��̼� ����
        enemy.anim.SetFloat("MoveAnimSpeedMultiplier", 1.5f); // �ִϸ��̼� �ӵ� ����
        speedUpActivated = true; // �ӵ� ���� Ȱ��ȭ
    }

    // ���� �ൿ ����
    private void PerformRandomAction()
    {
        actionTimer = enemy.actionCooldown; // �ൿ Ÿ�̸� �ʱ�ȭ

        if (Random.Range(0, 2) == 0) // 0 �Ǵ� 1�� ���� �� ����
        {
            TryAbility(); // �ɷ� ��� �õ�
        }
        else
        {
            if (enemy.CanDoJumpAttack())
                stateMachine.ChangeState(enemy.jumpAttackState); // ���� ���� ���·� ��ȯ
            else if (enemy.bossWeaponType == BossWeaponType.Hummer)
                TryAbility(); // ��ġ �ɷ� ��� �õ�
        }
    }

    // �ɷ� ��� �õ�
    private void TryAbility()
    {
        if (enemy.CanDoAbility())
            stateMachine.ChangeState(enemy.abilityState); // �ɷ� ���·� ��ȯ
    }

    // �ӵ� ���� ���� Ȯ��
    private bool ShouldSpeedUp()
    {
        if (speedUpActivated)
            return false;

        if (Time.time > enemy.attackState.lastTimeAttacked + timeBeforeSpeedUp)
        {
            return true;
        }

        return false;
    }
}

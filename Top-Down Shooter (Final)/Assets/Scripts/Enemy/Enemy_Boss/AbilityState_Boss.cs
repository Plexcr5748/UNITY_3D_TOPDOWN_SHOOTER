using UnityEngine;

// ������ �ɷ� ��� ���¸� ó���ϴ� Ŭ����
public class AbilityState_Boss : EnemyState
{
    private Enemy_Boss enemy; // ���� ��ü ����

    public AbilityState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Boss; // Enemy�� Enemy_Boss�� ĳ����
    }

    public override void Enter()
    {
        base.Enter();

        // ���� Ÿ�̸Ӹ� ȭ�� ��� ���� �ð����� ����
        stateTimer = enemy.flamethrowDuration;

        // �׺���̼� ���� �� �ʱ�ȭ
        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;

        // ���� Ʈ���� Ȱ��ȭ
        enemy.bossVisuals.EnableWeaponTrail(true);
    }

    public override void Update()
    {
        base.Update();

        // �÷��̾ �ٶ󺸵��� ���� ȸ��
        enemy.FaceTarget(enemy.player.position);

        // ȭ�� ���� ��Ȱ��ȭ ���� Ȯ��
        if (ShouldDisableFlamethrower())
            DisableFlamethrower();

        // �ִϸ��̼� Ʈ���� ȣ�� �� �̵� ���·� ��ȯ
        if (triggerCalled)
            stateMachine.ChangeState(enemy.moveState);
    }

    // ȭ�� ���⸦ ��Ȱ��ȭ���� �Ǵ�
    private bool ShouldDisableFlamethrower() => stateTimer < 0;

    // ȭ�� ���� ��Ȱ��ȭ
    public void DisableFlamethrower()
    {
        if (enemy.bossWeaponType != BossWeaponType.Flamethrower)
            return;

        if (!enemy.flamethrowActive)
            return;

        enemy.ActivateFlamethrower(false);
    }

    public override void AbilityTrigger()
    {
        base.AbilityTrigger();

        // ȭ�� ���� �ɷ� Ȱ��ȭ
        if (enemy.bossWeaponType == BossWeaponType.Flamethrower)
        {
            enemy.ActivateFlamethrower(true);
            enemy.bossVisuals.DischargeBatteries(); // ���͸� ����
            enemy.bossVisuals.EnableWeaponTrail(false);
        }

        // ��ġ �ɷ� Ȱ��ȭ
        if (enemy.bossWeaponType == BossWeaponType.Hummer)
        {
            enemy.ActivateHummer();
        }
    }

    public override void Exit()
    {
        base.Exit();

        // �ɷ� ��ٿ� ����
        enemy.SetAbilityOnCooldown();

        // ���͸� �ʱ�ȭ
        enemy.bossVisuals.ResetBatteries();
    }
}

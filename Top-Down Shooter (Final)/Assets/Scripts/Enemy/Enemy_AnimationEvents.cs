using UnityEngine;

// ���� �ִϸ��̼� �̺�Ʈ�� ó���ϴ� Ŭ����
public class Enemy_AnimationEvents : MonoBehaviour
{
    private Enemy enemy; // ���� �Ϲ� ��� ����
    private Enemy_Melee enemyMelee; // ���� �� ��� ����
    private Enemy_Boss enemyBoss; // ���� �� ��� ����

    // �ʱ�ȭ: �θ� ��ü���� �� Ŭ���� ���� ��������
    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
        enemyMelee = GetComponentInParent<Enemy_Melee>();
        enemyBoss = GetComponentInParent<Enemy_Boss>();
    }

    // �ִϸ��̼� Ʈ���� ȣ��
    public void AnimationTrigger() => enemy.AnimationTrigger();

    // ���� �̵� ����
    public void StartManualMovement() => enemy.ActivateManualMovement(true);

    // ���� �̵� ����
    public void StopManualMovement() => enemy.ActivateManualMovement(false);

    // ���� ȸ�� ����
    public void StartManualRotation() => enemy.ActivateManualRotation(true);

    // ���� ȸ�� ����
    public void StopManualRotation() => enemy.ActivateManualRotation(false);

    // �ɷ� �ߵ� �̺�Ʈ
    public void AbilityEvent() => enemy.AbilityTrigger();

    // IK Ȱ��ȭ
    public void EnableIK() => enemy.visuals.EnableIK(true, true, 1f);

    // ���� �� Ȱ��ȭ
    public void EnableWeaponModel()
    {
        enemy.visuals.EnableWeaponModel(true); // �� ���� Ȱ��ȭ
        enemy.visuals.EnableSeconoderyWeaponModel(false); // ���� ���� ��Ȱ��ȭ
    }

    // ���� ���� ��� �̺�Ʈ
    public void BossJumpImpact()
    {
        enemyBoss?.JumpImpact(); // ���� ���� ���� ��� ȣ��
    }

    // ���� ���� ���� ����
    public void BeginMeleeAttackCheck()
    {
        enemy?.EnableMeleeAttackCheck(true); // ���� ���� Ȱ��ȭ
        enemy?.audioManager.PlaySFX(enemyMelee?.meleeSFX.swoosh, true); // ���� ���� ȿ���� ���
    }

    // ���� ���� ���� ����
    public void FinishMeleeAttackCheck()
    {
        enemy?.EnableMeleeAttackCheck(false); // ���� ���� ��Ȱ��ȭ
    }
}

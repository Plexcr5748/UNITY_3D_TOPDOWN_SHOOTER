using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// HealthController: ü���� �����ϴ� Ŭ����
public class HealthController : MonoBehaviour
{
    public delegate void EnemyDestroyed(); // ���� �ı��Ǿ��� �� ȣ��Ǵ� ��������Ʈ
    public event EnemyDestroyed OnEnemyDestroyed; // �� �ı� �̺�Ʈ

    public int maxHealth; // �ִ� ü��
    public int currentHealth; // ���� ü��
    [SerializeField] private float destroyDelay = 5f; // �� ���� �� ������ �ð�

    private bool isDead; // ��� ���¸� ����

    protected virtual void Awake()
    {
        // �ʱ�ȭ: ���� ü���� �ִ� ü������ ����
        currentHealth = maxHealth;
    }

    public virtual void ReduceHealth(int damage)
    {
        // ü���� ���ҽ�Ű�� �޼���
        currentHealth -= damage;
    }

    public virtual void IncreaseHealth()
    {
        // ü���� ������Ű�� �޼���
        currentHealth++;

        // �ִ� ü���� �ʰ����� �ʵ��� ����
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }

    public bool ShouldDie()
    {
        // ��� ���θ� �Ǵ�
        if (isDead)
            return false; // �̹� ���� �����̸� ó������ ����

        if (currentHealth < 0) // ü���� 0���� ������ ��� ó��
        {
            isDead = true; // ��� ���� ����
            if (CompareTag("Enemy") || CompareTag("Enemy_Boss")) // �� �±� Ȯ��
            {
                OnEnemyDestroyed?.Invoke(); // �� �ı� �̺�Ʈ ȣ��
                StartCoroutine(DestroyAfterDelay()); // ������ �� ����
            }
            return true;
        }

        return false;
    }

    private IEnumerator DestroyAfterDelay()
    {
        // ������ �� ������Ʈ ����
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject); // ���� ������Ʈ ����
    }
}

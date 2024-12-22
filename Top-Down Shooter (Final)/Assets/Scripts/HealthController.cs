using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// HealthController: 체력을 관리하는 클래스
public class HealthController : MonoBehaviour
{
    public delegate void EnemyDestroyed(); // 적이 파괴되었을 때 호출되는 델리게이트
    public event EnemyDestroyed OnEnemyDestroyed; // 적 파괴 이벤트

    public int maxHealth; // 최대 체력
    public int currentHealth; // 현재 체력
    [SerializeField] private float destroyDelay = 5f; // 적 삭제 전 딜레이 시간

    private bool isDead; // 사망 상태를 추적

    protected virtual void Awake()
    {
        // 초기화: 현재 체력을 최대 체력으로 설정
        currentHealth = maxHealth;
    }

    public virtual void ReduceHealth(int damage)
    {
        // 체력을 감소시키는 메서드
        currentHealth -= damage;
    }

    public virtual void IncreaseHealth()
    {
        // 체력을 증가시키는 메서드
        currentHealth++;

        // 최대 체력을 초과하지 않도록 제한
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }

    public bool ShouldDie()
    {
        // 사망 여부를 판단
        if (isDead)
            return false; // 이미 죽은 상태이면 처리하지 않음

        if (currentHealth < 0) // 체력이 0보다 작으면 사망 처리
        {
            isDead = true; // 사망 상태 설정
            if (CompareTag("Enemy") || CompareTag("Enemy_Boss")) // 적 태그 확인
            {
                OnEnemyDestroyed?.Invoke(); // 적 파괴 이벤트 호출
                StartCoroutine(DestroyAfterDelay()); // 딜레이 후 삭제
            }
            return true;
        }

        return false;
    }

    private IEnumerator DestroyAfterDelay()
    {
        // 딜레이 후 오브젝트 삭제
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject); // 게임 오브젝트 제거
    }
}

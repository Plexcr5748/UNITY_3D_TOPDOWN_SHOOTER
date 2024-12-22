using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 적의 방패를 관리하는 클래스
public class Enemy_Shield : MonoBehaviour, IDamagable
{
    private Enemy_Melee enemy; // 방패를 가진 적의 참조
    [SerializeField] private int durability; // 방패 내구도

    // 초기화
    private void Awake()
    {
        enemy = GetComponentInParent<Enemy_Melee>(); // 부모 객체에서 적 정보 가져오기
        durability = enemy.shieldDurability; // 적의 방패 내구도를 설정
    }

    // 방패 내구도 감소
    public void ReduceDurability(int damage)
    {
        durability -= damage;

        // 내구도가 0 이하가 되면 방패 비활성화
        if (durability <= 0)
        {
            enemy.anim.SetFloat("ChaseIndex", 0); // 기본 추적 애니메이션 활성화
            gameObject.SetActive(false); // 방패 오브젝트 비활성화
        }
    }

    // 피해를 받았을 때 내구도 감소 처리
    public void TakeDamage(int damage)
    {
        ReduceDurability(damage);
    }
}

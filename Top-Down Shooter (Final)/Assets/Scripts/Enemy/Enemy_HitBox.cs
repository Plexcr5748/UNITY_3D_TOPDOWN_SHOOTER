using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 적의 히트박스를 처리하는 클래스
public class Enemy_HitBox : HitBox
{
    private Enemy enemy; // 적 캐릭터에 대한 참조

    // 초기화
    protected override void Awake()
    {
        base.Awake(); // 부모 클래스의 Awake 호출
        enemy = GetComponentInParent<Enemy>(); // 부모 오브젝트에서 Enemy 컴포넌트를 가져옴
    }

    // 피해 처리
    public override void TakeDamage(int damage)
    {
        // 피해량에 히트박스의 데미지 배율 적용
        int newDamage = Mathf.RoundToInt(damage * damageMultiplier);

        // 적에게 수정된 피해량 전달
        enemy.GetHit(newDamage);
    }
}

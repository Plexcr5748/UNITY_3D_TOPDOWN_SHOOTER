using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Player_Hitbox: 플레이어의 히트박스를 관리하며 데미지를 처리하는 클래스
public class Player_Hitbox : HitBox
{
    private Player player; // 플레이어 객체 참조

    protected override void Awake()
    {
        // 부모 클래스의 Awake 호출
        base.Awake();

        // 부모 객체에서 Player 컴포넌트 가져오기
        player = GetComponentInParent<Player>();
    }

    public override void TakeDamage(int damage)
    {
        // 데미지에 데미지 배율을 적용
        int newDamage = Mathf.RoundToInt(damage * damageMultiplier);

        // 플레이어의 체력을 감소시킴
        player.health.ReduceHealth(newDamage);
    }
}

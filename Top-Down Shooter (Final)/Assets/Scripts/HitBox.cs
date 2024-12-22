using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// HitBox: 데미지를 받을 수 있는 히트박스 클래스
public class HitBox : MonoBehaviour, IDamagable
{
    [SerializeField] protected float damageMultiplier = 1f;
    // 데미지 배수: 히트박스에 따라 데미지를 조정하는 데 사용

    protected virtual void Awake()
    {
    }

    public virtual void TakeDamage(int damage)
    {
        // 데미지를 받을 때 실행될 로직
    }
}

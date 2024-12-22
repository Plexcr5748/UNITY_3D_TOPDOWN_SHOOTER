using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// IDamagable 인터페이스: 데미지를 받을 수 있는 객체를 정의
public interface IDamagable
{
    // 데미지를 받는 메서드
    void TakeDamage(int damage);
}

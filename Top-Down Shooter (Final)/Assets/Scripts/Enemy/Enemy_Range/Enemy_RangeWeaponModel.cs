using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 적의 원거리 무기 유지 타입 정의 (일반, 낮게 유지, 높게 유지)
public enum Enemy_RangeWeaponHoldType { Common, LowHold, HighHold };

// 적의 원거리 무기 모델을 나타내는 클래스
public class Enemy_RangeWeaponModel : MonoBehaviour
{
    // 총알이 발사되는 위치
    public Transform gunPoint;

    [Space]
    // 무기 타입 (피스톨, 리볼버 등)
    public Enemy_RangeWeaponType weaponType;

    // 무기 유지 타입 (Common, LowHold, HighHold)
    public Enemy_RangeWeaponHoldType weaponHoldType;

    // 왼손의 목표 위치 및 팔꿈치의 목표 위치 (IK 설정에 사용)
    public Transform leftHandTarget;
    public Transform leftElbowTarget;
}

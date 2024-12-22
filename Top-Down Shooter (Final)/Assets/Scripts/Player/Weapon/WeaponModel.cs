using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 무기 장착 및 유지 애니메이션 타입
public enum EquipType { SideEquipAnimation, BackEquipAnimation };

// 무기 홀드(잡는 방식) 타입
public enum HoldType { CommonHold = 1, LowHold, HighHold };

public class WeaponModel : MonoBehaviour
{
    public WeaponType weaponType; // 무기의 타입
    public EquipType equipAnimationType; // 장착 애니메이션 타입
    public HoldType holdType; // 무기의 잡는 방식

    public Transform gunPoint; // 총알이 발사되는 위치
    public Transform holdPoint; // 캐릭터가 무기를 잡는 위치

    [Header("Audio")]
    public AudioSource fireSFX; // 발사 사운드
    public AudioSource realodSfx; // 재장전 사운드
}

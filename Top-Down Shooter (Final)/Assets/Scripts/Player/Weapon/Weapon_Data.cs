using UnityEngine;

// ScriptableObject를 사용하여 무기 데이터를 정의
[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Weapon System/Weapon Data")]
public class Weapon_Data : ScriptableObject
{
    public string weaponName; // 무기 이름

    [Header("Bullet info")]
    public int bulletDamage; // 총알 데미지

    [Header("Magazine details")]
    public int bulletsInMagazine; // 현재 탄창 내 총알 수
    public int magazineCapacity; // 탄창 최대 용량
    public int totalReserveAmmo; // 예비 탄약 수

    [Header("Regular shot")]
    public ShootType shootType; // 발사 타입 (예: 단일 발사, 연사 등)
    public int bulletsPerShot = 1; // 한 번의 발사에 발사되는 총알 수
    public float fireRate; // 발사 간격 (초)

    [Header("Burst shot")]
    public bool burstAvalible; // 연사 모드 가능 여부
    public bool burstActive; // 연사 모드 활성화 여부
    public int burstBulletsPerShot; // 연사 모드에서 발사되는 총알 수
    public float burstFireRate; // 연사 모드의 발사 간격
    public float burstFireDelay = .1f; // 연사 간격 지연 시간

    [Header("Weapon spread")]
    public float baseSpread; // 기본 탄퍼짐
    public float maxSpread; // 최대 탄퍼짐
    public float spreadIncreaseRate = .15f; // 탄퍼짐 증가 속도

    [Header("Weapon generics")]
    public WeaponType weaponType; // 무기 타입 (예: 권총, 소총 등)
    [Range(1, 3)]
    public float reloadSpeed = 1; // 재장전 속도 (1=기본 속도)
    [Range(1, 3)]
    public float equipmentSpeed = 1; // 무기 장착 속도 (1=기본 속도)
    [Range(4, 25)]
    public float gunDistance = 4; // 총알 비행 거리
    [Range(4, 10)]
    public float cameraDistance = 6; // 무기 장착 시 카메라 거리

    [Header("UI elements")]
    public Sprite weaponIcon; // 무기 아이콘 (UI에서 사용)
    public string weaponInfo; // 무기 설명 텍스트
}

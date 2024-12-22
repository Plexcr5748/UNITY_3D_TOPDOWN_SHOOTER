using UnityEngine;

// 무기 걸기 방식 정의
public enum HangType
{
    LowBackHang, // 허리 아래쪽 걸기
    BackHang,    // 등에 걸기
    SideHang     // 옆에 걸기
}

public class BackupWeaponModel : MonoBehaviour
{
    public WeaponType weaponType; // 백업 무기의 타입
    [SerializeField] private HangType hangType; // 무기를 걸 위치 타입

    // 무기를 활성화하거나 비활성화
    public void Activate(bool activated) => gameObject.SetActive(activated);

    // 현재 무기의 걸기 타입이 주어진 타입과 일치하는지 확인
    public bool HangTypeIs(HangType hangType) => this.hangType == hangType;
}

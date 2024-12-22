using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 보스의 시각 효과를 관리하는 클래스
public class Enemy_BossVisuals : MonoBehaviour
{
    private Enemy_Boss enemy;

    [SerializeField] private float landingOffset = 1; // 착지 효과의 오프셋
    [SerializeField] private ParticleSystem landindZoneFx; // 착지 효과
    [SerializeField] private GameObject[] weaponTrails; // 무기 트레일 효과

    [Header("Batteries")]
    [SerializeField] private GameObject[] batteries; // 배터리 오브젝트
    [SerializeField] private float initalBatterySclaeY = .2f; // 배터리 초기 Y축 스케일

    private float dischargeSpeed; // 배터리 방전 속도
    private float rechargeSpeed; // 배터리 충전 속도

    private bool isRecharging; // 배터리 충전 여부

    private void Awake()
    {
        enemy = GetComponent<Enemy_Boss>();

        // 착지 효과 초기화
        landindZoneFx.transform.parent = null;
        landindZoneFx.Stop();

        // 배터리 초기화
        ResetBatteries();
    }

    private void Update()
    {
        UpdateBatteriesScale(); // 배터리 상태 업데이트
    }

    // 무기 트레일 활성화/비활성화
    public void EnableWeaponTrail(bool active)
    {
        if (weaponTrails.Length <= 0)
        {
            Debug.LogWarning("No weapon trails assigned");
            return;
        }

        foreach (var trail in weaponTrails)
        {
            trail.gameObject.SetActive(active);
        }
    }

    // 착지 효과 위치 지정 및 재생
    public void PlaceLandindZone(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        Vector3 offset = dir.normalized * landingOffset;
        landindZoneFx.transform.position = target + offset;
        landindZoneFx.Clear();

        var mainModule = landindZoneFx.main;
        mainModule.startLifetime = enemy.travelTimeToTarget * 2; // 착지 효과 지속 시간 설정

        landindZoneFx.Play();
    }

    // 배터리 스케일 업데이트
    private void UpdateBatteriesScale()
    {
        if (batteries.Length <= 0)
            return;

        foreach (GameObject battery in batteries)
        {
            if (battery.activeSelf)
            {
                float scaleChange = (isRecharging ? rechargeSpeed : -dischargeSpeed) * Time.deltaTime;
                float newScaleY = Mathf.Clamp(battery.transform.localScale.y + scaleChange, 0, initalBatterySclaeY);

                battery.transform.localScale = new Vector3(0.15f, newScaleY, 0.15f);

                if (battery.transform.localScale.y <= 0)
                    battery.SetActive(false); // 배터리가 방전되면 비활성화
            }
        }
    }

    // 배터리 초기화
    public void ResetBatteries()
    {
        isRecharging = true;

        rechargeSpeed = initalBatterySclaeY / enemy.abilityCooldown; // 충전 속도 계산
        dischargeSpeed = initalBatterySclaeY / (enemy.flamethrowDuration * .75f); // 방전 속도 계산

        foreach (GameObject battery in batteries)
        {
            battery.SetActive(true);
        }
    }

    // 배터리 방전 시작
    public void DischargeBatteries() => isRecharging = false;
}

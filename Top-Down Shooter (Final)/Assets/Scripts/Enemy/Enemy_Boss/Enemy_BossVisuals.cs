using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ������ �ð� ȿ���� �����ϴ� Ŭ����
public class Enemy_BossVisuals : MonoBehaviour
{
    private Enemy_Boss enemy;

    [SerializeField] private float landingOffset = 1; // ���� ȿ���� ������
    [SerializeField] private ParticleSystem landindZoneFx; // ���� ȿ��
    [SerializeField] private GameObject[] weaponTrails; // ���� Ʈ���� ȿ��

    [Header("Batteries")]
    [SerializeField] private GameObject[] batteries; // ���͸� ������Ʈ
    [SerializeField] private float initalBatterySclaeY = .2f; // ���͸� �ʱ� Y�� ������

    private float dischargeSpeed; // ���͸� ���� �ӵ�
    private float rechargeSpeed; // ���͸� ���� �ӵ�

    private bool isRecharging; // ���͸� ���� ����

    private void Awake()
    {
        enemy = GetComponent<Enemy_Boss>();

        // ���� ȿ�� �ʱ�ȭ
        landindZoneFx.transform.parent = null;
        landindZoneFx.Stop();

        // ���͸� �ʱ�ȭ
        ResetBatteries();
    }

    private void Update()
    {
        UpdateBatteriesScale(); // ���͸� ���� ������Ʈ
    }

    // ���� Ʈ���� Ȱ��ȭ/��Ȱ��ȭ
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

    // ���� ȿ�� ��ġ ���� �� ���
    public void PlaceLandindZone(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        Vector3 offset = dir.normalized * landingOffset;
        landindZoneFx.transform.position = target + offset;
        landindZoneFx.Clear();

        var mainModule = landindZoneFx.main;
        mainModule.startLifetime = enemy.travelTimeToTarget * 2; // ���� ȿ�� ���� �ð� ����

        landindZoneFx.Play();
    }

    // ���͸� ������ ������Ʈ
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
                    battery.SetActive(false); // ���͸��� �����Ǹ� ��Ȱ��ȭ
            }
        }
    }

    // ���͸� �ʱ�ȭ
    public void ResetBatteries()
    {
        isRecharging = true;

        rechargeSpeed = initalBatterySclaeY / enemy.abilityCooldown; // ���� �ӵ� ���
        dischargeSpeed = initalBatterySclaeY / (enemy.flamethrowDuration * .75f); // ���� �ӵ� ���

        foreach (GameObject battery in batteries)
        {
            battery.SetActive(true);
        }
    }

    // ���͸� ���� ����
    public void DischargeBatteries() => isRecharging = false;
}

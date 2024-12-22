using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Dummy 클래스: 데미지를 받을 수 있는 더미 객체
public class Dummy : MonoBehaviour, IDamagable
{
    public int currentHealth; // 현재 체력
    public int maxHealth = 100; // 최대 체력

    [Space]
    public MeshRenderer mesh; // 더미의 MeshRenderer
    public Material whiteMat; // 기본 상태의 재질
    public Material redMat; // 파괴 상태의 재질

    [Space]
    public float refreshCooldown; // 체력 리프레시 쿨다운
    private float lastTimeDamaged; // 마지막으로 데미지를 받은 시간

    private void Start() => Refresh();
    // 초기화: 체력을 최대 체력으로 설정하고 기본 재질로 변경

    private void Update()
    {
        // 일정 시간이 지나거나 B 키가 눌리면 리프레시
        if (Time.time > refreshCooldown + lastTimeDamaged || Input.GetKeyDown(KeyCode.B))
            Refresh();
    }

    private void Refresh()
    {
        // 체력을 최대 체력으로 회복하고 기본 재질로 변경
        currentHealth = maxHealth;
        mesh.sharedMaterial = whiteMat;
    }

    public void TakeDamage(int damage)
    {
        // 데미지를 받았을 때 처리
        lastTimeDamaged = Time.time; // 마지막 데미지 시간을 갱신
        currentHealth -= damage; // 데미지만큼 체력 감소

        if (currentHealth <= 0)
            Die(); // 체력이 0 이하이면 파괴 처리
    }

    private void Die() => mesh.sharedMaterial = redMat;
    // 파괴 처리: 재질을 파괴 상태로 변경
}

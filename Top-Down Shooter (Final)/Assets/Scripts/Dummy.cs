using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Dummy Ŭ����: �������� ���� �� �ִ� ���� ��ü
public class Dummy : MonoBehaviour, IDamagable
{
    public int currentHealth; // ���� ü��
    public int maxHealth = 100; // �ִ� ü��

    [Space]
    public MeshRenderer mesh; // ������ MeshRenderer
    public Material whiteMat; // �⺻ ������ ����
    public Material redMat; // �ı� ������ ����

    [Space]
    public float refreshCooldown; // ü�� �������� ��ٿ�
    private float lastTimeDamaged; // ���������� �������� ���� �ð�

    private void Start() => Refresh();
    // �ʱ�ȭ: ü���� �ִ� ü������ �����ϰ� �⺻ ������ ����

    private void Update()
    {
        // ���� �ð��� �����ų� B Ű�� ������ ��������
        if (Time.time > refreshCooldown + lastTimeDamaged || Input.GetKeyDown(KeyCode.B))
            Refresh();
    }

    private void Refresh()
    {
        // ü���� �ִ� ü������ ȸ���ϰ� �⺻ ������ ����
        currentHealth = maxHealth;
        mesh.sharedMaterial = whiteMat;
    }

    public void TakeDamage(int damage)
    {
        // �������� �޾��� �� ó��
        lastTimeDamaged = Time.time; // ������ ������ �ð��� ����
        currentHealth -= damage; // ��������ŭ ü�� ����

        if (currentHealth <= 0)
            Die(); // ü���� 0 �����̸� �ı� ó��
    }

    private void Die() => mesh.sharedMaterial = redMat;
    // �ı� ó��: ������ �ı� ���·� ����
}

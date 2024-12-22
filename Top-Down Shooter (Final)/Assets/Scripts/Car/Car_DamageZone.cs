using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �ڵ����� �浹 �������� ���ظ� ó���ϴ� Ŭ����

public class Car_DamageZone : MonoBehaviour
{
    private Car_Controller carController; // �ڵ��� ��Ʈ�ѷ� ����

    [SerializeField] private float minSpeedToDamage = 1.5f; // ���ظ� ������ ���� �ּ� �ӵ�
    [SerializeField] private int carDamage; // ���ط�
    [SerializeField] private float impactForce = 150; // ��ݷ�
    [SerializeField] private float upwardsMultiplier = 3; // ����� ���� ����

    private void Awake()
    {
        carController = GetComponentInParent<Car_Controller>(); // �θ� ������Ʈ���� Car_Controller ��������
    }

    private void OnTriggerEnter(Collider other)
    {
        if (carController.rb.velocity.magnitude < minSpeedToDamage)
            return; // �ڵ��� �ӵ��� �ּ� ���� �ӵ����� ������ ����

        IDamagable damagable = other.GetComponent<IDamagable>(); // �浹 ��ü�� ���ظ� ���� �� �ִ��� Ȯ��
        if (damagable == null)
            return; // ���ظ� ���� �� ���� ��ü�� ����

        damagable.TakeDamage(carDamage); // ���ظ� ����

        Rigidbody rb = other.GetComponent<Rigidbody>(); // �浹 ��ü�� Rigidbody ��������
        if (rb != null)
            ApplyForce(rb); // Rigidbody�� �ִٸ� ���� ����
    }

    private void ApplyForce(Rigidbody rigidbody)
    {
        rigidbody.isKinematic = false; // ���������� �����ϵ��� ����
        rigidbody.AddExplosionForce(impactForce, transform.position, 3, upwardsMultiplier, ForceMode.Impulse);
        // ���߷� �߰� (��ġ, �ݰ�, ���� �� ����, �� ���� ���)
    }
}

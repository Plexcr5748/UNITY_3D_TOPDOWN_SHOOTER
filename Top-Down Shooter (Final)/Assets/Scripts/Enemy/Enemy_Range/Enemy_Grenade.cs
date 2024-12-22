using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� ����ź�� �����ϴ� Ŭ����
public class Enemy_Grenade : MonoBehaviour
{
    [SerializeField] private GameObject explosionFx; // ���� ȿ��
    [SerializeField] private float impactRadius; // ���� �ݰ�
    [SerializeField] private float upwardsMultiplier = 1; // ���� �� ���� ���ϴ� ���� ����
    private Rigidbody rb; // ����ź�� Rigidbody
    private float timer; // ����ź ���� Ÿ�̸�
    private float impactPower; // ������ ������ ��

    private LayerMask allyLayerMask; // �Ʊ� ���̾� ����ũ
    private bool canExplode = true; // ���� ���� ����

    private int grenadeDamage; // ����ź�� ������

    // �ʱ�ȭ
    private void Awake() => rb = GetComponent<Rigidbody>();

    // �� ������ Ÿ�̸Ӹ� ������Ʈ�ϰ� ���ǿ� ���� ����
    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0 && canExplode)
            Explode();
    }

    // ���� ó��
    private void Explode()
    {
        canExplode = false;
        PlayExplosionFx(); // ���� ȿ�� ���

        // �ߺ� Ÿ�� ������ ���� �ؽü�
        HashSet<GameObject> uniqueEntities = new HashSet<GameObject>();
        Collider[] colliders = Physics.OverlapSphere(transform.position, impactRadius);

        foreach (Collider hit in colliders)
        {
            IDamagable damagable = hit.GetComponent<IDamagable>();

            if (damagable != null)
            {
                // Ÿ�� ���� ���� �� ����
                if (IsTargetValid(hit) == false)
                    continue;

                // �ߺ� ó�� ����
                GameObject rootEntity = hit.transform.root.gameObject;
                if (uniqueEntities.Add(rootEntity) == false)
                    continue;

                // ������ ó��
                damagable.TakeDamage(grenadeDamage);
            }

            // ������ ���� �� ����
            ApplyPhysicalForceTo(hit);
        }

        // ��ü Ǯ�� ��ȯ
        ObjectPool.instance.ReturnObject(gameObject);
    }

    // �浹ü�� ������ ���� ���� ����
    private void ApplyPhysicalForceTo(Collider hit)
    {
        Rigidbody rb = hit.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = false; // ��ü�� ���������� Ȱ��ȭ
            rb.AddExplosionForce(impactPower, transform.position, impactRadius, upwardsMultiplier, ForceMode.Impulse);
        }
    }

    // ���� ȿ�� ���
    private void PlayExplosionFx()
    {
        GameObject newFx = ObjectPool.instance.GetObject(explosionFx, transform); // ���� ȿ�� ����
        ObjectPool.instance.ReturnObject(newFx, 1); // ���� �ð� �� ȿ�� ��ȯ
        ObjectPool.instance.ReturnObject(gameObject); // ����ź ��ȯ
    }

    // ����ź ����
    public void SetupGrenade(LayerMask allyLayerMask, Vector3 target, float timeToTarget, float countdown, float impactPower, int grenadeDamage)
    {
        canExplode = true;

        this.grenadeDamage = grenadeDamage;
        this.allyLayerMask = allyLayerMask;
        rb.velocity = CalculateLaunchVelocity(target, timeToTarget); // �߻� �ӵ� ���
        timer = countdown + timeToTarget; // ���� Ÿ�̸� ����
        this.impactPower = impactPower;
    }

    // Ÿ���� ��ȿ���� Ȯ��
    private bool IsTargetValid(Collider collider)
    {
        // �Ʊ� ����(friendly fire)�� Ȱ��ȭ�� ��� ��� Ÿ���� ��ȿ
        if (GameManager.instance.friendlyFire)
            return true;

        // �Ʊ� ���̾ ���Ե� ��� ��ȿ���� ����
        if ((allyLayerMask.value & (1 << collider.gameObject.layer)) > 0)
            return false;

        return true;
    }

    // ����ź �߻� �ӵ� ���
    private Vector3 CalculateLaunchVelocity(Vector3 target, float timeToTarget)
    {
        Vector3 direction = target - transform.position; // Ÿ�� ����
        Vector3 directionXZ = new Vector3(direction.x, 0, direction.z); // ���� ���� �и�

        Vector3 velocityXZ = directionXZ / timeToTarget; // ���� �ӵ� ���

        // ���� �ӵ� ���
        float velocityY =
            (direction.y - (Physics.gravity.y * Mathf.Pow(timeToTarget, 2)) / 2) / timeToTarget;

        Vector3 launchVelocity = velocityXZ + Vector3.up * velocityY; // �߻� �ӵ� ����

        return launchVelocity;
    }

    // ���� �ݰ��� �ð������� ǥ�� (����� �뵵)
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, impactRadius);
    }
}

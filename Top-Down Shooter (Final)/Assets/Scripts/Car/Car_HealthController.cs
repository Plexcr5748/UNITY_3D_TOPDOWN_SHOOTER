// �ڵ����� ü�°� ���� ���� ������ �����ϴ� Ŭ����

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_HealthController : MonoBehaviour, IDamagable
{
    private Car_Controller carController; // �ڵ��� ��Ʈ�ѷ� ����

    public int maxHealth; // �ִ� ü��
    public int currentHealth; // ���� ü��

    private bool carBroken; // �ڵ����� �ļ� �������� ����

    [Header("Explosion Info")]
    [SerializeField] private int explosionDamage = 350; // ���߷� ���ϴ� ���ط�
    [Space]
    [SerializeField] private float explosionRadius = 3; // ���� �ݰ�
    [SerializeField] private float explosionDelay = 3; // ���� ���� �ð�
    [SerializeField] private float explosionForce = 7; // ���� ��ݷ�
    [SerializeField] private float explosionUpwardsModifer = 2; // ������ ���� �� ����
    [SerializeField] private Transform explosionPoint; // ���� �߻� ��ġ
    [Space]
    [SerializeField] private ParticleSystem fireFx; // �Ҳ� ȿ��
    [SerializeField] private ParticleSystem explosionFx; // ���� ȿ��

    [Header("Explosion Sound")]
    [SerializeField] private AudioSource explosionAudioSource; // ���� �Ҹ��� ����� AudioSource
    [SerializeField] private AudioClip explosionSoundClip; // ���� ���� Ŭ��

    private void Start()
    {
        carController = GetComponent<Car_Controller>(); // Car_Controller ������Ʈ ��������
        currentHealth = maxHealth; // ���� ü���� �ִ� ü������ �ʱ�ȭ

        // ���� ����� �ҽ��� ���� ��� ������Ʈ���� ��������
        if (explosionAudioSource == null)
        {
            explosionAudioSource = GetComponent<AudioSource>();
        }
    }

    private void Update()
    {
        // �Ҳ� ȿ���� Ȱ��ȭ�� ��� ȸ���� ����
        if (fireFx.gameObject.activeSelf)
        {
            fireFx.transform.rotation = Quaternion.identity;
        }
    }

    public void UpdateCarHealthUI()
    {
        // UI�� ü�� ���� ������Ʈ
        UI.instance.inGameUI.UpdateCarHealthUI(currentHealth, maxHealth);
    }

    private void ReduceHealth(int damage)
    {
        if (carBroken)
            return; // �̹� �ļյ� ��� ü�� ���� ����

        currentHealth -= damage; // ���ط���ŭ ü�� ����

        if (currentHealth < 0)
            BrakeTheCar(); // ü���� 0 ���ϰ� �Ǹ� �ڵ��� �ļ� ó��
    }

    private void BrakeTheCar()
    {
        carBroken = true; // �ڵ��� �ļ� ���� ����
        carController.BrakeTheCar(); // �ڵ��� ���� ó��

        fireFx.gameObject.SetActive(true); // �Ҳ� ȿ�� Ȱ��ȭ
        StartCoroutine(ExplosionCo(explosionDelay)); // ���� ó�� �ڷ�ƾ ����
    }

    public void TakeDamage(int damage)
    {
        ReduceHealth(damage); // ü�� ���� ó��
        UpdateCarHealthUI(); // UI ������Ʈ
    }

    private IEnumerator ExplosionCo(float delay)
    {
        yield return new WaitForSeconds(delay); // ���� ����

        explosionFx.gameObject.SetActive(true); // ���� ȿ�� Ȱ��ȭ
        carController.rb.AddExplosionForce(explosionForce, explosionPoint.position,
            explosionRadius, explosionUpwardsModifer, ForceMode.Impulse); // ���� ���� ȿ�� ����

        Explode(); // ���� ���� ó��

        PlayExplosionSound(); // ���� ���� ���
    }

    private void Explode()
    {
        HashSet<GameObject> uniqueEntities = new HashSet<GameObject>(); // �ߺ� ó�� ������ ���� ����

        Collider[] colliders = Physics.OverlapSphere(explosionPoint.position, explosionRadius); // ���� �ݰ� �� ��� �浹ü ��������

        foreach (Collider hit in colliders)
        {
            IDamagable damagable = hit.GetComponent<IDamagable>(); // ���ظ� ���� �� �ִ� ��ü Ȯ��

            if (damagable != null)
            {
                GameObject rootEntity = hit.transform.root.gameObject; // ��Ʈ ��ü ��������

                if (uniqueEntities.Add(rootEntity) == false)
                    continue; // �̹� ó���� ��ü�� ����

                damagable.TakeDamage(explosionDamage); // ���� ���� ����

                hit.GetComponentInChildren<Rigidbody>()?.AddExplosionForce(
                    explosionForce, explosionPoint.position, explosionRadius,
                    explosionUpwardsModifer, ForceMode.VelocityChange); // ���� ��ݷ� ����
            }
        }
    }

    private void PlayExplosionSound()
    {
        // ���� ���尡 ��ȿ�� ��� ���
        if (explosionSoundClip != null && explosionAudioSource != null)
        {
            explosionAudioSource.PlayOneShot(explosionSoundClip);
        }
    }

    private void OnDrawGizmos()
    {
        // ���� �ݰ��� ������ ǥ��
        Gizmos.DrawWireSphere(explosionPoint.position, explosionRadius);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ZoneLimitation: Ư�� ���������� ���� �� ȿ���� �����ϴ� Ŭ����
public class ZoneLimitation : MonoBehaviour
{
    private ParticleSystem[] lines; // �ڽ� ������Ʈ���� ParticleSystem �迭
    private BoxCollider zoneCollider; // ������ �����ϴ� BoxCollider

    private void Start()
    {
        // MeshRenderer ��Ȱ��ȭ (�ð������� ������ �ʰ� ����)
        GetComponent<MeshRenderer>().enabled = false;

        // BoxCollider �� �ڽ� ParticleSystem ������Ʈ ��������
        zoneCollider = GetComponent<BoxCollider>();
        lines = GetComponentsInChildren<ParticleSystem>();

        // �ʱ� ���·� �� ��Ȱ��ȭ
        ActivateWall(false);
    }

    private void ActivateWall(bool activate)
    {
        // ���� Ȱ��ȭ/��Ȱ��ȭ ó��
        foreach (var line in lines)
        {
            if (activate)
            {
                line.Play(); // ParticleSystem Ȱ��ȭ
            }
            else
            {
                line.Stop(); // ParticleSystem ��Ȱ��ȭ
            }
        }

        // Collider�� Ʈ���ŷ� ���� (Ȱ��ȭ ���ο� ���� ����)
        zoneCollider.isTrigger = !activate;
    }

    IEnumerator WallActivationCo()
    {
        // ���� Ȱ��ȭ�ϰ� 1�� �� ��Ȱ��ȭ
        ActivateWall(true);

        yield return new WaitForSeconds(1);

        ActivateWall(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ʈ���ſ� �ٸ� ������Ʈ�� �����ϸ� �� Ȱ��ȭ �ڷ�ƾ ����
        StartCoroutine(WallActivationCo());
        Debug.Log("My sensors are going crazy, I think it's dangerous!");
    }
}

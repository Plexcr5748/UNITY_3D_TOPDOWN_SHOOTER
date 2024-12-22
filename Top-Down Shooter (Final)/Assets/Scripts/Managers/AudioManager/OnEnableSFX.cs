using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// OnEnableSFX: ������Ʈ�� Ȱ��ȭ�� �� ���带 ����ϴ� ��ũ��Ʈ
public class OnEnableSFX : MonoBehaviour
{
    [SerializeField] private AudioSource sfx; // ����� ����� �ҽ�
    [SerializeField] private float volume = .3f; // ���� ����
    [SerializeField] private float minPitch = .85f; // �ּ� ��ġ
    [SerializeField] private float maxPitch = 1.1f; // �ִ� ��ġ

    // ������Ʈ�� Ȱ��ȭ�� �� ȣ��
    private void OnEnable()
    {
        PlaySFX(); // ���� ���
    }

    // ���� ��� ����
    private void PlaySFX()
    {
        if (sfx == null) // ����� �ҽ��� �������� ���� ��� ��ȯ
            return;

        float pitch = Random.Range(minPitch, maxPitch); // ���� ��ġ ����
        sfx.pitch = pitch;
        sfx.volume = volume;
        sfx.Play(); // ���� ���
    }
}

// �ڵ����� ���� ȿ���� �����ϴ� Ŭ����

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_Sounds : MonoBehaviour
{
    private Car_Controller car; // �ڵ��� ��Ʈ�ѷ� ����

    [SerializeField] private float engineVolume = .07f; // ���� ���� ����
    [SerializeField] private AudioSource engineStart; // ���� ���� ����
    [SerializeField] private AudioSource workingEngine; // �۵� ���� ���� ����
    [SerializeField] private AudioSource engineOff; // ���� ���� ����

    private float maxSpeed = 10; // �ڵ��� �ִ� �ӵ�

    public float minPitch = .75f; // ���� ������ �ּ� ��ġ
    public float maxPitch = 1.5f; // ���� ������ �ִ� ��ġ

    private bool allowCarSounds; // �ڵ��� ���� Ȱ��ȭ ����

    private void Start()
    {
        car = GetComponent<Car_Controller>(); // Car_Controller ������Ʈ ��������
        Invoke(nameof(AllowCarSounds), 1); // 1�� �� ���� ��� ����
    }

    private void Update()
    {
        UpdateEngineSound(); // �� ������ ���� ���� ������Ʈ
    }

    private void UpdateEngineSound()
    {
        // �ڵ��� �ӵ��� ���� ���� ���� ��ġ�� ����
        float currentSpeed = car.speed;
        float pitch = Mathf.Lerp(minPitch, maxPitch, currentSpeed / maxSpeed); // �ӵ��� ����� ��ġ ���
        workingEngine.pitch = pitch; // ���� ���忡 ��ġ ����
    }

    public void ActivateCarSFX(bool activate)
    {
        // �ڵ��� ���� Ȱ��ȭ �Ǵ� ��Ȱ��ȭ
        if (allowCarSounds == false)
            return; // ���尡 ������ ���� ��� ����

        if (activate)
        {
            engineStart.Play(); // ���� ���� ���� ���
            AudioManager.instance.SFXDelayAndFade(workingEngine, true, engineVolume, 1); // ���� �۵� ���� ���̵� ��
        }
        else
        {
            AudioManager.instance.SFXDelayAndFade(workingEngine, false, 0f, .25f); // ���� �۵� ���� ���̵� �ƿ�
            engineOff.Play(); // ���� ���� ���� ���
        }
    }

    private void AllowCarSounds() => allowCarSounds = true; // ���� ��� ����
}

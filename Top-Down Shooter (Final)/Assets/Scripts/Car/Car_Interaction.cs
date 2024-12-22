// �ڵ������� ��ȣ�ۿ��� ó���ϴ� Ŭ����

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_Interaction : Interactable
{
    private Car_HealthController carHealthController; // �ڵ��� ü�� ����
    private Car_Controller carController; // �ڵ��� ��Ʈ�ѷ� ����
    private Transform player; // �÷��̾� ĳ���� ����

    private float defaultPlayerScale; // �÷��̾��� �⺻ ũ�� ����

    [Header("Exit details")]
    [SerializeField] private float exitCheckRadius = .2f; // �ⱸ �浹 üũ �ݰ�
    [SerializeField] private Transform[] exitPoints; // �ڵ������� ���� �� �ִ� ������
    [SerializeField] private LayerMask whatToIngoreForExit; // �ⱸ Ȯ�� �� ������ ���̾�

    private void Start()
    {
        carHealthController = GetComponent<Car_HealthController>(); // �ڵ��� ü�� ���� ������Ʈ ��������
        carController = GetComponent<Car_Controller>(); // �ڵ��� ��Ʈ�ѷ� ������Ʈ ��������
        player = GameManager.instance.player.transform; // ���� �� �÷��̾� ���� ��������

        // �ⱸ ������ �������� �浹ü�� ��Ȱ��ȭ
        foreach (var point in exitPoints)
        {
            point.GetComponent<MeshRenderer>().enabled = false;
            point.GetComponent<SphereCollider>().enabled = false;
        }
    }

    private void Update()
    {
        // �ڵ����� �ı��Ǿ��� ��� ���̶���Ʈ�� �⺻ ���·� ����
        if (carController.carDead == true)
        {
            highlightMaterial = defaultMaterial;
        }
    }

    public override void Interaction()
    {
        base.Interaction(); // ���� Ŭ������ Interaction ȣ��
        GetIntoTheCar(); // �ڵ��� ž�� ó��
    }

    private void GetIntoTheCar()
    {
        if (carController.carDead == true)
            return; // �ڵ����� �ı��Ǿ��ٸ� �������� ����

        ControlsManager.instance.SwitchToCarControls(); // �ڵ��� �������� �Է� ����
        carHealthController.UpdateCarHealthUI(); // �ڵ��� ü�� UI ������Ʈ
        carController.ActivateCar(true); // �ڵ��� Ȱ��ȭ

        defaultPlayerScale = player.localScale.x; // �÷��̾��� �⺻ ũ�� ����

        // �÷��̾ �ڵ��� ���η� ��� �� ��ġ �̵�
        player.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        player.transform.parent = transform;
        player.transform.localPosition = Vector3.up / 2;

        CameraManager.instance.ChangeCameraTarget(transform, 12, .5f); // ī�޶� ��ġ ����
    }

    public void GetOutOfTheCar()
    {
        if (carController.carActive == false)
            return; // �ڵ����� Ȱ��ȭ���� �ʾҴٸ� �������� ����

        carController.ActivateCar(false); // �ڵ��� ��Ȱ��ȭ

        player.parent = null; // �÷��̾� �θ� ����
        player.position = GetExitPoint(); // �ⱸ �������� ��ġ �̵�
        player.transform.localScale = new Vector3(defaultPlayerScale, defaultPlayerScale, defaultPlayerScale); // �÷��̾� ũ�� ����

        ControlsManager.instance.SwitchToCharacterControls(); // ĳ���� �������� �Է� ����
        Player_AimController aim = GameManager.instance.player.aim; // �÷��̾� ���� ��Ʈ�ѷ� ��������

        CameraManager.instance.ChangeCameraTarget(aim.GetAimCameraTarget(), 8.5f); // ī�޶� ��ġ ����
    }

    private Vector3 GetExitPoint()
    {
        // �ⱸ ���� �� ���� ����� ��� �ִ� ���� ��ȯ
        for (int i = 0; i < exitPoints.Length; i++)
        {
            if (IsExitClear(exitPoints[i].position))
                return exitPoints[i].position;
        }

        return exitPoints[0].position; // ��� �ִ� ������ ������ ù ��° ���� ��ȯ
    }

    private bool IsExitClear(Vector3 point)
    {
        // �ⱸ ������ ��� �ִ��� Ȯ��
        Collider[] colliders = Physics.OverlapSphere(point, exitCheckRadius, ~whatToIngoreForExit);
        return colliders.Length == 0;
    }

    private void OnDrawGizmos()
    {
        // �ⱸ ������ �����(������) ǥ��
        if (exitPoints.Length > 0)
        {
            foreach (var point in exitPoints)
            {
                Gizmos.DrawWireSphere(point.position, exitCheckRadius);
            }
        }
    }
}

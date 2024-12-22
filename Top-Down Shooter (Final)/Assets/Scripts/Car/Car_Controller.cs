// �ڵ����� ���� �� ������ ó���� �����ϴ� ���� ��Ʈ�ѷ� Ŭ����

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

// �ڵ��� ���� ����� �����ϴ� ������
public enum DriveType { FrontWheelDrive, RearWheelDrive, AllWheelDrive }

[RequireComponent(typeof(NavMeshObstacle))] // NavMesh���� �浹 ó���� ���� ������Ʈ
[RequireComponent(typeof(Car_HealthController))] // �ڵ����� ü�� ���� ������Ʈ
[RequireComponent(typeof(Car_Interaction))] // �÷��̾�� �ڵ��� ���� ��ȣ�ۿ� ������Ʈ
[RequireComponent(typeof(BoxCollider))] // �浹 ó���� ���� �ݶ��̴�
[RequireComponent(typeof(Rigidbody))] // �ڵ����� ������ ������ ó���ϴ� ������Ʈ
public class Car_Controller : MonoBehaviour
{
    public Car_Sounds carSounds { get; private set; } // �ڵ��� ���� ����
    public Rigidbody rb { get; private set; } // �ڵ����� Rigidbody ����
    public bool carActive { get; private set; } // �ڵ��� Ȱ��ȭ ����
    private PlayerControls controls; // �÷��̾� �Է� ��Ʈ�� ����
    private float moveInput; // ����/���� �Է� ��
    private float steerInput; // ���� �Է� ��

    public bool carDead = false; // �ڵ��� �ı� ����

    [SerializeField] private LayerMask whatIsGround; // �ٴ����� �ν��� ���̾�

    public float speed; // �ڵ����� ���� �ӵ�

    [Range(30, 60)]
    [SerializeField] private float turnSensetivity = 30; // ���� �ΰ���

    [Header("Car Settings")]
    [SerializeField] private DriveType driveType; // �ڵ����� ���� ���
    [SerializeField] private Transform centerOfMass; // �ڵ����� ���� �߽�
    [Range(350, 1000)]
    [SerializeField] private float carMass = 400; // �ڵ����� ����
    [Range(20, 80)]
    [SerializeField] private float wheelsMass = 30; // ������ ����
    [Range(.5f, 2f)]
    [SerializeField] private float frontWheelTraction = 1; // ���� ������
    [Range(.5f, 2f)]
    [SerializeField] private float backWheelTraction = 1; // �ķ� ������

    [Header("Engine Settings")]
    [SerializeField] private float currentSpeed; // ���� �ӵ�
    [Range(7, 12)]
    [SerializeField] private float maxSpeed = 7; // �ִ� �ӵ�
    [Range(.5f, 10)]
    [SerializeField] private float accleerationSpeed = 2; // ���ӵ�
    [Range(1500, 5000)]
    [SerializeField] private float motorForce = 1500f; // ������ ��� ��ũ

    [Header("Brakes Settings")]
    [Range(0, 10)]
    [SerializeField] private float frontBrakesSensetivity = 5; // ���� �극��ũ �ΰ���
    [Range(0, 10)]
    [SerializeField] private float backBrakesSensetivity = 5; // �ķ� �극��ũ �ΰ���
    [Range(4000, 6000)]
    [SerializeField] private float brakePower = 5000; // �극��ũ ���
    private bool isBraking; // �극��ũ ����

    [Header("Drift Settings")]
    [Range(0, 1)]
    [SerializeField] private float frontDriftFactor = .5f; // ���� �帮��Ʈ ���
    [Range(0, 1)]
    [SerializeField] private float backDriftFactor = .5f; // �ķ� �帮��Ʈ ���
    [SerializeField] private float driftDuration = 1f; // �帮��Ʈ ���� �ð�
    private float driftTimer; // �帮��Ʈ Ÿ�̸�
    private bool isDrifting; // �帮��Ʈ ����
    private bool canEmitTrails = true; // Ʈ���� Ȱ��ȭ ����

    private Car_Wheel[] wheels; // �ڵ��� ���� ����
    private UI ui; // UI ���� ����

    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // Rigidbody �ʱ�ȭ
        wheels = GetComponentsInChildren<Car_Wheel>(); // ��� �ڽ� ���� ��������
        carSounds = GetComponent<Car_Sounds>(); // �ڵ��� ���� ������Ʈ ��������
        ui = UI.instance; // UI �ν��Ͻ� ����

        controls = ControlsManager.instance.controls; // �÷��̾� ��Ʈ�� ��������

        AssignInputEvents(); // �Է� �̺�Ʈ ����
        SetupDefaultValues(); // �⺻�� �ʱ�ȭ
        ActivateCar(false); // �ڵ��� ��Ȱ��ȭ ���·� ����
    }

    private void SetupDefaultValues()
    {
        rb.centerOfMass = centerOfMass.localPosition; // Rigidbody�� �߽� ����
        rb.mass = carMass; // �ڵ��� ���� ����

        foreach (var wheel in wheels)
        {
            wheel.cd.mass = wheelsMass; // �� ������ ���� ����

            if (wheel.axelType == AxelType.Front)
                wheel.SetDefaultStiffnes(frontWheelTraction); // ���� �⺻ ������ ����

            if (wheel.axelType == AxelType.Back)
                wheel.SetDefaultStiffnes(backWheelTraction); // �ķ� �⺻ ������ ����
        }
    }

    private void Update()
    {
        if (carActive == false)
            return; // �ڵ����� ��Ȱ��ȭ ���¸� �������� ����

        speed = rb.velocity.magnitude; // ���� �ӵ� ���
        ui.inGameUI.UpdateSpeedText(Mathf.RoundToInt(speed * 10) + "km/h"); // �ӵ� UI ������Ʈ

        driftTimer -= Time.deltaTime; // �帮��Ʈ Ÿ�̸� ����

        if (driftTimer < 0)
            isDrifting = false; // �帮��Ʈ ����
    }

    private void FixedUpdate()
    {
        if (carActive == false)
            return; // �ڵ����� ��Ȱ��ȭ ���¸� �������� ����

        ApplyTrailsOnTheGround(); // Ʈ���� ȿ�� ����
        ApplyAnimationToWheels(); // ���� �ִϸ��̼� ����
        ApplyDrive(); // �̵� ���� ó��
        ApplySteering(); // ���� ó��
        ApplyBrakes(); // �극��ũ ó��
        ApplySpeedLimit(); // �ӵ� ���� ó��

        if (isDrifting)
            ApplyDrift(); // �帮��Ʈ ����
        else
            StopDrift(); // �帮��Ʈ ����
    }

    private void ApplyDrive()
    {
        currentSpeed = moveInput * accleerationSpeed * Time.deltaTime; // �Է°��� ���� �ӵ� ���

        float motorTorqueValue = motorForce * currentSpeed; // ���� ��ũ ���

        foreach (var wheel in wheels)
        {
            if (driveType == DriveType.FrontWheelDrive)
            {
                if (wheel.axelType == AxelType.Front)
                    wheel.cd.motorTorque = motorTorqueValue; // ���� ���� �� �������� ��ũ ����
            }
            else if (driveType == DriveType.RearWheelDrive)
            {
                if (wheel.axelType == AxelType.Back)
                    wheel.cd.motorTorque = motorTorqueValue; // �ķ� ���� �� �ķ����� ��ũ ����
            }
            else
            {
                wheel.cd.motorTorque = motorTorqueValue; // ��� ���� �� ��� ������ ��ũ ����
            }
        }
    }

    private void ApplySpeedLimit()
    {
        // �ڵ��� �ӵ��� �ִ� �ӵ��� �ʰ����� �ʵ��� ����
        if (rb.velocity.magnitude > maxSpeed)
            rb.velocity = rb.velocity.normalized * maxSpeed;
    }

    private void ApplySteering()
    {
        // ���� �Է°��� ���� ���� ������ ������ ����
        foreach (var wheel in wheels)
        {
            if (wheel.axelType == AxelType.Front)
            {
                float targetSteerAngle = steerInput * turnSensetivity; // ��ǥ ���� ���� ���
                wheel.cd.steerAngle = Mathf.Lerp(wheel.cd.steerAngle, targetSteerAngle, .5f); // �ε巴�� ���� ����
            }
        }
    }

    private void ApplyBrakes()
    {
        // �극��ũ �Է°��� ���� ������ ���� ��ũ�� ����
        foreach (var wheel in wheels)
        {
            bool frontBrakes = wheel.axelType == AxelType.Front; // ���� ���� Ȯ��
            float brakeSensetivity = frontBrakes ? frontBrakesSensetivity : backBrakesSensetivity; // �ΰ��� ����

            float newBrakeTorque = brakePower * brakeSensetivity * Time.deltaTime; // ���� ��ũ ���
            float currentBrakeTorque = isBraking ? newBrakeTorque : 0; // �극��ũ�� Ȱ��ȭ�� ��츸 ����

            wheel.cd.brakeTorque = currentBrakeTorque; // ���� ��ũ ����
        }
    }

    private void ApplyDrift()
    {
        // �帮��Ʈ ������ �� ������ ���� �������� ����
        foreach (var wheel in wheels)
        {
            bool frontWheel = wheel.axelType == AxelType.Front;
            float driftFactor = frontWheel ? frontDriftFactor : backDriftFactor; // ���� ��ġ�� ���� �帮��Ʈ ���

            WheelFrictionCurve sidewaysFriction = wheel.cd.sidewaysFriction;
            sidewaysFriction.stiffness *= (1 - driftFactor); // ������ ����
            wheel.cd.sidewaysFriction = sidewaysFriction; // ����� �� ����
        }
    }

    private void StopDrift()
    {
        // �帮��Ʈ ���¸� �����ϰ� �⺻ ������ ����
        foreach (var wheel in wheels)
        {
            wheel.RestoreDefaultStiffnes();
        }
    }

    private void ApplyAnimationToWheels()
    {
        // ������ �ִϸ��̼ǰ� ���� ������ ��ġ�� ����ȭ
        foreach (var wheel in wheels)
        {
            Quaternion rotation;
            Vector3 position;

            wheel.cd.GetWorldPose(out position, out rotation); // ���������� ���� ��ġ�� ȸ�� �� ��������

            if (wheel.model != null)
            {
                wheel.model.transform.position = position; // ���� ��ġ ������Ʈ
                wheel.model.transform.rotation = rotation; // ���� ȸ�� ������Ʈ
            }
        }
    }

    private void ApplyTrailsOnTheGround()
    {
        // ������ ���鿡 Ʈ������ ���� �� �ִ� �������� Ȯ�� �� ����
        if (canEmitTrails == false)
            return;

        foreach (var wheel in wheels)
        {
            WheelHit hit;

            if (wheel.cd.GetGroundHit(out hit)) // ������ ����� ���� ������ Ȯ��
            {
                if (whatIsGround == (whatIsGround | (1 << hit.collider.gameObject.layer)))
                {
                    wheel.trail.emitting = true; // ������ ���� ���̾ ���� �� Ʈ���� Ȱ��ȭ
                }
                else
                {
                    wheel.trail.emitting = false; // ������ �ƴ� ��� Ʈ���� ��Ȱ��ȭ
                }
            }
            else
            {
                wheel.trail.emitting = false; // ���� ������ ���� ��� Ʈ���� ��Ȱ��ȭ
            }
        }
    }

    public void ActivateCar(bool activate)
    {
        carActive = activate; // �ڵ��� Ȱ��ȭ ���� ����

        if (carSounds != null)
            carSounds.ActivateCarSFX(activate); // ���� ���µ� ����ȭ
    }

    public void BrakeTheCar()
    {
        // �ڵ����� ���� ���·� ��ȯ�ϰ� �ʿ��� ���� ����
        canEmitTrails = false; // Ʈ���� ��Ȱ��ȭ
        carDead = true; // �ڵ��� �ı� ����
        foreach (var wheel in wheels)
        {
            wheel.trail.emitting = false; // ��� ���� Ʈ���� ��Ȱ��ȭ
        }

        rb.drag = 1; // �巡�� ����
        motorForce = 0; // ���� ��� ��Ȱ��ȭ
        isDrifting = true; // �帮��Ʈ ���·� ��ȯ
        frontDriftFactor = .9f; // ���� �帮��Ʈ ��� ����
        backDriftFactor = .9f; // �ķ� �帮��Ʈ ��� ����
    }

    private void AssignInputEvents()
    {
        // �÷��̾� �Է� �̺�Ʈ ����
        controls.Car.Movement.performed += ctx =>
        {
            Vector2 input = ctx.ReadValue<Vector2>(); // �Է� �� �б�

            moveInput = input.y; // ����/���� �Է�
            steerInput = input.x; // ���� �Է�
        };

        controls.Car.Movement.canceled += ctx =>
        {
            moveInput = 0; // �Է��� �����Ǹ� �ʱ�ȭ
            steerInput = 0;
        };

        controls.Car.Brake.performed += ctx =>
        {
            isBraking = true; // �극��ũ Ȱ��ȭ
            isDrifting = true; // �帮��Ʈ ����
            driftTimer = driftDuration; // �帮��Ʈ ���� �ð� ����
        };
        controls.Car.Brake.canceled += ctx => isBraking = false; // �극��ũ ����

        controls.Car.CarExit.performed += ctx => GetComponent<Car_Interaction>().GetOutOfTheCar(); // �ڵ������� ������
    }

    [ContextMenu("Focus camera and enable")]
    public void TestThisCar()
    {
        ActivateCar(true); // �ڵ��� Ȱ��ȭ
        CameraManager.instance.ChangeCameraTarget(transform, 12); // ī�޶� �ڵ����� ��ȯ
    }
}

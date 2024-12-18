using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Sensors.Reflection;
using Unity.MLAgents.Demonstrations;
using Unity.MLAgents.Policies;
using Unity.MLAgents;
using static UnityEngine.ParticleSystem;
using System;

public class StaffAgent_S2_V1 : Agent
{
    public float moveSpeed = 5f;
    public float turnSpeed = 180f;

    private Rigidbody staffRigidbody;

    public Transform foodArea;
    public Transform pointArea;

    public GameObject itemItitPos;
    public GameObject staffItemHolder;
    public GameObject fishTrayPoint;
    public GameObject lemonTrayPoint;

    public GameObject fishModel;
    public GameObject lemonModel;
    public GameObject trayModel;

    private Vector3 originStaffPos;

    private Vector3 originalFishPos;
    private Quaternion originalFishRot;
    private Vector3 originalLemonPos;
    private Quaternion originalLemonRot;
    private Vector3 originalTrayPos;
    private Quaternion originalTrayRot;

    private bool isHold; // true���, agent�� ������Ʈ�� ��� ����

    // fish�� tray�� ������ �� ������ �Ǵ�
    private bool isFishOnTray;
    private bool isLemonOnTray;

    public override void Initialize()
    {
        staffRigidbody = GetComponent<Rigidbody>();

        isHold = false;
        isFishOnTray = false;

        originStaffPos = transform.localPosition;

        originalFishPos = fishModel.transform.localPosition;
        originalFishRot = fishModel.transform.localRotation;
        originalLemonPos = lemonModel.transform.localPosition;
        originalLemonRot = lemonModel.transform.localRotation;
        originalTrayPos = trayModel.transform.localPosition;
        originalTrayRot = trayModel.transform.localRotation;
    }

    public override void OnEpisodeBegin()
    {
        // #1 ������ ��ġ �ʱ�ȭ
        // #2 ���� ��ġ �ʱ�ȭ
        // #3 Ʈ���� ��ġ �ʱ�ȭ
        ResetItem();

        // #4 agent ��ġ �ʱ�ȭ -> velocity, angularVelocity = 0
        ResetStaff();

        // #5 isHold ���� false�� �ʱ�ȭ
        // #6 isFishOnTray ���� false�� �ʱ�ȭ
        // #7 isLemonOnTray ���� false�� �ʱ�ȭ
        isHold = false;
        isFishOnTray = false;
        isLemonOnTray = false;
    }

    // �����ؾ��ϴ� �� (�Ѱ� ��� �ϴ� ����)(���� ���� ���)
    public override void CollectObservations(VectorSensor sensor)
    {
        // #1 isHold ��
        // #2 agent�� foodArea ������ �Ÿ�
        // #3 agent�� trayArea ������ �Ÿ�
        // #4 agent�� pointArea ������ �Ÿ�
        // #5 agent�� �ٶ󺸴� ����

        sensor.AddObservation(transform.forward);

        sensor.AddObservation(isHold);
        sensor.AddObservation(isFishOnTray ? 1 : 0);
        sensor.AddObservation(isLemonOnTray ? 1 : 0);

        // #2 agent�� foodArea ������ ����� �Ÿ�
        sensor.AddObservation((foodArea.localPosition - transform.localPosition).normalized);
        sensor.AddObservation(Vector3.Distance(foodArea.position, transform.position));

        // #3 agent�� trayArea ������ ����� �Ÿ�
        sensor.AddObservation((trayModel.transform.localPosition - transform.localPosition).normalized);
        sensor.AddObservation(Vector3.Distance(trayModel.transform.position, transform.position));

        // #4 agent�� pointArea ������ ����� �Ÿ�
        sensor.AddObservation((pointArea.localPosition - transform.localPosition).normalized);
        sensor.AddObservation(Vector3.Distance(pointArea.position, transform.position));
    }

    // ��å�� ���ؼ� ������ �ൿ ��ħ (�޾ƾ� �ϴ� ����)
    // ��å ����
    public override void OnActionReceived(ActionBuffers actions)
    {
        //forwardAmount & turnAmount ����
        float forwardAmount;
        float turnAmount = 0;

        var disActions = actions.DiscreteActions;

        forwardAmount = disActions[0];

        if (disActions[1] == 1f)
        {
            turnAmount = -1f;
        }
        else if (disActions[1] == 2f)
        {
            turnAmount = +1f;
        }

        // forwardAmount & turnAmount ����
        staffRigidbody.MovePosition(transform.position + transform.forward * forwardAmount * moveSpeed * Time.fixedDeltaTime);
        transform.Rotate(transform.up * turnAmount * turnSpeed * Time.fixedDeltaTime);

        // �� Step���� ���� �г�Ƽ �ο�
        if (MaxStep > 0)
        {
            AddReward(-0.001f);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        int forwardAction = 0;
        int turnAction = 0;

        // ���� ����Ű: forwardAction=1, ���� ����Ű: turnAction=1, ������ ����Ű: turnAction=2 

        if (Input.GetKey(KeyCode.UpArrow))
        {
            forwardAction = 1;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            turnAction = 1;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            turnAction = 2;
        }

        actionsOut.DiscreteActions.Array[0] = forwardAction;
        actionsOut.DiscreteActions.Array[1] = turnAction;
    }

    private void ResetItem()
    {
        // #1 ������ ��ġ �ʱ�ȭ
        fishModel.transform.SetParent(itemItitPos.transform);
        fishModel.transform.SetLocalPositionAndRotation(originalFishPos, originalFishRot);

        // #2 ���� ��ġ �ʱ�ȭ
        lemonModel.transform.SetParent(itemItitPos.transform);
        lemonModel.transform.SetLocalPositionAndRotation(originalLemonPos, originalLemonRot);

        // #3 Ʈ���� ��ġ �ʱ�ȭ
        trayModel.transform.SetParent(itemItitPos.transform);
        trayModel.transform.SetLocalPositionAndRotation(originalTrayPos, originalTrayRot);
    }

    private void ResetStaff()
    {
        // #4 agent ��ġ �ʱ�ȭ -> velocity, angularVelocity = 0
        Rigidbody rigidbody = staffRigidbody.GetComponent<Rigidbody>();
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;

        // position, rot�� �ʱ�ȭ!
        transform.localPosition = originStaffPos;
        transform.localRotation = Quaternion.identity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // #1 ���̺��� tag ���� ��������

        // #2-1 ���̺��� tag ������ fish�� ���
        if (collision.transform.CompareTag("FishTable"))
        {
            // 1. ���� �ƹ� �͵� ��� ���� �ʴٸ� (isHold = false)
            // (isFishOnTray false���)
            // 1) ��� �ִ� ���·� ��ȯ : isHold = true
            // 2) fish�� player �Ӹ� ���� �ű�
            // ���� �ڵ� : targetObject.transform.SetParent(parentObject.transform);
            // 3) +1 ����
            // (isFishOnTray true��� �״�� ����)
            // 2. ���� ���� �� ��� �ִٸ� �״�� ����(�ݸ��� �ѱ�)

            if (isHold)
            {
                AddReward(-0.05f);
                return;
            }

            if (!isFishOnTray)
            {
                isHold = true;

                // 2) fish�� player �Ӹ� ���� �ű�
                fishModel.transform.SetParent(staffItemHolder.transform);
                fishModel.transform.localPosition = Vector3.zero;
                fishModel.transform.localRotation = Quaternion.identity;

                AddReward(+1);
            }
            else
            {
                AddReward(-0.05f);
                return;
            }
        }
        // #2-2 ���̺��� tag ������ lemon�� ���
        else if (collision.transform.CompareTag("LemonTable"))
        {
            // 1. ���� �ƹ� �͵� ��� ���� �ʴٸ� (isHold = false)
            // 1) isFishOnTray true���� �Ǵ�
            // 1-1) True���
            // (1) ��� �ִ� ���·� ��ȯ isHold = true
            // (2) lemon�� player �Ӹ� ���� �ű�
            // (3) +1 ����
            // 1-2) False��� �״�� ����
            // 2. ���� ���� ��� �ִٸ� �״�� ����

            if (isHold)
            {
                AddReward(-0.05f);
                return;
            }
            if (!isFishOnTray || isLemonOnTray)
            {
                AddReward(-0.05f);
                return;
            }
            else
            {
                isHold = true;

                // (2) lemon�� player �Ӹ� ���� �ű�
                lemonModel.transform.SetParent(staffItemHolder.transform);
                lemonModel.transform.localPosition = Vector3.zero;
                lemonModel.transform.localRotation = Quaternion.identity;

                AddReward(+1);
            }
        }
        // #2-3 ���̺��� tag ������ tray�� ���
        else if (collision.transform.CompareTag("TrayTable"))
        {
            // 1. ���� �ƹ� �͵� ��� ���� �ʴٸ� (isHold = false)
            // 1) ������ Ʈ���� ���� �׿��ٸ� tray�� player �Ӹ� ���� �ű� (�̶�, isHold = true ����)
            // 2) ������ Ʈ���� ���� ������ �ʾҴٸ� �״�� ����
            // 2. ���� ���� ��� �ִٸ� (isHold = true)
            // ���� ������ Ʈ���� ���� ������ �ʾҴٸ�
            // 1) isFishOnTray true���� �Ǵ�
            // 1-1) False���
            // (1) ��� ���� ���� ���·� ��ȯ : isHold = false
            // (2) ��� �ִ� ������Ʈ(������)�� Tray�� �ű�
            // (3) isPickedFish�� true�� ����
            // (4) +1 ����
            // 1-2) True���
            // (1) ��� ���� ���� ���·� ��ȯ : isHold = false
            // (2) ��� �ִ� ������Ʈ(����)�� Tray�� �ű�
            // (3) +1 ����
            // (4) ���Ǽҵ� ����
            // ���� ������ Ʈ���� ���� �׿��ٸ�(=Ʈ���̸� ��� �ִٸ�) �״�� ����


            if (!isHold)
            {
                if (!isLemonOnTray)
                {
                    AddReward(-0.05f);
                    return;
                }
                else
                {
                    isHold = true;

                    // tray�� player �Ӹ� ���� �ű�
                    trayModel.transform.SetParent(staffItemHolder.transform);
                    trayModel.transform.localPosition = Vector3.zero;
                    trayModel.transform.localRotation = Quaternion.identity;

                    AddReward(+1);
                }
            }
            else
            {
                if (!isLemonOnTray)
                {
                    isHold = false;
                    if (!isFishOnTray)
                    {
                        // (2) ��� �ִ� ������Ʈ(������)�� Tray�� �ű�
                        fishModel.transform.SetParent(fishTrayPoint.transform);
                        fishModel.transform.localPosition = Vector3.zero;
                        fishModel.transform.localRotation = Quaternion.identity;

                        isFishOnTray = true;
                        AddReward(+1);
                    }
                    else
                    {
                        // (2) ��� �ִ� ������Ʈ(����)�� Tray�� �ű�
                        lemonModel.transform.SetParent(lemonTrayPoint.transform);
                        lemonModel.transform.localPosition = Vector3.zero;
                        lemonModel.transform.localRotation = Quaternion.identity;

                        isLemonOnTray = true;
                        AddReward(+1);
                    }
                }
                else
                {
                    AddReward(-0.05f);
                    return;
                }
            }
        }
        else if (collision.transform.CompareTag("Wall"))
        {
            // (1) -1 ����
            // (2) ���Ǽҵ� ����

            AddReward(-1);
            EndEpisode();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Point Ʈ���� ����");
        if (other.transform.CompareTag("Point"))
        {
            if (isHold && isLemonOnTray)
            {
                AddReward(+2);
                EndEpisode();
            }
        }
    }
}
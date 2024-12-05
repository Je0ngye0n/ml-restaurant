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

public class StaffAgent_V1 : Agent
{
    public float moveSpeed = 5f;
    public float turnSpeed = 180f;

    public GameObject fishObj;
    public GameObject lemonObj;
    public GameObject tray;

    public Transform staffHoldPoint;
    public Transform fishTrayPoint;
    public Transform lemonTrayPoint;

    private Rigidbody staffRigidbody;

    private bool isHold; // true���, agent�� ������Ʈ�� ��� ����
    
    // agent�� lemon�� ���� �� fish�� ���� �� �ֵ���,
    // fish�� ���� ���¸� ��� ����
    private bool isPickedFish;

    private Transform originalFishPos;
    private Transform originalLemonPos;
    private Transform originalTrayPos;

    public override void Initialize()
    {
        staffRigidbody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        // #1 ����� ��ġ �ʱ�ȭ
        // #2 ���� ��ġ �ʱ�ȭ
        // #3 Ʈ���� ��ġ �ʱ�ȭ
        // #4 agent ��ġ �ʱ�ȭ -> velocity, angularVelocity = 0
    }

    // �����ؾ��ϴ� �� (�Ѱ� ��� �ϴ� ����)(���� ���� ���)
    public override void CollectObservations(VectorSensor sensor)
    {
       // #1 isHold ��
       // #2 isPickedFish ��
       // #3 agent�� �ٶ󺸴� ����
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

        // �� Step���� ���� �г�Ƽ(-0.001) �ο�
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
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Table"))
        {
            // #1 ���̺��� item ���� ��������

            // #2-1 ���̺��� item ������ fish�� ���
            // 1. ���� �ƹ� �͵� ��� ���� �ʴٸ�
            // 1) ��� �ִ� ���·� ��ȯ
            // 2) fish�� player �Ӹ� ���� �ű�
            // ���� �ڵ� : targetObject.transform.SetParent(parentObject.transform);


            // #2-2 ���̺��� item ������ lemon�� ���

            // #2-3 ���̺��� item ������ tray�� ���
        }
        else if (collision.transform.CompareTag("Wall"))
        {
            //
        }
        else if (collision.transform.CompareTag("Point"))
        {

        }

    }
}

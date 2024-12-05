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

    private bool isHold; // true라면, agent가 오브젝트를 들고 있음
    
    // agent가 lemon을 집기 전 fish를 집을 수 있도록,
    // fish가 집힌 상태를 담는 변수
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
        // #1 물고기 위치 초기화
        // #2 레몬 위치 초기화
        // #3 트레이 위치 초기화
        // #4 agent 위치 초기화 -> velocity, angularVelocity = 0
    }

    // 관찰해야하는 값 (넘겨 줘야 하는 정보)(관찰 정보 기록)
    public override void CollectObservations(VectorSensor sensor)
    {
       // #1 isHold 값
       // #2 isPickedFish 값
       // #3 agent가 바라보는 정보
    }

    // 정책에 의해서 정해진 행동 지침 (받아야 하는 정보)
    // 정책 결정
    public override void OnActionReceived(ActionBuffers actions)
    {
        //forwardAmount & turnAmount 정의
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

        // forwardAmount & turnAmount 적용
        staffRigidbody.MovePosition(transform.position + transform.forward * forwardAmount * moveSpeed * Time.fixedDeltaTime);
        transform.Rotate(transform.up * turnAmount * turnSpeed * Time.fixedDeltaTime);

        // 매 Step마다 작은 패널티(-0.001) 부여
        if (MaxStep > 0)
        {
            AddReward(-0.001f);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        int forwardAction = 0;
        int turnAction = 0;

        // 위쪽 방향키: forwardAction=1, 왼쪽 방향키: turnAction=1, 오른쪽 방향키: turnAction=2 

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
            // #1 테이블의 item 정보 가져오기

            // #2-1 테이블의 item 정보가 fish일 경우
            // 1. 만약 아무 것도 들고 있지 않다면
            // 1) 들고 있는 상태로 전환
            // 2) fish를 player 머리 위로 옮김
            // 예시 코드 : targetObject.transform.SetParent(parentObject.transform);


            // #2-2 테이블의 item 정보가 lemon일 경우

            // #2-3 테이블의 item 정보가 tray일 경우
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

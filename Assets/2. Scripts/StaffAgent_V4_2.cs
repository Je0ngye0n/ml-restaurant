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

public class StaffAgent_V4_2 : Agent
{
    public float moveSpeed = 5f;
    public float turnSpeed = 180f;

    private Rigidbody staffRigidbody;

    public Transform foodArea;

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

    private bool isHold; // true라면, agent가 오브젝트를 들고 있음
    
    // fish가 tray에 담겼는지 안 담겼는지 판단
    private bool isFishOnTray;

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
        // #1 물고기 위치 초기화
        // #2 레몬 위치 초기화
        // #3 트레이 위치 초기화
        ResetItem();

        // #4 agent 위치 초기화 -> velocity, angularVelocity = 0
        ResetStaff();

        // #5 isHold 변수 false로 초기화
        // #6 isFishOnTray 변수 false로 초기화
        isHold = false;
        isFishOnTray = false;
    }

    // 관찰해야하는 값 (넘겨 줘야 하는 정보)(관찰 정보 기록)
    public override void CollectObservations(VectorSensor sensor)
    {
        // #1 isHold 값
        // #2 isFishOnTray 값
        // #3 isHold와 isFishOnTray값에 따른 agent와 테이블 사이의 거리
        // #4 agent가 바라보는 정보 ?

        sensor.AddObservation(transform.forward);

        sensor.AddObservation(isHold);

        if (!isHold)
        {
            Vector3 dirFoodArea = (foodArea.localPosition - transform.localPosition).normalized;
            sensor.AddObservation(dirFoodArea.x);
            sensor.AddObservation(dirFoodArea.z);
        }
        else
        {
            Vector3 dirTrayTable = (trayModel.transform.localPosition - transform.localPosition).normalized;
            sensor.AddObservation(dirTrayTable.x);
            sensor.AddObservation(dirTrayTable.z);
        }
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

        // 매 Step마다 작은 패널티 부여
        if (MaxStep > 0)
        {
            AddReward(-1f / MaxStep); // MaxStep = 5000
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

    private void ResetItem()
    {
        // #1 물고기 위치 초기화
        fishModel.transform.SetParent(itemItitPos.transform);
        fishModel.transform.SetLocalPositionAndRotation(originalFishPos, originalFishRot);

        // #2 레몬 위치 초기화
        lemonModel.transform.SetParent(itemItitPos.transform);
        lemonModel.transform.SetLocalPositionAndRotation(originalLemonPos, originalLemonRot);

        // #3 트레이 위치 초기화
        trayModel.transform.SetParent(itemItitPos.transform);
        trayModel.transform.SetLocalPositionAndRotation(originalTrayPos, originalTrayRot);
    }

    private void ResetStaff()
    {
        // #4 agent 위치 초기화 -> velocity, angularVelocity = 0
        Rigidbody rigidbody = staffRigidbody.GetComponent<Rigidbody>();
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;

        // position, rot도 초기화!
        transform.localPosition = originStaffPos;
        transform.localRotation = Quaternion.identity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // #1 테이블의 tag 정보 가져오기

        // #2-1 테이블의 tag 정보가 fish일 경우
        if (collision.transform.CompareTag("FishTable"))
        {
            // 1. 만약 아무 것도 들고 있지 않다면 (isHold = false)
                // (isFishOnTray false라면)
                    // 1) 들고 있는 상태로 전환 : isHold = true
                    // 2) fish를 player 머리 위로 옮김
                    // 예시 코드 : targetObject.transform.SetParent(parentObject.transform);
                    // 3) +1 보상
                // (isFishOnTray true라면 그대로 리턴)
            // 2. 만약 무언 갈 들고 있다면 그대로 리턴(콜리젼 넘김)

            if (isHold)
            {
                return;
            }

            if (isFishOnTray)
            {
                return;
            }
            else
            {
                isHold = true;

                // 2) fish를 player 머리 위로 옮김
                fishModel.transform.SetParent(staffItemHolder.transform);
                fishModel.transform.localPosition = Vector3.zero;
                fishModel.transform.localRotation = Quaternion.identity;

                AddReward(+1);
            }
        }
        // #2-2 테이블의 tag 정보가 lemon일 경우
        else if (collision.transform.CompareTag("LemonTable")) 
        {
                // 1. 만약 아무 것도 들고 있지 않다면 (isHold = false)
                    // 1) isFishOnTray true인지 판단
                        // 1-1) True라면
                            // (1) 들고 있는 상태로 전환 isHold = true
                            // (2) lemon을 player 머리 위로 옮김
                            // (3) +1 보상
                        // 1-2) False라면 그대로 리턴
                // 2. 만약 무언갈 들고 있다면 그대로 리턴
            
            if (isHold)
            {
                return;
            }
            if (!isFishOnTray)
            {
                return;
            }
            else
            {
                isHold = true;

                // (2) lemon을 player 머리 위로 옮김
                lemonModel.transform.SetParent(staffItemHolder.transform);
                lemonModel.transform.localPosition = Vector3.zero;
                lemonModel.transform.localRotation = Quaternion.identity;

                AddReward(+1);
            }
        }
        // #2-3 테이블의 tag 정보가 tray일 경우
        else if (collision.transform.CompareTag("TrayTable")) 
        {
                // 1. 만약 아무 것도 들고 있지 않다면 (isHold = false)
                    // 1) 그대로 리턴
                // 2. 만약 무언갈 들고 있다면 (isHold = true)
                    // 1) isFishOnTray true인지 판단
                        // 1-1) False라면
                            // (1) 들고 있지 않은 상태로 전환 : isHold = false
                            // (2) 들고 있는 오브젝트(물고기)를 Tray로 옮김
                            // (3) isPickedFish를 true로 변경
                            // (4) +1 보상
                        // 1-2) True라면
                            // (1) 들고 있지 않은 상태로 전환 : isHold = false
                            // (2) 들고 있는 오브젝트(레몬)을 Tray로 옮김
                            // (3) +1 보상
                            // (4) 에피소드 종료

            if (!isHold)
            {
                return;
            }
            else
            {
                isHold = false;
                if (!isFishOnTray)
                {
                    // (2) 들고 있는 오브젝트(물고기)를 Tray로 옮김
                    fishModel.transform.SetParent(fishTrayPoint.transform);
                    fishModel.transform.localPosition = Vector3.zero;
                    fishModel.transform.localRotation = Quaternion.identity;

                    isFishOnTray = true;
                    AddReward(+1);
                }
                else
                {
                    // (2) 들고 있는 오브젝트(레몬)을 Tray로 옮김
                    lemonModel.transform.SetParent(lemonTrayPoint.transform);
                    lemonModel.transform.localPosition = Vector3.zero;
                    lemonModel.transform.localRotation = Quaternion.identity;

                    AddReward(+1);
                    EndEpisode();
                }
            }
        }
        else if (collision.transform.CompareTag("Wall"))
        {
            // (1) -1 보상
            // (2) 에피소드 종료

            AddReward(-1);
            EndEpisode();
        }
    }
}

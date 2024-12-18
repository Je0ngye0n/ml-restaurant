using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GTable : MonoBehaviour
{
    // #1 게스트 정보
    public bool isGuest;
    public GameObject guestModel;

    public GTable (bool isGuest, GameObject guestModel)
    {
        this.isGuest = isGuest;
        this.guestModel = guestModel;
    }

    // #3 게스트 스폰 위치에 스폰 함수
    public void SpawnGuest()
    {
        if (isGuest)
        {
            guestModel.SetActive(true);
        }
    }

    public void ResetGuestModel()
    {
        guestModel.SetActive(false);
    }

    public Transform GetGuestTransform()
    {
        return guestModel.transform;
    }
}

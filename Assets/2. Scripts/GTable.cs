using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GTable : MonoBehaviour
{
    // #1 �Խ�Ʈ ����
    public bool isGuest;
    public GameObject guestModel;

    public GTable (bool isGuest, GameObject guestModel)
    {
        this.isGuest = isGuest;
        this.guestModel = guestModel;
    }

    // #3 �Խ�Ʈ ���� ��ġ�� ���� �Լ�
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

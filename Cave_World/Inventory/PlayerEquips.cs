using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquips : MonoBehaviour
{
    public EquipSlot[] equipSlot;

    private void Awake()
    {
        for (int i = 3; i < equipSlot.Length + 3; i++)
        {
            equipSlot[i - 3].receiveItemType = i;           
        }
    }

    // ���⿡ equipSlot[].equipItemData�ȿ� �ִ� Items�� �ɷ�ġ�� ���� Player���� �־��ִ� �޼��尡 �ʿ�.
}

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

    // 여기에 equipSlot[].equipItemData안에 있는 Items의 능력치를 전부 Player한테 넣어주는 메서드가 필요.
}

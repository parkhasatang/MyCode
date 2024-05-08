using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SlotData
{
    public bool isEmpty;
    public bool isChoose;
    /*public GameObject slotObj; // 이미지 변경을 위한*/
    public Item item;
    public int stack;
}

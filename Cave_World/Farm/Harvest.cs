using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harvest : MonoBehaviour
{
    private Field field;

    private void Awake()
    {
        field = GetComponentInParent<Field>();
    }

    // �� �޼���� �ִϸ��̼ǿ� ����.
    public void GrowFinish()
    {
        field.isGrowFinish = true;
    }
}

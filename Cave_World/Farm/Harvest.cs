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

    // 이 메서드는 애니메이션에 연결.
    public void GrowFinish()
    {
        field.isGrowFinish = true;
    }
}

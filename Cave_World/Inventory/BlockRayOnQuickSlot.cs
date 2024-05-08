using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRayOnQuickSlot : MonoBehaviour
{
    public CanvasGroup quickSlot;
    private void OnEnable()
    {
        quickSlot.blocksRaycasts = true;
    }

    private void OnDisable()
    {
        quickSlot.blocksRaycasts = false;
    }
}

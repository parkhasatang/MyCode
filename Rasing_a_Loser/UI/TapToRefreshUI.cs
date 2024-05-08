using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TapToRefreshUI : MonoBehaviour, IPointerDownHandler
{
    public RectTransform content;

    public void OnPointerDown(PointerEventData eventData)
    {
        EventChatManager.Instance.NextDialoguePiece();
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        Debug.Log("гоюл");
    }
}

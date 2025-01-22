using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStack : MonoBehaviour
{
    private void OnEnable()
    {
        UIManager.Instance.uiEscapeStack.PushView(this);
    }
}
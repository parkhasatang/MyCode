using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingUIView : UIView
{
    [Header("Animation Settings")]
    public float moveDuration = 0.2f;

    public TMP_Text uIDText;

    protected override void Awake()
    {
        base.Awake();
        uIDText.text = $"UID : {Authenticate.Instance.UID}";

        if (backButtons != null)
        {
            foreach (var button in backButtons)
            {
                button.onClick.AddListener(() => GameManager.Instance.Player.PluckController.SetPointerInputEnabled(true));
            }
        }
    }

    public void SetWhiteImagePosition(Toggle toggle, RectTransform whiteImage, Vector2 onPosition, Vector2 offPosition, bool animate)
    {
        Vector2 targetPosition = toggle.isOn ? onPosition : offPosition;

        if (animate)
        {
            whiteImage.DOAnchorPos(targetPosition, moveDuration);
        }
        else
        {
            whiteImage.anchoredPosition = targetPosition;
        }
    }
}

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewWigNoticeUIController : UIController
{
    public TMP_Text wigNameTxtField;
    public Image wigImage;

    private void OnEnable()
    {
        SoundManager.Instance.PlaySoundEffect("가발 해금", 0.5f);
        SoundManager.Instance.PlaySoundEffect("불꽃놀이", 0.5f, 1f, true);
    }

    public void SetupNewWigNoticeUI(WigSO wigSO)
    {
        if (wigSO == null)
        {
            Debug.LogError("wigSO가 null입니다.");
            return;
        }

        // ���� �̸��� �̹��� ����
        wigNameTxtField.text = wigSO.wigName;
        wigImage.sprite = GameDataManager.Instance.GetWigSpriteByName(wigSO.uniqueID);
    }

    private void OnDisable()
    {
        SoundManager.Instance.StopSoundEffect("불꽃놀이");
    }
}
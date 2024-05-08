using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestBoard : MonoBehaviour
{
    public TMP_Text questShortCutTxt;
    public Image questShortCutImg;
    public Button clearBtn;
    public Button cancelBtn;
    public CanvasGroup questShortCutAlpha;

    internal QuestInfo questInfo;

    private void Awake()
    {
        QuestManager.instance.OnCheckQuestRequest += CheckRequst;
    }

    public void OnEnable()
    {
        transform.SetAsLastSibling();
    }

    public void ShortCutFadeOut()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(questShortCutAlpha.DOFade(0f, 1f).OnComplete(DeactiveThis));

        // 시퀀스 실행
        sequence.Play();
    }

    public void ShortCutFadeIn()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(questShortCutAlpha.DOFade(1f, 1f));

        sequence.Play();
    }

    public void ShowQuestOnShortCut()
    {
        if (questInfo != null)
        {
            if (questInfo.isMainQuest)
            {
                cancelBtn.gameObject.SetActive(false);
            }
            else cancelBtn.gameObject.SetActive(true);
            CheckRequst();
            ShortCutFadeIn();
        }
        else return;
    }

    public void CheckRequst()
    {
        if (questInfo != null)
        {
            questShortCutTxt.text = $"{UIManager.Instance.playerInventoryData.ReturnStackInInventory(questInfo.requestItem.ItemCode)}/{questInfo.requestCount}";
            questShortCutImg.sprite = ItemManager.instance.GetSpriteByItemCode(questInfo.requestItem.ItemCode);

            if (UIManager.Instance.playerInventoryData.CheckStackAmount(questInfo.requestItem.ItemCode, questInfo.requestCount))
            {
                // 버튼 이미지 바꾸고 눌리게 해주기.
                clearBtn.interactable = true;

                ColorBlock colors = clearBtn.colors;
                colors.colorMultiplier = 2f;
                clearBtn.colors = colors;
            }
            else
            {
                clearBtn.interactable = false;

                ColorBlock colors = clearBtn.colors;
                colors.colorMultiplier = 1f;
                clearBtn.colors = colors;
            }
        }
        else
        {
            questShortCutTxt.text = $"0/0";
            questShortCutImg.sprite = null;

            clearBtn.interactable = false;

            ColorBlock colors = clearBtn.colors;
            colors.colorMultiplier = 1f;
            clearBtn.colors = colors;

            Debug.Log("QuestBoard에 저장된 퀘스트 정보가 없습니다.");
        }
    }

    // ClearBtn에 연결.
    public void GetReward()
    {
        UIManager.Instance.playerInventoryData.RemoveItemFromInventory(questInfo.requestItem.ItemCode, questInfo.requestCount);
        UIManager.Instance.playerInventoryData.GiveItemToEmptyInv(questInfo.rewardItem, questInfo.rewardCount);
        questInfo = null;

        if (QuestManager.instance.isMainQuestDoing)
        {
            QuestManager.instance.isMainQuestDoing = false;
        }

        QuestManager.instance.CheckAllQuestRequest();
        questInfo = new QuestInfo();
        DeactiveThis();
    }

    // CancelBtn에 연결.
    public void CancelQuest()
    {
        questInfo = null;
        CheckRequst();
        questInfo = new QuestInfo();
        DeactiveThis();
    }

    private void DeactiveThis()
    {
        gameObject.SetActive(false);
    }
}

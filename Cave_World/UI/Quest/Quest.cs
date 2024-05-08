using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Quest : MonoBehaviour
{
    public event Action QuestStateChanged;

    public QuestInfo questInfo;
    private MainQuest mainQuest;

    public GameObject ErrorMsgBtn;
    public GameObject rewardBoard;
    public Image requsetImg;
    public Image rewardImg;
    public TMP_Text requestCountTxt;
    public TMP_Text rewardCountTxt;
    public TMP_Text description;
    public TMP_Text questTile;

    public Button mainQuestAcceptBtn;
    public Button acceptBtn;
    public Button resetBtn;
    public CanvasGroup canvasGroup;
    private Sequence sequence;

    private ItemManager itemManager;

    private void Awake()
    {
        mainQuest = GetComponent<MainQuest>();
    }

    private void Start()
    {
        itemManager = ItemManager.instance;
    }

    // 랜덤 퀘스트 주기.
    public void CreateRandomQuest()
    {
        questInfo = new QuestInfo();

        questInfo.requestItem = itemManager.CreateRandomItemByType(2);
        questInfo.requestCount = UnityEngine.Random.Range(1, 21);
        questInfo.rewardItem = itemManager.CreateRandomItemByType(8);
        questInfo.rewardCount = UnityEngine.Random.Range(1, 21);
        questInfo.isMainQuest = false;
    }

    // 퀘스트 필요 아이템과 보상의 이미지 결정.
    public void SetQuestImgAndTxt()
    {
        requsetImg.sprite = itemManager.GetSpriteByItemCode(questInfo.requestItem.ItemCode);
        requestCountTxt.text = $"{questInfo.requestCount}";
        rewardImg.sprite = itemManager.GetSpriteByItemCode(questInfo.rewardItem.ItemCode);
        rewardCountTxt.text = $"{questInfo.rewardCount}";
    }

    private void FadeOut()
    {
        sequence = DOTween.Sequence();

        sequence.Append(canvasGroup.DOFade(0f, 1f)).OnComplete(FadeIn);
        
        // 시퀀스 실행
        sequence.Play();
    }

    private void FadeIn()
    {
        sequence = DOTween.Sequence();

        sequence.Append(canvasGroup.DOFade(1f, 1f));

        sequence.Play();
    }

    public void ResetQuest()
    {
        CreateRandomQuest();
        Invoke("SetQuestImgAndTxt", 1f);

        FadeOut();
    }

    public void PressAcceptBtn()
    {
        SetQuestInfo();

        QuestManager.instance.SetQuestOnQuestBoard();

        ResetQuest();
    }

    public void PressResetBtn()
    {
        ResetQuest();
    }

    // 퀘스트 정보를 퀘스트 매니저에 보내기.
    private void SetQuestInfo()
    {
        QuestManager.instance.questInfo = questInfo;
    }

    public void SetMainQuest(int index)
    {
        questInfo = new QuestInfo();

        questInfo = mainQuest.mainQuests[index];
        description.text = questInfo.questDescription;

        QuestManager.instance.mainQuestProgressIndex++;
    }

    public void PressMainQuestBtn()
    {
        if (!QuestManager.instance.isMainQuestDoing)
        {
            if (QuestManager.instance.mainQuestProgressIndex >= mainQuest.mainQuests.Count)
            {
                SetQuestInfo();

                QuestManager.instance.SetQuestOnQuestBoard();

                ResetQuest();

                Invoke("FinishMainQuest", 1f);
            }
            else
            {
                QuestManager.instance.isMainQuestDoing = true;

                SetQuestInfo();

                QuestManager.instance.SetQuestOnQuestBoard();

                SetNextMainQuest();
            }
        }
        else
        {
            if (!ErrorMsgBtn.activeSelf)
            {
                ErrorMsgBtn.SetActive(true);
            }
        }
    }

    public void SetNextMainQuest()
    {
        SetMainQuest(QuestManager.instance.mainQuestProgressIndex);

        Invoke("SetQuestImgAndTxt", 1f);

        FadeOut();
    }

    private void FinishMainQuest()
    {
        mainQuestAcceptBtn.gameObject.SetActive(false);
        acceptBtn.gameObject.SetActive(true);
        resetBtn.gameObject.SetActive(true);
        rewardBoard.SetActive(true);
        description.text = "던전에서 아래 재료를 좀 가져다 주지 않겠나?\r\n보상으로 음식을 주겠네.";
        questTile.text = "랜덤 퀘스트";
    }
}

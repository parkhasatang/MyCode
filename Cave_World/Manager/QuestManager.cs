using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class QuestInfo
{
    public Item requestItem;
    public int requestCount;

    public Item rewardItem;
    public int rewardCount;

    public string questDescription;

    public bool isMainQuest;
}

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;
    internal QuestShortCut questShortCut;
    internal int questSlotCount;
    internal QuestInfo questInfo;

    internal bool isMainQuestDoing;
    internal bool isLastMainQuest;
    internal int mainQuestProgressIndex;

    public event Action OnQuestAccepted;
    public event Action OnCheckQuestRequest;

    private void Awake()
    {
        instance = this;

        questShortCut = FindObjectOfType<QuestShortCut>();

        if (questShortCut != null)
        {
            questSlotCount = questShortCut.questBoards.Length;
        }
        else
        {
            Debug.LogError("����Ʈ ������ ��ã�ҽ��ϴ�.");
        }

        for (int i = 0; i < questSlotCount; i++)
        {
            questShortCut.questBoards[i].questInfo = new QuestInfo();
        }
    }

    public void SetQuestOnQuestBoard()
    {
        for (int i = 0; i < questSlotCount; i++)
        {
            if (questShortCut.questBoards[i].questInfo.requestItem == null)
            {
                if (questInfo != null)
                {
                    questShortCut.questBoards[i].questInfo = new QuestInfo();
                    questShortCut.questBoards[i].questInfo = questInfo;
                    questInfo = null;

                    questShortCut.questBoards[i].ShowQuestOnShortCut();

                    questShortCut.questBoards[i].transform.gameObject.SetActive(true);
                    break;
                }
                else
                {
                    Debug.LogError("����Ʈ ������ ����Ʈ â���� �ȳѾ�ɴϴ�.");
                }
            }
        }
    }

    public void CheckAllQuestRequest()
    {
        OnCheckQuestRequest?.Invoke();
    }
}

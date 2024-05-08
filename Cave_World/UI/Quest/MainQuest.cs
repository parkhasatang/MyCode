using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestData
{
    public int requestItemCode;
    public int requestCount;
    public int rewardItemCode;
    public int rewardCount;
    public string questDescription;
}

[System.Serializable]
public class MainQuestData
{
    public List<QuestData> mainQuestdata;
}

public class MainQuest : MonoBehaviour
{
    private ItemManager itemManager;

    public List<QuestInfo> mainQuests = new List<QuestInfo>();

    private void Start()
    {
        itemManager = ItemManager.instance;
        LoadMainQuestData();
    }

    private void LoadMainQuestData()
    {
        // Json ���� ���
        string jsonPath = "MainQuestData";

        // Resources ���� ���� Json ���� �ε�
        TextAsset jsonFile = Resources.Load<TextAsset>(jsonPath);

        if (jsonFile != null)
        {
            // Json ������ ������ ���ڿ��� ��ȯ
            string json = jsonFile.text;

            // Json �����͸� QuestInfo ��ü�� ����Ʈ�� ������ȭ
            MainQuestData questData = JsonConvert.DeserializeObject<MainQuestData>(json);

            // ���� ����Ʈ ����Ʈ �ʱ�ȭ
            mainQuests = new List<QuestInfo>();

            // Json �����͸� ����Ʈ�� �߰�
            foreach (var questDataItem in questData.mainQuestdata)
            {
                QuestInfo newQuest = new QuestInfo();

                // ��û ������ �ڵ带 ������ ��ü�� ��ȯ�Ͽ� �Ҵ�
                newQuest.requestItem = itemManager.SetItemData(questDataItem.requestItemCode);
                newQuest.requestCount = questDataItem.requestCount;

                // ���� ������ �ڵ带 ������ ��ü�� ��ȯ�Ͽ� �Ҵ�
                newQuest.rewardItem = itemManager.SetItemData(questDataItem.rewardItemCode);
                newQuest.rewardCount = questDataItem.rewardCount;

                // ����Ʈ ���� �Ҵ�
                newQuest.questDescription = questDataItem.questDescription;

                // ������ ����Ʈ�� ���� ����Ʈ���� ǥ��
                newQuest.isMainQuest = true;

                // ����Ʈ�� �߰�
                mainQuests.Add(newQuest);
            }

            // ���������� �����͸� �ҷ������Ƿ� ��� ����
            Debug.Log("MainQuestData.json ������ ���������� �ҷ��Խ��ϴ�.");
        }
        else
        {
            Debug.LogError("MainQuestData.json ������ �ҷ����� �� �����߽��ϴ�.");
        }
    }
}
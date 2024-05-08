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
        // Json 파일 경로
        string jsonPath = "MainQuestData";

        // Resources 폴더 내의 Json 파일 로드
        TextAsset jsonFile = Resources.Load<TextAsset>(jsonPath);

        if (jsonFile != null)
        {
            // Json 파일의 내용을 문자열로 변환
            string json = jsonFile.text;

            // Json 데이터를 QuestInfo 객체의 리스트로 역직렬화
            MainQuestData questData = JsonConvert.DeserializeObject<MainQuestData>(json);

            // 메인 퀘스트 리스트 초기화
            mainQuests = new List<QuestInfo>();

            // Json 데이터를 리스트에 추가
            foreach (var questDataItem in questData.mainQuestdata)
            {
                QuestInfo newQuest = new QuestInfo();

                // 요청 아이템 코드를 아이템 객체로 변환하여 할당
                newQuest.requestItem = itemManager.SetItemData(questDataItem.requestItemCode);
                newQuest.requestCount = questDataItem.requestCount;

                // 보상 아이템 코드를 아이템 객체로 변환하여 할당
                newQuest.rewardItem = itemManager.SetItemData(questDataItem.rewardItemCode);
                newQuest.rewardCount = questDataItem.rewardCount;

                // 퀘스트 설명 할당
                newQuest.questDescription = questDataItem.questDescription;

                // 생성된 퀘스트가 메인 퀘스트임을 표시
                newQuest.isMainQuest = true;

                // 리스트에 추가
                mainQuests.Add(newQuest);
            }

            // 성공적으로 데이터를 불러왔으므로 사용 가능
            Debug.Log("MainQuestData.json 파일을 성공적으로 불러왔습니다.");
        }
        else
        {
            Debug.LogError("MainQuestData.json 파일을 불러오는 데 실패했습니다.");
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;
using DialogueSystem;
using System;
using UnityEngine.UI;

public class EventChatManager : MonoBehaviour
{
    public static EventChatManager Instance;

    private Queue<GameObject> chatQueue = new Queue<GameObject>();
    private List<GameObject> messageList = new List<GameObject>();

    public RectTransform eventChatContent;
    public GameObject npcChatPrefab;
    public GameObject playerChatPrefab;
    public GameObject chatMessageBox;
    public GameObject chatMessage;

    public DialogueData dialogueData; // 건드리면 안되는 데이터

    // 초기화 할 수 있는 데이터들
    private DialogueEntry currentDialogueEntry;
    private Queue<ConversationPiece> currentConversation = new Queue<ConversationPiece>();
    private bool isBranchDialogueActive = false;

    private GameObject lastSpeakerChatBox = null;  // 마지막으로 생성된 채팅 박스를 추적
    private RectTransform lastChatBoxMessagePosition = null; // 마지막으로 메세지 생성될 챗 박스
    private string lastSpeaker = null;  // 마지막으로 대화를 한 스피커 추적

    private void Awake()
    {
        if (Instance != null) Destroy(this.gameObject);
        Instance = this;

        LoadDialogueData(); // Npc스크립트 로드
    }

    private void LoadDialogueData()
    {
        TextAsset file = Resources.Load<TextAsset>("NPCDialogues");
        dialogueData = JsonUtility.FromJson<DialogueData>(file.text);
    }

    public void StartDialogue(string id)
    {
        foreach (var dialogue in dialogueData.dialogues)
        {
            if (dialogue.id == id)
            {
                currentDialogueEntry = dialogue; // 현재 대화 설정
                LoadConversation(dialogue.conversations); // 대화 로드
                Debug.Log("대화시작");
                break;
            }
        }
    }

    private void LoadConversation(List<ConversationPiece> conversationPieces)
    {
        currentConversation.Clear(); // 현재 대화 큐 초기화
        foreach (var piece in conversationPieces)
        {
            currentConversation.Enqueue(piece); // 새로운 대화 조각 추가
        }
        NextDialoguePiece(); // 첫 대화 조각 표시
    }

    public void NextDialoguePiece()
    {
        if (currentConversation.Count > 0)
        {
            var piece = currentConversation.Dequeue();
            DisplayDialoguePiece(piece); // 대화 조각 표시
        }
        else
        {
            if (currentDialogueEntry != null && !isBranchDialogueActive)
            {
                // 모든 대화가 끝났으면 성공 또는 실패 분기 처리
                if (currentDialogueEntry.requirementValue <= 7 /*여기에 친구requirementStat에 맞게 합치시*/)
                {
                    LoadConversation(currentDialogueEntry.success);
                }
                else
                {
                    LoadConversation(currentDialogueEntry.failure);
                }
                isBranchDialogueActive = true;
            }
            else
            {
                Debug.Log("대화 종료");
                EndDialogue(); // 대화 종료 처리
            }
        }
    }

    private void DisplayDialoguePiece(ConversationPiece piece)
    {
        // 스피커가 변경되었거나 처음 대화를 시작하는 경우 새 채팅 박스 생성
        if (lastSpeaker == null || lastSpeaker != piece.speaker)
        {
            if (piece.speaker == "NPC")
            {
                lastSpeakerChatBox = GenerateChat(npcChatPrefab);
            }
            else if (piece.speaker == "Player")
            {
                lastSpeakerChatBox = GenerateChat(playerChatPrefab);
            }
            lastSpeaker = piece.speaker;
        }
        if (lastSpeakerChatBox != null)
        {
            lastChatBoxMessagePosition = lastSpeakerChatBox.GetComponentInChildren<Chat>().chat;
        }

        // 대화 메시지 생성 및 추가
        GenerateMessage(chatMessage, lastChatBoxMessagePosition, piece.text);
        FitUI(lastChatBoxMessagePosition);
        FitUI((RectTransform)lastSpeakerChatBox.transform);
    }

    private void EndDialogue()
    {
        // 대화가 완전히 종료됐을 때 필요한 처리
        currentDialogueEntry = null; // 현재 대화 엔트리 초기화
        isBranchDialogueActive = false; // 분기 대화 비활성화
        TestUIManager.Instance.friendEventUI.SetActive(false);
        Debug.Log("대화가 완전히 종료되었습니다.");
        // 필요한 경우 외부 이벤트 트리거
    }

    public GameObject GenerateChat(GameObject chatPrefab)
    {
        GameObject newChat = null;

        // 비활성화된 채팅 오브젝트를 큐에서 찾아 재사용
        foreach (GameObject chat in chatQueue)
        {
            if (!chat.activeInHierarchy)
            {
                newChat = chat;
                break;
            }
        }

        // 비활성화된 채팅 오브젝트가 없다면 새로 생성
        if (newChat == null)
        {
            newChat = Instantiate(chatPrefab);
            chatQueue.Enqueue(newChat); // 새로 생성된 오브젝트를 큐에 추가
        }
        else
        {
            chatQueue.Enqueue(chatQueue.Dequeue()); // 재사용 오브젝트를 큐의 끝으로 이동
        }

        // 채팅 메시지 설정 및 활성화
        newChat.transform.SetParent(eventChatContent, false);
        newChat.transform.SetAsLastSibling();
        newChat.SetActive(true);

        Debug.Log("생성");
        return newChat;
    }

    public void GenerateMessage(GameObject chatPrefab, Transform messagePosition, string text)
    {
        GameObject newMessage = null;

        // 비활성화된 메세지 오브젝트를 리스트에서 찾아 재사용
        foreach (GameObject message in messageList)
        {
            if (!message.activeInHierarchy)
            {
                newMessage = message;
                newMessage.SetActive(true);
                break;
            }
        }

        // 비활성화된 메세지 오브젝트가 없다면 새로 생성
        if (newMessage == null)
        {
            newMessage = Instantiate(chatPrefab);
            messageList.Add(newMessage);
        }

        // 채팅 메시지 설정 및 활성화
        newMessage.GetComponentInChildren<TMP_Text>().text = text;
        newMessage.transform.SetParent(messagePosition, false);
        newMessage.transform.SetAsLastSibling();
    }

    public void FitUI(RectTransform rect)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
    }
}

        /*GameObject newChat = null;
        if (piece.speaker == "NPC")
        {
            newChat = GenerateChat(npcChatPrefab);
            
        }
        else if (piece.speaker == "Player")
        {
            newChat = GenerateChat(playerChatPrefab);
        }

        if (newChat == null) Debug.LogError("NPCDialogues에 해당하는 Speaker가 없습니다.");

        Transform position = newChat.GetComponent<Chat>().chat;
        GenerateMessage(chatMessage, position, piece.text);*/
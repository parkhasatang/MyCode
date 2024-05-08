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

    public DialogueData dialogueData; // �ǵ帮�� �ȵǴ� ������

    // �ʱ�ȭ �� �� �ִ� �����͵�
    private DialogueEntry currentDialogueEntry;
    private Queue<ConversationPiece> currentConversation = new Queue<ConversationPiece>();
    private bool isBranchDialogueActive = false;

    private GameObject lastSpeakerChatBox = null;  // ���������� ������ ä�� �ڽ��� ����
    private RectTransform lastChatBoxMessagePosition = null; // ���������� �޼��� ������ ê �ڽ�
    private string lastSpeaker = null;  // ���������� ��ȭ�� �� ����Ŀ ����

    private void Awake()
    {
        if (Instance != null) Destroy(this.gameObject);
        Instance = this;

        LoadDialogueData(); // Npc��ũ��Ʈ �ε�
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
                currentDialogueEntry = dialogue; // ���� ��ȭ ����
                LoadConversation(dialogue.conversations); // ��ȭ �ε�
                Debug.Log("��ȭ����");
                break;
            }
        }
    }

    private void LoadConversation(List<ConversationPiece> conversationPieces)
    {
        currentConversation.Clear(); // ���� ��ȭ ť �ʱ�ȭ
        foreach (var piece in conversationPieces)
        {
            currentConversation.Enqueue(piece); // ���ο� ��ȭ ���� �߰�
        }
        NextDialoguePiece(); // ù ��ȭ ���� ǥ��
    }

    public void NextDialoguePiece()
    {
        if (currentConversation.Count > 0)
        {
            var piece = currentConversation.Dequeue();
            DisplayDialoguePiece(piece); // ��ȭ ���� ǥ��
        }
        else
        {
            if (currentDialogueEntry != null && !isBranchDialogueActive)
            {
                // ��� ��ȭ�� �������� ���� �Ǵ� ���� �б� ó��
                if (currentDialogueEntry.requirementValue <= 7 /*���⿡ ģ��requirementStat�� �°� ��ġ��*/)
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
                Debug.Log("��ȭ ����");
                EndDialogue(); // ��ȭ ���� ó��
            }
        }
    }

    private void DisplayDialoguePiece(ConversationPiece piece)
    {
        // ����Ŀ�� ����Ǿ��ų� ó�� ��ȭ�� �����ϴ� ��� �� ä�� �ڽ� ����
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

        // ��ȭ �޽��� ���� �� �߰�
        GenerateMessage(chatMessage, lastChatBoxMessagePosition, piece.text);
        FitUI(lastChatBoxMessagePosition);
        FitUI((RectTransform)lastSpeakerChatBox.transform);
    }

    private void EndDialogue()
    {
        // ��ȭ�� ������ ������� �� �ʿ��� ó��
        currentDialogueEntry = null; // ���� ��ȭ ��Ʈ�� �ʱ�ȭ
        isBranchDialogueActive = false; // �б� ��ȭ ��Ȱ��ȭ
        TestUIManager.Instance.friendEventUI.SetActive(false);
        Debug.Log("��ȭ�� ������ ����Ǿ����ϴ�.");
        // �ʿ��� ��� �ܺ� �̺�Ʈ Ʈ����
    }

    public GameObject GenerateChat(GameObject chatPrefab)
    {
        GameObject newChat = null;

        // ��Ȱ��ȭ�� ä�� ������Ʈ�� ť���� ã�� ����
        foreach (GameObject chat in chatQueue)
        {
            if (!chat.activeInHierarchy)
            {
                newChat = chat;
                break;
            }
        }

        // ��Ȱ��ȭ�� ä�� ������Ʈ�� ���ٸ� ���� ����
        if (newChat == null)
        {
            newChat = Instantiate(chatPrefab);
            chatQueue.Enqueue(newChat); // ���� ������ ������Ʈ�� ť�� �߰�
        }
        else
        {
            chatQueue.Enqueue(chatQueue.Dequeue()); // ���� ������Ʈ�� ť�� ������ �̵�
        }

        // ä�� �޽��� ���� �� Ȱ��ȭ
        newChat.transform.SetParent(eventChatContent, false);
        newChat.transform.SetAsLastSibling();
        newChat.SetActive(true);

        Debug.Log("����");
        return newChat;
    }

    public void GenerateMessage(GameObject chatPrefab, Transform messagePosition, string text)
    {
        GameObject newMessage = null;

        // ��Ȱ��ȭ�� �޼��� ������Ʈ�� ����Ʈ���� ã�� ����
        foreach (GameObject message in messageList)
        {
            if (!message.activeInHierarchy)
            {
                newMessage = message;
                newMessage.SetActive(true);
                break;
            }
        }

        // ��Ȱ��ȭ�� �޼��� ������Ʈ�� ���ٸ� ���� ����
        if (newMessage == null)
        {
            newMessage = Instantiate(chatPrefab);
            messageList.Add(newMessage);
        }

        // ä�� �޽��� ���� �� Ȱ��ȭ
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

        if (newChat == null) Debug.LogError("NPCDialogues�� �ش��ϴ� Speaker�� �����ϴ�.");

        Transform position = newChat.GetComponent<Chat>().chat;
        GenerateMessage(chatMessage, position, piece.text);*/
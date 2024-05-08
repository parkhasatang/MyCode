using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestUIManager : MonoBehaviour
{
    public static TestUIManager Instance;

    public GameObject[] friendList; // UI���� ģ�� ����Ʈ�� ǥ���ϱ� ���� �迭
    public GameObject friendEventUI;

    private void Awake()
    {
        if (Instance != null) Destroy(this.gameObject);
        Instance = this;
    }

    public void UpdateFriendListUI(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < friendList.Length)
        {
            friendList[slotIndex].SetActive(true);

            // �߰�������, ģ���� �ر� ���³� �ٸ� ������ UI�� �ݿ��ϴ� �ڵ带 ���⿡ �߰��� �� ����
            // ���� ���, �رݵ� ģ���� �̸��� ǥ���ϰų�, Ư�� �������� Ȱ��ȭ.
        }
        else
        {
            Debug.LogError("Invalid slotIndex passed to UpdateFriendListUI.");
        }
    }
}

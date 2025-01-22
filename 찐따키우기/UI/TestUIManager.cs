using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestUIManager : MonoBehaviour
{
    public static TestUIManager Instance;

    public GameObject[] friendList; // UI에서 친구 리스트를 표현하기 위한 배열
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

            // 추가적으로, 친구의 해금 상태나 다른 정보를 UI에 반영하는 코드를 여기에 추가할 수 있음
            // 예를 들어, 해금된 친구의 이름을 표시하거나, 특정 아이콘을 활성화.
        }
        else
        {
            Debug.LogError("Invalid slotIndex passed to UpdateFriendListUI.");
        }
    }
}

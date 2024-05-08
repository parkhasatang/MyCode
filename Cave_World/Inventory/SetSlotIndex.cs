using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSlotIndex : MonoBehaviour
{
    [SerializeField] private DraggableUI draggableUI;

    private void Awake()
    {
        ExtractNumberFromPrefabName();
    }

    private void ExtractNumberFromPrefabName()
    {
        // 프리팹 이름 가져오기
        string prefabName = gameObject.name;

        // 괄호 안에 있는 숫자 추출
        string numberString = ExtractNumberInsideParentheses(prefabName);

        if (int.TryParse(numberString, out int number))
        {
            // InventoryIndex 변수에 할당
            draggableUI.inventoryIndex = number;
        }
        else
        {
            Debug.Log("No number found in the prefab name");
        }
    }

    private string ExtractNumberInsideParentheses(string input)
    {
        int startIndex = input.IndexOf('(');
        int endIndex = input.IndexOf(')');

        if (startIndex != -1 && endIndex != -1)
        {
            // 괄호 안에 있는 부분 추출
            return input.Substring(startIndex + 1, endIndex - startIndex - 1);
        }
        else
        {
            return string.Empty;
        }
    }
}

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
        // ������ �̸� ��������
        string prefabName = gameObject.name;

        // ��ȣ �ȿ� �ִ� ���� ����
        string numberString = ExtractNumberInsideParentheses(prefabName);

        if (int.TryParse(numberString, out int number))
        {
            // InventoryIndex ������ �Ҵ�
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
            // ��ȣ �ȿ� �ִ� �κ� ����
            return input.Substring(startIndex + 1, endIndex - startIndex - 1);
        }
        else
        {
            return string.Empty;
        }
    }
}

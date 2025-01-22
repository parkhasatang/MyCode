using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField]
    private TMP_Text moneyText;

    private void Awake()
    {
        if (Instance != null) Destroy(this.gameObject);
        Instance = this;
    }

    private void Start()
    {
        CurrencyManager.Instance.currencyAction[CurrencyType.Money] += UpdateMoneyText;
        InvokeRepeating("GenerateMoney", 1.0f, 1.0f); // 1�ʸ��� GenerateMoney �Լ� ȣ��
    }

    private void GenerateMoney() // �ڵ����� ���� �����ϴ� �Լ�
    {
        CurrencyManager.Instance.ManipulateCurrency(CurrencyType.Money, FriendManager.Instance.totalMoney);
    }

    private void UpdateMoneyText() // �� ǥ�� �ؽ�Ʈ ������Ʈ
    {
        moneyText.text = CurrencyManager.Instance.GetCurrencyString(CurrencyType.Money);
    }
}

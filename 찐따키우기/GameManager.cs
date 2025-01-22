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
        InvokeRepeating("GenerateMoney", 1.0f, 1.0f); // 1초마다 GenerateMoney 함수 호출
    }

    private void GenerateMoney() // 자동으로 돈을 생성하는 함수
    {
        CurrencyManager.Instance.ManipulateCurrency(CurrencyType.Money, FriendManager.Instance.totalMoney);
    }

    private void UpdateMoneyText() // 돈 표시 텍스트 업데이트
    {
        moneyText.text = CurrencyManager.Instance.GetCurrencyString(CurrencyType.Money);
    }
}

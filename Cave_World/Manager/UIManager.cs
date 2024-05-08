using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject Player;
    // �÷��̾� ���� UI
    [SerializeField] private Slider HPSlider;
    [SerializeField] private Slider HungerSilder;

    [SerializeField] private TextMeshProUGUI HpTxt;
    [SerializeField] private TextMeshProUGUI HungerTxt;

    // ���� ���� UI
    //[SerializeField] private GameObject BossHPUI;
    //[SerializeField] private Slider BossHPBar;
    //[SerializeField] private TextMeshProUGUI BossName;

    private HealthSystem playerHealthSystem;
    public SpawnDamageUI spawnDamageUI;

    // �κ��丮 ������
    public Inventory playerInventoryData;
    public Item takeTemporaryItemData;
    public Item giveTemporaryItemData;
    public int takeTemporaryItemStack;
    public int giveTemporaryItemStack;

    //���â �ؽ�Ʈ
    public TextMeshProUGUI playerStatHP;
    public TextMeshProUGUI playerStatATK;
    public TextMeshProUGUI playerStatDEF;
    public TextMeshProUGUI playerStatMOV;
    public TextMeshProUGUI playerStatSight;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdatePlayerStatTxt()
    {
        CharacterStatHandler statHandler = Player.GetComponent<CharacterStatHandler>();
        playerStatHP.text = statHandler.CurrentStats.maxHP.ToString();
        playerStatATK.text = statHandler.CurrentStats.attackDamage.ToString();
        playerStatDEF.text = statHandler.CurrentStats.defense.ToString();
        playerStatMOV.text = statHandler.CurrentStats.speed.ToString();
        playerStatSight.text = 1.ToString();
        
    }
}

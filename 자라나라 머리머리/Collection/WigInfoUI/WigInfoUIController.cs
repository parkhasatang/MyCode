using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WigInfoUIController : UIController
{
    public GameObject unlockWigInfo;
    public GameObject lockWigInfo;

    public TMP_Text wigName;
    public Image wigImage;
    public TMP_Text wigRequireMaterial;
    public TMP_Text wigSellingPrice;
    public TMP_Text wigDescription;
    
    private void OnEnable()
    {
        // ���� ���õ� WigSO �����͸� ������ UI ������Ʈ
        WigSO wigData = CollectionManager.Instance.currentWigInfoData;
        if (wigData != null)
        {
            SetWigInfoData(wigData);
        }
        else
        {
            Debug.LogError("currentWigInfoData�� null�Դϴ�.");
        }
    }

    private void OnDisable()
    {
        // currentWigInfoData�� �ʱ�ȭ
        CollectionManager.Instance.currentWigInfoData = null;
    }

    private void SetWigInfoImage(bool isUnlocked)
    {
        if (isUnlocked)
        {
            lockWigInfo.SetActive(true);
            unlockWigInfo.SetActive(false);
        }
        else
        {
            lockWigInfo.SetActive(false);
            unlockWigInfo.SetActive(true);
        }
    }
    
    private void SetWigInfoData(WigSO wigData)
    {
        if (wigData == null)
        {
            Debug.LogError("wigData�� null�Դϴ�.");
            return;
        }
        
        if (CollectionManager.Instance.IsWigUnlocked(wigData.uniqueID))
        {
            SetWigInfoImage(false);
            
            wigName.text = wigData.wigName;
            wigImage.sprite = GameDataManager.Instance.GetWigSpriteByName(wigData.uniqueID);
            wigSellingPrice.text = wigData.uniqueID.StartsWith("WEV") ? "싯가" : CurrencySystem.ToCurrencyString(wigData.sellingPrice);
            wigRequireMaterial.text = wigData.hairMaterial;
            wigDescription.text = wigData.description;
        }
        else
        {
            // �ӽ�
            SetWigInfoImage(true);
        }
    }
}

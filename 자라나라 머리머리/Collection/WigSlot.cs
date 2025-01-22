using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WigSlot : MonoBehaviour
{
    private WigData wigData;

    public Image wigImage;
    public Image newImage;
    public Image pearlImage;
    
    private Image wigSlotBackGroundImage;
    private Button wigSlotBtn;
    private Tween newImageTween;
    
    private void Awake()
    {
        wigSlotBackGroundImage = GetComponent<Image>();
    }

    public void Initialize(WigData wigData)
    {
        this.wigData = wigData;

        if (wigSlotBtn == null)
            wigSlotBtn = GetComponent<Button>();

        wigSlotBtn.onClick.AddListener(OnWigSlotClicked);

        UpdateUI();
    }

    private void OnWigSlotClicked()
    {
        // 해금되었고, 첫 확인이 안된 경우
        if (wigData.isUnlocked && !wigData.isFirstChecked)
        {
            CurrencyEffectParticleController ps = GameManager.Instance.Pool.SpawnFromPool<CurrencyEffectParticleController>(E_PoolObjectType.CurrencyEffect);
            Vector3 worldPosition = pearlImage.rectTransform.position;
            ps.transform.position = new Vector3(worldPosition.x, worldPosition.y, 90);
            // 파티클이 닿으면 보상 지급
            ps.InitParticle(CurrencyType.Gem, true, true, 5, () => CollectionManager.Instance.SetWigFirstChecked(wigData.UniqueID));
            
            // 미확인 가발이 존재하지 않으면 체크 해제
            if (!CollectionManager.Instance.HasUnseenUnlockedWigs())
            {
                CollectionManager.Instance.collectionRedDotComponent.OnRedDotChecked();
            }

            SetNewImage(false);
        }
        else
        {
            CollectionManager.Instance.currentWigInfoData = wigData.wigSO;
            UIManager.Instance.ShowUI("WigInfoUI");
        }
    }

    public void UpdateUI()
    {
        if (wigData.isUnlocked)
        {
            wigSlotBackGroundImage.sprite = ResourceManager.Instance.GetResource<Sprite>("WigSlot_Unlock");
            wigImage.gameObject.SetActive(true);
            wigImage.sprite = GameDataManager.Instance.GetWigSpriteByName(wigData.wigSO.uniqueID);

            SetNewImage(!wigData.isFirstChecked);
        }
        else
        {
            wigImage.gameObject.SetActive(false);
            wigSlotBackGroundImage.sprite = ResourceManager.Instance.GetResource<Sprite>("WigSlot_Lock");
            SetNewImage(false);
        }
    }

    public void SetNewImage(bool isNew)
    {
        if (newImage == null)
        {
            Debug.LogWarning("newImage�� �����Ǿ� ���� �ʽ��ϴ�.");
            return;
        }

        if (isNew)
        {
            newImage.gameObject.SetActive(true);
            pearlImage.gameObject.SetActive(true);

            if (newImageTween != null && newImageTween.IsActive())
            {
                newImageTween.Kill();
            }

            newImage.transform.localScale = Vector3.one;

            newImageTween = newImage.transform.DOScale(1.2f, 0.5f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
        else
        {
            if (newImageTween != null && newImageTween.IsActive())
            {
                newImageTween.Kill();
                newImageTween = null;
            }

            newImage.gameObject.SetActive(false);
            pearlImage.gameObject.SetActive(false);
        }
    }
}

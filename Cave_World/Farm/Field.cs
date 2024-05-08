using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour // �翡 ���� ��ũ��Ʈ
{
    public GameObject seed;
    private Animator seedAnimation;

    [SerializeField] internal bool isWatering;
    [SerializeField] internal bool isSeed;
    [SerializeField] internal Item seedData;
    internal bool isGrowing;
    internal bool isGrowFinish;

    private void Awake()
    {
        seedAnimation = seed.GetComponent<Animator>();
    }

    private void OnEnable()
    {
        ReadyHarvest();
    }

    public void CheckIsSeed()
    {
        // ���� ������ �� �޾Ҵٸ�.
        if (isWatering && isSeed)
        {
            isGrowing = true;
            seed.SetActive (true);
            seedAnimation.SetInteger("ItemCode", seedData.ItemCode);
            Debug.Log(seedAnimation.ToString());
        }
    }

    public void HarvestState()
    {
        isGrowFinish = true;
    }

    public void ReadyHarvest()
    {
        seed.SetActive(false);
        isSeed = false;
        seedData = null;
        isWatering = false;
        isGrowFinish = false;
        isGrowing = false;
    }
}

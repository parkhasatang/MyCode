using UnityEngine;

public class TreasureBox : MonoBehaviour
{
    private Animator boxAnimator;
    private bool isOpened = false;

    void Start()
    {
        boxAnimator = GetComponent<Animator>();
    }

    public void OpenTreasureBox()
    {
        if (!isOpened)
        {
            isOpened = true;
            boxAnimator.SetBool("Open", true);
            ItemManager.instance.itemPool.ItemSpawn(3002, gameObject.transform.position);
            Destroy(gameObject, 2f);
        }
    }
}
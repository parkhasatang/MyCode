using UnityEngine;
using UnityEngine.EventSystems;

public class Clicker : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        CurrencyManager.Instance.ManipulateCurrency(CurrencyType.Money, 10000);
    }
}

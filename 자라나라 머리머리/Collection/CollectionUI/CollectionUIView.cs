using UnityEngine;

public class CollectionUIView : UIView
{

    protected override void Awake()
    {
        base.Awake();
        if (backButtons != null)
        {
            foreach (var button in backButtons)
            {
                button.onClick.AddListener(() => GameManager.Instance.Player.PluckController.SetPointerInputEnabled(true));
            }
        }
    }
}

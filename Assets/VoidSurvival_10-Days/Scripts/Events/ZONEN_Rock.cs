using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ZONEN_Rock : InteractAction
{
    [Header("岩があって通れないメッセージ")]
    [SerializeField]
    private List<string> rockMessages = new List<string>
        {
            "There's a rock so I can't pass!",
            "If there's something explosive, I might be able to destroy it..."
        };
    [Header("爆弾ある時のメッセージ")]
    [SerializeField]
    private List<string> clearMessages = new List<string>
        {
            "There's a bomb!",
            "Consume one bomb to destroy it."
        };
    [SerializeField]
    private Item bomb;

    private void OnEnable()
    {
        if (FlagData.Instance.destructionRock)
        {
            Destroy(gameObject);
        }
    }
    public override void Action()
    {
        StartCoroutine(Event());
    }

    private IEnumerator Event()
    {
        yield return StartCoroutine(MessageText.Instance.TextMessage(rockMessages));
        yield return new WaitForSeconds(0.5f);
        if (ItemManager.Instance.GetItemStack(bomb) != null)
        {
            yield return StartCoroutine(MessageText.Instance.TextMessage(clearMessages));
            yield return new WaitForSeconds(0.5f);
            ItemManager.Instance.RemoveItem(bomb);
            Destroy(gameObject);
            FlagData.Instance.destructionRock = true;
        }
    }
}

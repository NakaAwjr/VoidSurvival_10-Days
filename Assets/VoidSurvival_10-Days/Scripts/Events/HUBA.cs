using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUBA : MonoBehaviour
{
    [Header("クラフトチュートリアル")]
    [Tooltip("クラフトしよう！のメッセージ")]
    [SerializeField]
    private List<string> craftMessages = new List<string>
        {
            "Let's try crafting!"
        };
    [Tooltip("何を作ってほしいか")]
    [SerializeField] Item craftItem;
    private bool isCraft = false;

    [Header("作業台チュートリアル")]
    [SerializeField]
    private List<string> workBenchMessages = new List<string>
        {
            "Crafting successful!",
            "Next, let's use the workbench!\nLet's make a Weapon!"
        };
    [SerializeField] Item workBenchItem;
    private bool isWork = false;

    [Header("次の場所に行く")]
    [SerializeField]
    private List<string> nextMessages = new List<string>
        {
            "Working successful!\nHead north."
        };


    // Start is called before the first frame update
    void Start()
    {
        if (!FlagData.Instance.craftTutorial)
        {
            StartCoroutine(Event());
        }
    }
    private IEnumerator Event()
    {
        ItemManager.Instance.OnGetItem.AddListener(OnGetItem);
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(MessageText.Instance.TextMessage(craftMessages));
        yield return new WaitUntil(() => isCraft);
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(MessageText.Instance.TextMessage(workBenchMessages));
        yield return new WaitUntil(() => isWork);
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(MessageText.Instance.TextMessage(nextMessages));
        FlagData.Instance.craftTutorial = true;
        ItemManager.Instance.OnGetItem.RemoveListener(OnGetItem);
    }

    private void OnGetItem(Item item)
    {
        isCraft = item.Equals(craftItem);
        isWork = item.Equals(workBenchItem);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZONEN : MonoBehaviour
{
    [SerializeField] EnemyStatus enemy;
    [SerializeField]
    private List<string> enemyMessages = new List<string>
        {
            "Let's defeat the enemy!"
        };
    [SerializeField]
    private List<string> completeMessages = new List<string>
        {
            "Success!"
        };
    private bool isDie = false;

    private void Start()
    {
        if (FlagData.Instance.craftTutorial)
        {
            StartCoroutine(Event());
        }
    }
    private IEnumerator Event()
    {
        enemy.onDie.AddListener(onDie);
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(MessageText.Instance.TextMessage(enemyMessages));
        yield return new WaitUntil(() => isDie);
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(MessageText.Instance.TextMessage(completeMessages));
        enemy.onDie.RemoveListener(onDie);
    }

    private void onDie()
    {
        isDie = true;
    }
}

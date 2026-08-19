using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AZeroZero : MonoBehaviour
{
    private List<String> testMessage = new List<string>
    {
        "There was someone inside!",
        "Let's carry it to the base for now."
    };
    // Start is called before the first frame update
    void Start()
    {
        if (!FlagData.Instance.findSomeone)
        {
            StartCoroutine(Event());
        }
    }

    private IEnumerator Event()
    {
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(MessageText.Instance.TextMessage(testMessage));
        FlagData.Instance.findSomeone = true;
    }
}

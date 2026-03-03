using TMPro;
using UnityEngine;

public class ShowStatus : MonoBehaviour
{
    [SerializeField] private TMP_Text HPText;
    [SerializeField] private TMP_Text SPText;
    [SerializeField] private PlayerStatus playerStats;
    // Update is called once per frame
    void Update()
    {
        HPText.text = playerStats.CurrentHealth + " / " + playerStats.MaxHitPoint;
        SPText.text = playerStats.CurrentStaminaPoint + " / " + playerStats.MaxStaminaPoint;
    }
}

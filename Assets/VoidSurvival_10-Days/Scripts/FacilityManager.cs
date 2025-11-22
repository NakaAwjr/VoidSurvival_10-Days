using UnityEngine;

/// <summary>
/// 施設管理クラス
/// </summary>
public class FacilityManager : MonoBehaviour
{
    public enum FacilityType
    {
        None,
        PowerSpupply,
    }
    public static FacilityManager Instance { get; private set; }

    [SerializeField] private FacilityBase[] facilities;
    [SerializeField] private int damagePerMinute = 1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // 毎分施設にダメージを与える
        TimeManager.Instance.OnMinuteChanged.AddListener(DamageFacilitys);
    }

    /// <summary>
    /// 指定した施設を取得する
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public FacilityBase GetFacility(FacilityType type)
    {
        foreach (var facility in facilities)
        {
            if (facility.FacilityType == type)
            {
                return facility;
            }
        }
        return null;
    }
    /// <summary>
    /// 施設にダメージを与える
    /// </summary>
    private void DamageFacilitys()
    {
        foreach (var facility in facilities)
        {
            facility.DamageFacility(damagePerMinute);
        }
    }
    /// <summary>
    /// 全ての施設のバフをプレイヤーに付与する
    /// </summary>
    public void AllBuffAddToPlayer()
    {
        var buffManager = FindAnyObjectByType<PlayerStatus>().GetComponent<CharacterBuffManager>();
        for (int i = 0; i < facilities.Length; i++)
        {
            for (int j = 0; j < facilities[i].Buffs.Count; j++)
            {
                buffManager.ActiveBuffs.Add(facilities[i].Buffs[j] as CharacterBuff);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 作業台の本体
/// </summary>
public class WorkbenchManager
{
    public static WorkbenchManager Instance { get; private set; }
    private ItemDatabase _itemDatabase;
    private WorkbenchRecipeDataBase _workbenchRecipeDataBase;

    // 今作成しているレシピ
    private WorkbenchRecipe _workingRecipe;
    // 今作成している数
    private int _workingAmount;
    /// <summary>
    /// 現在の作業進行度
    /// </summary>
    public int WorkingProgress { get; private set; }
    /// <summary>
    /// 今作業台に入っている必要アイテム
    /// </summary>
    public ItemStack WorkingRequiredItem { get; private set; }
    /// <summary>
    /// 今作業台に入っている作成アイテム
    /// </summary>
    public ItemStack WorkingResultItem { get; private set; }

    /// <summary>
    /// 作業状態が変化したときに発火するイベント
    /// </summary>
    public UnityEvent OnChangeWorkingState = new UnityEvent();

    private WorkbenchManager(ItemDatabase itemDatabase, WorkbenchRecipeDataBase workbenchRecipeDataBase)
    {
        _itemDatabase = itemDatabase;
        _workbenchRecipeDataBase = workbenchRecipeDataBase;
        Instance = this;
    }
    public static void Initialize(ItemDatabase itemDatabase, WorkbenchRecipeDataBase workbenchRecipeDataBase)
    {
        if (Instance == null)
        {
            new WorkbenchManager(itemDatabase, workbenchRecipeDataBase);
        }
        // AllBuffs = FacilityManager.Instance.GetBuffs(BuffType.WorkbenchEfficiency);
    }

    /// <summary>
    /// 入力したアイテムから作られるアイテムのレシピを返す
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public List<WorkbenchRecipe> GetRecipes(Item item)
    {
        return _workbenchRecipeDataBase.GetRecipes(item);
    }
    /// <summary>
    /// 指定されたレシピに基づいてクラフト可能なアイテムの数を計算する
    /// </summary>
    /// <param name="recipe"></param>
    /// <returns></returns>ss
    public int NumberCanCrafting(WorkbenchRecipe recipe)
    {
        if (recipe == null)
        {
            Debug.LogError("Crafting recipe is null.");
            return 0;
        }

        int minCount = int.MaxValue;

        var itemStack = ItemManager.Instance.GetItemStack(recipe.RequiredItem.Item);
        if (itemStack == null || itemStack.Amount < recipe.RequiredItem.Amount)
        {
            return 0;
        }
        minCount = Mathf.Min(minCount, itemStack.Amount / recipe.RequiredItem.Amount);
        return minCount;
    }
    /// <summary>
    /// 作業を開始
    /// </summary>
    /// <param name="recipe"></param>
    /// <param name="amount"></param>
    public void StartCraftItem(WorkbenchRecipe recipe, int amount)
    {
        // エラー処理
        if (FacilityManager.Instance.GetFacility(FacilityManager.FacilityType.PowerSpupply).IsBroken)
        {
            Debug.LogWarning("Cannot start crafting: Power Supply facility is broken.");
            return;
        }
        if (recipe == null || amount <= 0)
        {
            Debug.LogError("Invalid crafting recipe or count.");
            return;
        }
        if (NumberCanCrafting(recipe) < amount)
        {
            Debug.LogWarning("Not enough resources to craft the item.");
            return;
        }
        if (WorkingResultItem != null && WorkingResultItem.Item != _workingRecipe.ResultItem)
        {
            Debug.LogWarning("Previous result items have not been collected.");
            return;
        }
        if (WorkingRequiredItem != null && WorkingRequiredItem.Amount > 0)
        {
            Debug.LogWarning("Previous required items have not been used up.");
            return;
        }

        _workingRecipe = recipe;
        _workingAmount = amount;
        WorkingRequiredItem = new ItemStack(recipe.RequiredItem.Item, recipe.RequiredItem.Amount * amount);
        OnChangeWorkingState.Invoke();
        ItemManager.Instance.RemoveItem(recipe.RequiredItem.Item, recipe.RequiredItem.Amount * amount);
        TimeManager.Instance.OnMinuteChanged.AddListener(Crafting);
    }
    /// <summary>
    /// 作業を停止
    /// Craftingが終わったとき、またはRequiredItemを回収したときに呼び出される
    /// </summary>
    public void StopCraftItem()
    {
        _workingRecipe = null;
        _workingAmount = 0;
        WorkingProgress = 0;
        // RequiredItemを返却
        if (WorkingRequiredItem != null)
        {
            ItemManager.Instance.AddItem(WorkingRequiredItem.Item, WorkingRequiredItem.Amount);
            WorkingRequiredItem = null;
            OnChangeWorkingState.Invoke();
        }
        TimeManager.Instance.OnMinuteChanged.RemoveListener(Crafting);
    }
    /// <summary>
    /// 作成アイテムを回収
    /// </summary>
    public void CollectResultItem()
    {
        if (WorkingResultItem != null)
        {
            ItemManager.Instance.AddItem(WorkingResultItem.Item, WorkingResultItem.Amount);
            WorkingResultItem = null;
            OnChangeWorkingState.Invoke();
        }
    }
    /// <summary>
    /// クラフト進行
    /// TimeManagerの一定時間ごとに呼び出される
    /// </summary>
    public void Crafting()
    {
        if (FacilityManager.Instance.GetFacility(FacilityManager.FacilityType.PowerSpupply).IsBroken)
        {
            Debug.LogWarning("Crafting paused: Power Supply facility is broken.");
            TimeManager.Instance.OnMinuteChanged.RemoveListener(Crafting);
            return;
        }

        // エラー処理
        if (_workingRecipe == null || _workingAmount <= 0)
        {
            Debug.LogWarning("No crafting in progress.");
            return;
        }
        if (WorkingResultItem != null && WorkingResultItem.Item != _workingRecipe.ResultItem)
        {
            Debug.LogWarning("Previous result items have not been collected.");
            return;
        }
        if (WorkingRequiredItem == null || WorkingRequiredItem.Amount < _workingRecipe.RequiredItem.Amount)
        {
            Debug.LogWarning("Not enough required items to continue crafting.");
            return;
        }

        Debug.Log("元の進行度: " + _workingRecipe.time + ", 現在の進行度: " + WorkingTime());

        // 進行
        WorkingProgress++;
        if (WorkingProgress >= WorkingTime())
        {
            // クラフト完了
            if (WorkingResultItem == null)
            {
                WorkingResultItem = new ItemStack(_workingRecipe.ResultItem, _workingRecipe.ResultAmount);
            }
            else
            {
                WorkingResultItem.Add(_workingRecipe.ResultAmount);
            }
            WorkingRequiredItem.Remove(_workingRecipe.RequiredItem.Amount);
            if (WorkingRequiredItem.Amount <= 0)
            {
                WorkingRequiredItem = null;
            }
            _workingAmount--;
            WorkingProgress = 0;
        }
        // 作業完了チェック
        if (_workingAmount <= 0)
        {
            StopCraftItem();
        }
        OnChangeWorkingState.Invoke();
    }
    /// <summary>
    /// 現在のレシピの作成にかかる時間を返す
    /// </summary>
    /// <returns></returns>
    public int WorkingTime()
    {
        if (_workingRecipe != null)
        {
            float time = _workingRecipe.time;
            var AllBuffs = OthersBuffManager.GetBuffs(OthersBuffType.WorkbenchEfficiency);
            foreach (var buff in AllBuffs)
            {
                if (buff.OperationType == BuffOperationType.Multiply)
                {
                    time = time * (1 + buff.GetValue());
                }
                else if (buff.OperationType == BuffOperationType.Add)
                {
                    time += buff.GetValue();
                }
                Debug.Log(buff.Description + " applied. New time: " + time);
            }
            return Mathf.CeilToInt(time);
        }
        return 0;
    }

    public void FromSaveData(WorkbenchSaveData data)
    {
        _workingRecipe = data.WorkingRecipe != -1 ? _workbenchRecipeDataBase.GetValue(_itemDatabase.GetValue(data.WorkingRecipe)) : null;
        _workingAmount = data.WorkingAmount;
        WorkingProgress = data.WorkingProgress;
        WorkingRequiredItem = data.WorkingRequiredItem.ItemID != -1 ? new ItemStack(_itemDatabase.GetValue(data.WorkingRequiredItem.ItemID), data.WorkingRequiredItem.Amount) : null;
        WorkingResultItem = data.WorkingResultItem.ItemID != -1 ? new ItemStack(_itemDatabase.GetValue(data.WorkingResultItem.ItemID), data.WorkingResultItem.Amount) : null;
        if (_workingRecipe != null && WorkingRequiredItem != null && !FacilityManager.Instance.GetFacility(FacilityManager.FacilityType.PowerSpupply).IsBroken)
        {
            TimeManager.Instance.OnMinuteChanged.AddListener(Crafting);
        }
    }
    public WorkbenchSaveData ToSaveData()
    {
        WorkbenchSaveData data = new WorkbenchSaveData();
        data.WorkingRecipe = _workingRecipe != null ? _workingRecipe.ResultItem.ItemID : -1;
        data.WorkingAmount = _workingAmount;
        data.WorkingProgress = WorkingProgress;
        data.WorkingRequiredItem = WorkingRequiredItem != null ? new ItemStackSaveData()
        {
            ItemID = WorkingRequiredItem.Item.ItemID,
            Amount = WorkingRequiredItem.Amount
        } : new ItemStackSaveData()
        {
            ItemID = -1,
            Amount = 0
        };
        data.WorkingResultItem = WorkingResultItem != null ? new ItemStackSaveData()
        {
            ItemID = WorkingResultItem.Item.ItemID,
            Amount = WorkingResultItem.Amount
        } : new ItemStackSaveData()
        {
            ItemID = -1,
            Amount = 0
        };
        return data;
    }
}

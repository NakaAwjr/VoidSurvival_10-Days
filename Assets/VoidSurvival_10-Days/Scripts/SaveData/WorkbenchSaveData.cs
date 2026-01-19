[System.Serializable]
public struct WorkbenchSaveData
{
    public int WorkingRecipe;
    public int WorkingAmount;
    public int WorkingProgress;
    public ItemStackSaveData WorkingRequiredItem;
    public ItemStackSaveData WorkingResultItem;
}
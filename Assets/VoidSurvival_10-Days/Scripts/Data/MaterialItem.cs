using UnityEngine;

[CreateAssetMenu(fileName = "NewMaterialItem", menuName = "Items/MaterialItem")]
public class MaterialItem : Item, IDatabaseEntry<int>
{
    public override bool CanUseOn()
    {
        // 素材アイテムは使用できないため、常にfalseを返す
        return false;
    }

    public override void Use(GameObject player)
    {
        // 素材アイテムは使用できないため、何もしない
        Debug.Log("This item cannot be used.");
    }
}

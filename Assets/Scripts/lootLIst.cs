using UnityEngine;

[CreateAssetMenu]
public class LootItem : ScriptableObject
{
    public Sprite lootSprite;
    public string lootName;
    public int dropChance;

    public LootItem(string lootName, int dropChance)
    {
        this.lootName = lootName;
        this.dropChance = dropChance;
    }


}

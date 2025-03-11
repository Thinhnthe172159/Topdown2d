using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class Lootbag : MonoBehaviour
{

    public GameObject DropItemPrefab;
    public List<LootItem> LootList = new List<LootItem>();
    private

   LootItem GetDroppedItem()
    {
        int RandomNumber = Random.Range(1, 101);
        List<LootItem> possibleItem = new List<LootItem>();
        foreach (LootItem item in LootList)
        {
            if (RandomNumber <= item.dropChance)
            {
                possibleItem.Add(item);

            }
        }
        if (possibleItem.Count > 0)
        {
            LootItem dropItem = possibleItem[Random.Range(0, possibleItem.Count)];
            return dropItem;
        }
        return null;
    }

    public void InstantiateLoot(Vector3 spawnPosition)
    {
        LootItem item = GetDroppedItem();
        if (item != null)
        {
            GameObject gameObject = Instantiate(DropItemPrefab, spawnPosition, Quaternion.identity);
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            gameObject.GetComponent<SpriteRenderer>().sprite = item.lootSprite;
            gameObject.layer = LayerMask.NameToLayer("Default");
            spriteRenderer.sortingOrder = 10;

            float dropForce = 300f;
            Vector2 dropDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            gameObject.GetComponent<Rigidbody2D>().AddForce(dropDirection * dropForce,ForceMode2D.Impulse);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}

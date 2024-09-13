using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LootBox : MonoBehaviour
{
    [Serializable]
    public class LootItem
    {
        public string itemType;
        public string itemName;
        public int amount;
        public string animationName;
    }

    [Serializable]
    public class LootData
    {
        public List<LootBox.ItemPool> itemPools;
    }

    [Serializable]
    public class ItemPool
    {
        public float probability;
        public List<LootItem> items;
    }

    public List<ItemPool> itemPools;

    private List<LootItem> receivedItems = new List<LootItem>();

    private void Start()
    {
        LoadLootBoxData();
    }

    public void OpenLootBox()
    {
        int itemsToReceive = 2;
        List<LootItem> items = new List<LootItem>();

        // Determine how many unique items are left
        List<LootItem> uniqueItems = new List<LootItem>();
        foreach (var pool in itemPools)
        {
            foreach (var item in pool.items)
            {
                if (!receivedItems.Contains(item) && !uniqueItems.Contains(item))
                {
                    uniqueItems.Add(item);
                }
            }
        }

        int uniqueItemCount = uniqueItems.Count;
        if (uniqueItemCount == 0)
        {
            // No unique items left, return no items
            Debug.Log("No items left to receive from the loot box.");
            return;
        }
        else if (uniqueItemCount == 1)
        {
            // Only one unique item left, return that one item
            items.Add(uniqueItems[0]);
            receivedItems.Add(uniqueItems[0]);
        }
        else
        {
            // Two or more unique items left, return two items
            int itemsToAdd = Mathf.Min(itemsToReceive, uniqueItemCount);
            for (int i = 0; i < itemsToAdd; i++)
            {
                LootItem item = GetRandomUniqueItem(uniqueItems);
                if (item != null)
                {
                    items.Add(item);
                    receivedItems.Add(item);
                }
            }
        }

        DisplayItems(items);
    }

    private LootItem GetRandomUniqueItem(List<LootItem> uniqueItems)
    {
        if (uniqueItems.Count == 0)
        {
            return null; // No unique item left
        }

        int randomIndex = Random.Range(0, uniqueItems.Count);
        LootItem item = uniqueItems[randomIndex];
        uniqueItems.RemoveAt(randomIndex);
        return item;
    }

    private void LoadLootBoxData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("loot_box_data");
        if (jsonFile == null)
        {
            Debug.LogError("Failed to load loot box data JSON file.");
            return;
        }

        // Deserialize the JSON data into the itemPools list
        itemPools = JsonUtility.FromJson<LootData>(jsonFile.text).itemPools;
    }

    private LootItem GetRandomItem()
    {
        float randomValue = Random.Range(0f, 1f);
        float cumulativeProbability = 0f;

        foreach (var pool in itemPools)
        {
            cumulativeProbability += pool.probability;
            if (randomValue <= cumulativeProbability)
            {
                return GetUniqueItemFromPool(pool);
            }
        }

        return null;
    }

    private LootItem GetUniqueItemFromPool(ItemPool pool)
    {
        List<LootItem> uniqueItems = new List<LootItem>();

        foreach (var item in pool.items)
        {
            if (!receivedItems.Contains(item))
            {
                uniqueItems.Add(item);
            }
        }

        if (uniqueItems.Count == 0)
        {
            return null; // No unique item left in this pool
        }

        int randomIndex = Random.Range(0, uniqueItems.Count);
        return uniqueItems[randomIndex];
    }

    private void DisplayItems(List<LootItem> items)
    {
        foreach (var item in items)
        {
            switch (item.itemType)
            {
                case "Emote":
                    Debug.Log($"You got a new emote: {item.animationName}");
                    break;
                case "Coins":
                    Debug.Log($"You got {item.amount} coins");
                    break;
                case "Skin":
                    Debug.Log($"You got a new skin: {item.itemName}");
                    break;
                default:
                    Debug.LogError($"Unknown item type: {item.itemType}");
                    break;
            }
        }
    }
    public void Home()
    {
        SceneManager.LoadScene("Main Scene");
    }
}

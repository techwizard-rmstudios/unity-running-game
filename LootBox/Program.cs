using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LootBox
{
    internal class Program
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
            public List<Program.ItemPool> itemPools;
        }

        [Serializable]
        public class ItemPool
        {
            public float probability;
            public List<LootItem> items;
        }

        public static List<ItemPool> itemPools;

        private static List<LootItem> receivedItems = new List<LootItem>();

        public static LootItem GetRandomUniqueItem(List<LootItem> uniqueItems)
        {
            if (uniqueItems.Count == 0)
            {
                return null; // No unique item left
            }

            int randomIndex = new Random().Next(0, uniqueItems.Count);
            LootItem item = uniqueItems[randomIndex];
            uniqueItems.RemoveAt(randomIndex);
            return item;
        }

        private LootItem GetRandomItem()
        {
            float randomValue = new Random().Next(0, 100) / 100.0f;
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

            int randomIndex = new Random().Next(0, uniqueItems.Count);
            return uniqueItems[randomIndex];
        }

        private static void DisplayItems(List<LootItem> items)
        {
            foreach (var item in items)
            {
                switch (item.itemType)
                {
                    case "Emote":
                        Console.WriteLine($"You got a new emote: {item.animationName}");
                        break;
                    case "Coins":
                        Console.WriteLine($"You got {item.amount} coins");
                        break;
                    case "Skin":
                        Console.WriteLine($"You got a new skin: {item.itemName}");
                        break;
                    default:
                        Console.WriteLine($"Unknown item type: {item.itemType}");
                        break;
                }
            }
        }
        static void Main(string[] args)
        {
            string jsonFile = File.ReadAllText(System.AppDomain.CurrentDomain.BaseDirectory + "loot_box_data.json");

            if (jsonFile == null)
            {
                Console.WriteLine("Failed to load loot box data JSON file.");
                return;
            }

            // Deserialize the JSON data into the itemPools list
            List<ItemPool> itemPools = JsonConvert.DeserializeObject<List<ItemPool>>(jsonFile);

            while (true)
            {
                Console.ReadLine();

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
                    Console.WriteLine("No items left to receive from the loot box.");
                    continue;
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
                    int itemsToAdd = Math.Min(itemsToReceive, uniqueItemCount);
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
        }
    }
}

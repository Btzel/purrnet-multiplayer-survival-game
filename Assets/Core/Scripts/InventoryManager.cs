using NUnit.Framework;
using PurrNet;
using PurrNet.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private InventoryItem itemPrefab;
    [SerializeField] private List<InventorySlot> slots;

    [PurrReadOnly, SerializeField]private InventoryItemData[] inventoryData;

    private void Awake()
    {
        InstanceHandler.RegisterInstance(this);
        inventoryData = new InventoryItemData[slots.Count];
    }

    private void OnDestroy()
    {
        InstanceHandler.UnregisterInstance<InventoryManager>();
    }

    public void AddItem(Item item)
    {
        if(!TryStackItem(item))
            AddNewItem(item);
    }

    private bool TryStackItem(Item item)
    {
        return false;
    }

    private void AddNewItem(Item item)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            InventorySlot slot = slots[i];
            if (!slot.isEmpty)
                continue;

            var inventoryItem = Instantiate(itemPrefab, slot.transform);
            inventoryItem.Init(item.ItemName, item.ItemPicture,1);

            var itemData = new InventoryItemData()
            {
                item = item,
                inventoryItem = inventoryItem,
                amount = 1
            };

            inventoryData[i] = itemData;
            slot.SetItem(inventoryItem);
            return;
        }
    }
    public void ItemMoved(InventoryItem item, InventorySlot newSlot)
    {

    }

    [Serializable]
    public struct InventoryItemData
    {
        public Item item;
        public InventoryItem inventoryItem;
        public int amount;
    }
}

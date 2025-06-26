using NUnit.Framework;
using PurrNet;
using PurrNet.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private List<Item> allItems = new();

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private InventoryItem itemPrefab;
    [SerializeField] private List<InventorySlot> slots;

    [PurrReadOnly, SerializeField]private InventoryItemData[] inventoryData;

    private void Awake()
    {
        InstanceHandler.RegisterInstance(this);
        inventoryData = new InventoryItemData[slots.Count];
        ToggleInventory(false);
    }

    private void OnDestroy()
    {
        InstanceHandler.UnregisterInstance<InventoryManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            bool isOpen = canvasGroup.alpha > 0;
            ToggleInventory(!isOpen);
        }
    }

    private void ToggleInventory(bool toggle)
    {
        
        canvasGroup.alpha = toggle ? 1 : 0;
        canvasGroup.blocksRaycasts = toggle;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = toggle;
    }

    public void AddItem(Item item)
    {
        if (!TryStackItem(item))
            AddNewItem(item);
       
            
    }

    private bool TryStackItem(Item item)
    {
        

        for (int i = 0; i < inventoryData.Length; i++)
        {
            var data = inventoryData[i];
            if (string.IsNullOrEmpty(data.itemName))
                continue;
            if (data.itemName!= item.ItemName)
                continue;

            data.amount++;
            data.inventoryItem.Init(item.ItemName, item.ItemPicture, data.amount);
            inventoryData[i] = data;
            return true;
        }

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
                itemName = item.ItemName,
                itemPicture = item.ItemPicture,
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
        var newSlotIndex = slots.IndexOf(newSlot);
        var oldSlotIndex = Array.FindIndex(inventoryData, x => x.inventoryItem == item);
        if(oldSlotIndex == -1)
        {
            Debug.LogError($"Couldnt find item {item.name} in inventory data", this);
            return;
        }

        var oldData = inventoryData[oldSlotIndex];

        inventoryData[oldSlotIndex] = default;
        inventoryData[newSlotIndex] = oldData;
    }

    public void DropItem(InventoryItem inventoryItem)
    {
        for (int i = 0; i < inventoryData.Length; i++)
        {
            var data = inventoryData[i];
            if (data.inventoryItem != inventoryItem)
                continue;

            var itemToSpawn = allItems.Find(x => x.ItemName == data.itemName);
            if (itemToSpawn == null)
                return;

            Vector3 spawnPosition = PlayerMovement.localPlayerMovement.transform.position + PlayerMovement.localPlayerMovement.transform.forward + Vector3.up;
            var item = Instantiate(itemToSpawn, spawnPosition, Quaternion.identity);


            DeductItem(inventoryItem);
            break;
        }
    }

    private void DeductItem(InventoryItem inventoryItem)
    {
        for (int i = 0; i < inventoryData.Length; i++)
        {
            var data = inventoryData[i];
            if (data.inventoryItem != inventoryItem)
                continue;

            data.amount--;
            if(data.amount <= 0)
            {
                inventoryData[i] = default;
                slots[i].SetItem(null);
                Destroy(inventoryItem.gameObject);
            }
            else
            {
                data.inventoryItem.Init(data.itemName, data.itemPicture, data.amount);
                inventoryData[i] = data;
            }
        }
    }

    [Serializable]
    public struct InventoryItemData
    {
        public string itemName;
        public Sprite itemPicture;
        public InventoryItem inventoryItem;
        public int amount;
    }
}

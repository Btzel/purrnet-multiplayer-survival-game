using NUnit.Framework;
using PurrNet;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private InventoryItem itemPrefab;
    [SerializeField] private List<InventorySlot> slots;

    private InventoryItem[] inventoryData;

    private void Awake()
    {
        InstanceHandler.RegisterInstance(this);
    }

    private void OnDestroy()
    {
        InstanceHandler.UnregisterInstance<InventoryManager>();
    }

    public void AddItem()
    {

    }
    public void ItemMoved()
    {

    }

    public struct InventoryItemData
    {
        public int amount;
    }
}

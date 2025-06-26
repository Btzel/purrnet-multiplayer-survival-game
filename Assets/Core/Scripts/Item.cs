using PurrNet;
using UnityEngine;

public class Item : AInteractable
{
    [SerializeField] private string itemName;
    [SerializeField] private Sprite itemPicture;
    [SerializeField] private Rigidbody rigidBody;

    public string ItemName => itemName;
    public Sprite ItemPicture => itemPicture;

    protected override void OnSpawned()
    {
        base.OnSpawned();

        if (!isServer)
            rigidBody.isKinematic = true;
    }




    [ContextMenu("Test Pickup")]
    public void Pickup()
    {
        if (!InstanceHandler.TryGetInstance(out InventoryManager inventoryManager))
        {
            Debug.LogError($"Couldnt get inventory manager for item {itemName}", this);
            return;
        }
        inventoryManager.AddItem(this);
        Destroy(gameObject);
    }


    public override void Interact()
    {
        Pickup();
    }
    

}

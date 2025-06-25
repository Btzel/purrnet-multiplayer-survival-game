using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour,IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        eventData.pointerDrag.transform.SetParent(transform);
        eventData.pointerDrag.GetComponent<InventoryItem>().SetAvailable();
    }

   
}

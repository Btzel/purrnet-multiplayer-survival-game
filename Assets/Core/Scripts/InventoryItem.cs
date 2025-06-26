using PurrNet;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler,IDragHandler,IEndDragHandler,IPointerClickHandler
{
    [SerializeField] private TMP_Text amountText;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private Canvas canvas;
    private Image itemImage;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        itemImage = GetComponent<Image>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        canvasGroup.blocksRaycasts = false;
        rectTransform.SetParent(rectTransform.root);
    }

    public void OnDrag(PointerEventData eventData)
    {
       
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
            

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.pointerEnter == null || !eventData.pointerEnter.TryGetComponent(out InventorySlot inventorySlot))
        {
            
            rectTransform.SetParent(originalParent);
            SetAvailable();
        }
        
    }

    public void SetAvailable()
    {
        canvasGroup.blocksRaycasts = true;
        rectTransform.anchoredPosition = Vector2.zero;
    }

    public void Init(string itemName, Sprite itemPicture, int amount)
    {
        itemImage.sprite = itemPicture;
        amountText.text = amount.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right)
            return;

        if(!InstanceHandler.TryGetInstance(out InventoryManager inventoryManager))
        {
            return;
        }

        inventoryManager.DropItem(this);
    }
}

using PurrNet;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private float interactDistance = 4f;

    private Camera cam;
    private AInteractable[] currentHoveredInteractables;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        HandleHovers();

        if (!Input.GetMouseButtonDown(0) && !Input.GetKeyDown(KeyCode.E))
            return;

        if (!Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, interactDistance, interactableLayer))
            return;

        var interactables = hit.collider.GetComponents<AInteractable>();

        foreach (var interactable in interactables)
        {
            if (interactable.CanInteract())
                interactable.Interact();
        }
    }
    private void HandleHovers()
    {
        if (!Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, interactDistance, interactableLayer))
        {
            ClearHover();
            return;

        }
        var interactables = hit.collider.GetComponents<AInteractable>();
        if(interactables == null || interactables.Length == 0)
        {
            ClearHover();
            return;
        }

        if (currentHoveredInteractables != null && currentHoveredInteractables.Length > 0 && hit.collider.gameObject == currentHoveredInteractables[0].gameObject)
            return;

        currentHoveredInteractables = interactables;
        foreach (var interactable in interactables)
        {
            if (interactable.CanInteract())
                interactable.OnHover();
                
        }
    }

    private void ClearHover()
    {
        if (currentHoveredInteractables == null || currentHoveredInteractables.Length <= 0)
            return;
        foreach (var interactable in currentHoveredInteractables)
        {
            interactable.OnStopHover();
        }
        
        currentHoveredInteractables = null;
    }
}

public abstract class AInteractable : NetworkBehaviour
{
    public abstract void Interact();

    public virtual void OnHover() { }
    public virtual void OnStopHover() { }
    public virtual bool CanInteract()
    {
        return true;
    }
}
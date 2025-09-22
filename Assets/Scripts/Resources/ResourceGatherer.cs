using UnityEngine;
using UnityEngine.InputSystem;

public class ResourceGatherer : MonoBehaviour
{
    [Header("Gathering")]
    public float gatherCooldown = 0.5f;
    
    [Header("UI")]
    public GameObject interactionPrompt;
    
    [Header("Input")]
    public InputActionReference gatherAction;
    
    private float lastGatherTime;
    private ResourceNode currentResource;
    private SimpleInventory inventory;
    
    private void Start()
    {
        inventory = GetComponent<SimpleInventory>();
        if (inventory == null)
        {
            Debug.LogError("ResourceGatherer requires SimpleInventory component!");
        }
        
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }
    
    private void OnEnable()
    {
        if (gatherAction != null)
        {
            gatherAction.action.performed += OnGatherInput;
            gatherAction.action.Enable();
        }
    }
    
    private void OnDisable()
    {
        if (gatherAction != null)
        {
            gatherAction.action.performed -= OnGatherInput;
            gatherAction.action.Disable();
        }
    }
    
    private void Update()
    {
        CheckForResources();
    }
    
    private void CheckForResources()
    {
        ResourceNode nearestResource = null;
        float nearestDistance = float.MaxValue;
        
        // Find all resource nodes in scene
        ResourceNode[] allResources = FindObjectsOfType<ResourceNode>();
        
        foreach (var resource in allResources)
        {
            if (!resource.isAvailable) continue;
            
            float distance = Vector3.Distance(transform.position, resource.transform.position);
            if (distance <= resource.interactionRange && distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestResource = resource;
            }
        }
        
        // Update current resource and UI
        if (currentResource != nearestResource)
        {
            currentResource = nearestResource;
            UpdateInteractionUI();
        }
    }
    
    private void UpdateInteractionUI()
    {
        if (interactionPrompt != null)
        {
            bool showPrompt = currentResource != null && currentResource.isAvailable;
            interactionPrompt.SetActive(showPrompt);
        }
    }
    
    private void OnGatherInput(InputAction.CallbackContext context)
    {
        if (CanGather())
        {
            GatherCurrentResource();
        }
    }
    
    private bool CanGather()
    {
        return currentResource != null && 
               currentResource.isAvailable && 
               Time.time >= lastGatherTime + gatherCooldown;
    }
    
    private void GatherCurrentResource()
    {
        if (currentResource.TryGather(gameObject))
        {
            lastGatherTime = Time.time;
            Debug.Log($"Successfully gathered from {currentResource.resourceName}");
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        // Visualize interaction range
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 5f); // Default interaction check range
    }
}
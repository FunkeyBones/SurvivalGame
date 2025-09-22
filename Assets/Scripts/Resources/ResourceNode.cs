using UnityEngine;

public abstract class ResourceNode : MonoBehaviour
{
    [Header("Resource Info")]
    public string resourceName = "Resource";
    public int maxHitPoints = 3;
    public float respawnTime = 30f;
    
    [Header("Drops")]
    public ResourceDrop[] possibleDrops;
    
    [Header("Interaction")]
    public float interactionRange = 3f;
    public LayerMask playerLayerMask = 1; // Default layer
    
    [Header("Visual Feedback")]
    public GameObject highlightEffect;
    public ParticleSystem gatherEffect;
    public AudioClip gatherSound;
    public AudioClip depletedSound;
    
    [Header("State")]
    public bool isAvailable = true;
    
    protected int currentHitPoints;
    protected bool playerInRange = false;
    protected AudioSource audioSource;
    
    protected virtual void Start()
    {
        currentHitPoints = maxHitPoints;
        audioSource = GetComponent<AudioSource>();
        
        if (highlightEffect != null)
            highlightEffect.SetActive(false);
    }
    
    protected virtual void Update()
    {
        CheckForPlayer();
    }
    
    protected virtual void CheckForPlayer()
    {
        bool wasInRange = playerInRange;
        
        Collider[] players = Physics.OverlapSphere(transform.position, interactionRange, playerLayerMask);
        playerInRange = players.Length > 0;
        
        // Show/hide highlight when player enters/exits range
        if (playerInRange != wasInRange && highlightEffect != null)
        {
            highlightEffect.SetActive(playerInRange && isAvailable);
        }
    }
    
    public virtual bool TryGather(GameObject gatherer)
    {
        if (!isAvailable || !playerInRange)
            return false;
        
        currentHitPoints--;
        
        // Play gather effect and sound
        if (gatherEffect != null)
            gatherEffect.Play();
        
        if (audioSource != null && gatherSound != null)
            audioSource.PlayOneShot(gatherSound);
        
        // Give resources to player
        var inventory = gatherer.GetComponent<SimpleInventory>();
        if (inventory != null)
        {
            GiveResources(inventory);
        }
        
        // Check if resource is depleted
        if (currentHitPoints <= 0)
        {
            OnResourceDepleted();
        }
        
        return true;
    }
    
    protected virtual void GiveResources(SimpleInventory inventory)
    {
        foreach (var drop in possibleDrops)
        {
            if (Random.Range(0f, 1f) <= drop.dropChance)
            {
                int amountToDrop = Random.Range(drop.minAmount, drop.maxAmount + 1);
                bool success = inventory.AddItem(drop.itemData, amountToDrop);
                
                if (!success)
                {
                    Debug.LogWarning($"Couldn't add {amountToDrop} {drop.itemData.itemName} - inventory full!");
                }
            }
        }
    }
    
    protected virtual void OnResourceDepleted()
    {
        isAvailable = false;
        
        if (highlightEffect != null)
            highlightEffect.SetActive(false);
        
        if (audioSource != null && depletedSound != null)
            audioSource.PlayOneShot(depletedSound);
        
        // Start respawn timer
        if (respawnTime > 0)
        {
            Invoke(nameof(RespawnResource), respawnTime);
        }
        
        // Hide the visual representation
        SetVisualState(false);
    }
    
    protected virtual void RespawnResource()
    {
        currentHitPoints = maxHitPoints;
        isAvailable = true;
        SetVisualState(true);
        
        Debug.Log($"{resourceName} has respawned!");
    }
    
    protected abstract void SetVisualState(bool active);
    
    protected virtual void OnDrawGizmosSelected()
    {
        // Draw interaction range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
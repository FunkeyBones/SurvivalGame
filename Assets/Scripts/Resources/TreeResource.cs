using UnityEngine;

public class TreeResource : ResourceNode
{
    [Header("Tree Specific")]
    public GameObject treeMesh;
    public GameObject stumpMesh;
    public float fallAnimation = 1f;
    
    private bool isFalling = false;
    
    protected override void Start()
    {
        base.Start();
        resourceName = "Tree";
        
        // Make sure stump is hidden initially
        if (stumpMesh != null)
            stumpMesh.SetActive(false);
    }
    
    protected override void SetVisualState(bool active)
    {
        if (active)
        {
            // Resource is available - show tree, hide stump
            if (treeMesh != null)
                treeMesh.SetActive(true);
            if (stumpMesh != null)
                stumpMesh.SetActive(false);
        }
        else
        {
            // Resource is depleted - show stump, hide tree (with animation if possible)
            if (!isFalling)
                StartCoroutine(FallAnimation());
        }
    }
    
    private System.Collections.IEnumerator FallAnimation()
    {
        isFalling = true;
        
        if (treeMesh != null)
        {
            // Simple fall animation - rotate tree
            float elapsed = 0f;
            Vector3 originalRotation = treeMesh.transform.eulerAngles;
            
            while (elapsed < fallAnimation)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / fallAnimation;
                
                // Rotate tree to simulate falling
                treeMesh.transform.rotation = Quaternion.Euler(
                    originalRotation.x,
                    originalRotation.y,
                    originalRotation.z + (90f * progress)
                );
                
                yield return null;
            }
            
            treeMesh.SetActive(false);
        }
        
        // Show stump
        if (stumpMesh != null)
            stumpMesh.SetActive(true);
        
        isFalling = false;
    }
}
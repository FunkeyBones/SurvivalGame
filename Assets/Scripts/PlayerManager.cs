using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;
    
    [Header("Hunger & Thirst")]
    public float maxHunger = 100f;
    public float maxThirst = 100f;
    public float currentHunger;
    public float currentThirst;
    
    [Header("Depletion Rates")]
    public float hungerDepletionRate = 1.5f; // per minute
    public float thirstDepletionRate = 2f; // per minute
    
    [Header("Damage from Needs")]
    public float starvationDamage = 5f; // per minute when hunger = 0
    public float dehydrationDamage = 8f; // per minute when thirst = 0
}

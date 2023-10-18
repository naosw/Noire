using Unity.Collections;
using UnityEngine;

/// <summary>
/// Manages the players "health"(drowsiness) for the whole game.
/// </summary>
[CreateAssetMenu(fileName = "Stamina", menuName = "Player/Stamina")]
public class PlayerStaminaSO : ScriptableObject
{
    [SerializeField] private float staminaRegenRate = 8f;
    
    private float max_stamina = 100;
    private float currentStamina;
    
    public float CurrentStamina => currentStamina;
    
    public float CurrentStaminaPercentage => currentStamina / max_stamina;

    public void SetCurrentStamina(float newValue) => currentStamina = newValue;
	
    // returns true upon successful use of stamina
    public bool UseStamina(float value)
    {
        if (value > currentStamina) 
            return false;
        
        currentStamina -= value;
        return true;
    }
    
    // returns true upon successful regen. Should not decrease potion otherwise
    public bool RegenStamina()
    {
        if (currentStamina >= max_stamina)
            return false;
        
        currentStamina += staminaRegenRate * Time.deltaTime;
        if (currentStamina >= max_stamina)
            currentStamina = max_stamina;
        return true;
    }

    public void ResetStamina()
    {
        currentStamina = max_stamina;
    }
    
    public void SetMaxStamina(float x) => max_stamina = x;
}

using Unity.Collections;
using UnityEngine;

/// <summary>
/// Manages the players "health"(drowsiness) for the whole game.
/// </summary>
[CreateAssetMenu(fileName = "Stamina", menuName = "Player/Stamina")]
public class PlayerStaminaSO : ScriptableObject
{
    [SerializeField] private float staminaRegenRate = 8f;
    
    private const float MAX_STAMINA = 100;
    private float currentStamina;
    
    public float CurrentStamina => currentStamina;
    
    public float CurrentStaminaPercentage => currentStamina / MAX_STAMINA;

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
        if (currentStamina >= MAX_STAMINA)
            return false;
        
        currentStamina += staminaRegenRate * Time.deltaTime;
        if (currentStamina >= MAX_STAMINA)
            currentStamina = MAX_STAMINA;
        return true;
    }

    public void ResetStamina()
    {
        currentStamina = MAX_STAMINA;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBarManager : MonoBehaviour
{
    [SerializeField] private PlayerHealthSO playerHealthSO;
    [SerializeField] private Slider drowsiness;
    [SerializeField] private Slider buffer;
    [SerializeField] private Player playerHealth;

    private void Awake()
    {
        playerHealth.updateHealthBar += UpdateHealthBarValues;
    }
    void UpdateHealthBarValues()
    {
        drowsiness.value = playerHealthSO.CurrentDrowsiness/playerHealthSO.MaxDrowsiness;
        buffer.value = playerHealthSO.CurrentBuffer / playerHealthSO.MaxBuffer;
    }
}

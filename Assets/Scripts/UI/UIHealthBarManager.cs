using System;
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
        playerHealth.UpdateHealthBar += UpdateHealthBarValues;
    }

    private void Start()
    {
        UpdateHealthBarValues();
    }

    void UpdateHealthBarValues()
    {
        drowsiness.value = playerHealthSO.GetCurrentDrowsiness / playerHealthSO.GetMaxDrowsiness;
        buffer.value = playerHealthSO.GetCurrentBuffer / playerHealthSO.GetMaxBuffer;
    }
}

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : UI
{
    [SerializeField] private PlayerHealthSO playerHealthSO;
    [SerializeField] private PlayerStatisticsSO dreamShardsSO;
    [SerializeField] private PlayerStatisticsSO dreamThreadsSO;
    [SerializeField] private TextMeshProUGUI dreamShardsCount;
    [SerializeField] private TextMeshProUGUI dreamThreadsCount;
    [SerializeField] private Slider drowsiness;
    [SerializeField] private Slider buffer;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        Show();
        
        GameEventsManager.Instance.PlayerEvents.OnUpdateHealthBar += UpdateHealthBarValues;
        GameEventsManager.Instance.PlayerEvents.OnDreamShardsChangeFinished += UpdateDreamShardsCount;
        GameEventsManager.Instance.PlayerEvents.OnDreamThreadsChangeFinished += UpdateDreamThreadsCount;

        UpdateHealthBarValues();
        UpdateDreamShardsCount();
        UpdateDreamThreadsCount();
    }
    
    private void OnDisable()
    {
        GameEventsManager.Instance.PlayerEvents.OnUpdateHealthBar += UpdateHealthBarValues;
        GameEventsManager.Instance.PlayerEvents.OnDreamShardsChangeFinished -= UpdateDreamShardsCount;
        GameEventsManager.Instance.PlayerEvents.OnDreamThreadsChangeFinished -= UpdateDreamThreadsCount;
    }

    private void UpdateHealthBarValues()
    {
        drowsiness.value = playerHealthSO.GetCurrentDrowsinessPercentage;
        buffer.value = playerHealthSO.GetCurrentBuffer / playerHealthSO.GetMaxBuffer;
    }
    
    private void UpdateDreamShardsCount()
    {
        dreamShardsCount.text = dreamShardsSO.GetCurrencyCount().ToString();
    }
    
    private void UpdateDreamThreadsCount()
    {
        dreamThreadsCount.text = dreamThreadsSO.GetCurrencyCount().ToString();
    }
}

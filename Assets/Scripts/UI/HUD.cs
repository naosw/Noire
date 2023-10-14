using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HUD : UI
{
    public static HUD Instance { get; private set; }
    
    [SerializeField] private PlayerHealthSO playerHealthSO;
    [SerializeField] private PlayerStaminaSO PlayerStaminaSO;
    [SerializeField] private PlayerStatisticsSO dreamShardsSO;
    [SerializeField] private PlayerStatisticsSO dreamThreadsSO;
    
    [SerializeField] private TextMeshProUGUI dreamShardsCount;
    [SerializeField] private TextMeshProUGUI dreamThreadsCount;
    
    [SerializeField] private Slider neutralDrowsiness;
    [SerializeField] private Slider lucidDrowsiness; // this appears when you are in lucid state
    [SerializeField] private Slider deepDrowsiness;
    [SerializeField] private Slider stamina;

    private float neutralBarLength;
    private float deepBarLength;

    private void Awake()
    {
        Instance = this;
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        neutralBarLength = Player.Instance.DeepThreshold - Player.Instance.LucidThreshold;
        deepBarLength = 1 - Player.Instance.DeepThreshold;
        
        GameEventsManager.Instance.PlayerEvents.OnUpdateHealthBar += UpdateHealthBar;
        GameEventsManager.Instance.PlayerEvents.OnUpdateStaminaBar += UpdateStaminaBar;
        GameEventsManager.Instance.PlayerEvents.OnDreamShardsChangeFinished += UpdateDreamShardsCount;
        GameEventsManager.Instance.PlayerEvents.OnDreamThreadsChangeFinished += UpdateDreamThreadsCount;

        Show();
        UpdateHealthBar();
        UpdateStaminaBar();
        UpdateDreamShardsCount();
        UpdateDreamThreadsCount();
    }
    
    private void OnDisable()
    {
        GameEventsManager.Instance.PlayerEvents.OnUpdateHealthBar -= UpdateHealthBar;
        GameEventsManager.Instance.PlayerEvents.OnUpdateStaminaBar -= UpdateStaminaBar;
        GameEventsManager.Instance.PlayerEvents.OnDreamShardsChangeFinished -= UpdateDreamShardsCount;
        GameEventsManager.Instance.PlayerEvents.OnDreamThreadsChangeFinished -= UpdateDreamThreadsCount;
    }

    private void UpdateHealthBar()
    {
        ToggleHealthBars();
        var currPercentage = playerHealthSO.CurrentDrowsinessPercentage;
        
        if (currPercentage < Player.Instance.LucidThreshold)
        {
            lucidDrowsiness.value = currPercentage / Player.Instance.LucidThreshold;
        }
        else if (currPercentage > Player.Instance.DeepThreshold)
        {
            neutralDrowsiness.value = 1;
            deepDrowsiness.value = (currPercentage - Player.Instance.DeepThreshold) / deepBarLength;   
        }
        else
        {
            neutralDrowsiness.value = (currPercentage - Player.Instance.LucidThreshold) / neutralBarLength;
            deepDrowsiness.value = 0;
        }
    }

    private void ToggleHealthBars()
    {
        var currPercentage = playerHealthSO.CurrentDrowsinessPercentage;
        if (Player.Instance.LucidThreshold <= currPercentage)
        {
            neutralDrowsiness.gameObject.SetActive(true);
            deepDrowsiness.gameObject.SetActive(true);
            lucidDrowsiness.gameObject.SetActive(false);
        }
        else
        {
            neutralDrowsiness.gameObject.SetActive(false);
            deepDrowsiness.gameObject.SetActive(false);
            lucidDrowsiness.gameObject.SetActive(true);
        }
    }

    private void UpdateStaminaBar()
    {
        stamina.value = PlayerStaminaSO.CurrentStaminaPercentage;
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

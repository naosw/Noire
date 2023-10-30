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
    
    [SerializeField] private SwitchableSprite icon; 

    [SerializeField] private Bar neutralDrowsiness;
    [SerializeField] private Bar lucidDrowsiness;
    [SerializeField] private Bar deepDrowsiness;
    [SerializeField] private Slider stamina;

    private int neutralBarLength;

    private void Awake()
    {
        Instance = this;
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        neutralBarLength = Player.Instance.DeepThreshold - Player.Instance.LucidThreshold;
        
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
        var drowsiness = playerHealthSO.CurrentDrowsiness;
        
        if (drowsiness < Player.Instance.LucidThreshold)
        {
            icon.Switch(0);
            lucidDrowsiness.Display(drowsiness);
        }
        else if (drowsiness > Player.Instance.DeepThreshold)
        {
            icon.Switch(2);
            neutralDrowsiness.Display(neutralBarLength);
            deepDrowsiness.Display(drowsiness - Player.Instance.DeepThreshold);   
        }
        else
        {
            icon.Switch(1);
            neutralDrowsiness.Display(drowsiness - Player.Instance.LucidThreshold);
            deepDrowsiness.Display(0);
        }
    }

    private void ToggleHealthBars()
    {
        if (Player.Instance.LucidThreshold <= playerHealthSO.CurrentDrowsiness)
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

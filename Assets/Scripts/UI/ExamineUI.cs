using System;
using TMPro;
using UnityEngine;

public class ExamineUI : UI
{
    public static ExamineUI Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI examineText;
    [SerializeField] private ButtonUI closeButton;

    private void Awake()
    {
        Instance = this;

        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        gameObject.SetActive(false);
        closeButton.AddListener(Hide);
    }

    public void Display(string text)
    {
        if(!gameObject.activeSelf)
            gameObject.SetActive(true);
        examineText.text = '-' + text;
        Show();
        GameEventsManager.Instance.GameStateEvents.UIToggle(true);
    }

    protected override void Deactivate()
    {
        GameEventsManager.Instance.GameStateEvents.UIToggle(false);
    }
}

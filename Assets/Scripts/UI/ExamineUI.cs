using System;
using System.Collections;
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
        gameObject.SetActive(true);
        examineText.text = '-' + text;
        Show();
        StartCoroutine(HideText());
    }

    IEnumerator HideText()
    {
        yield return new WaitForSeconds(10);
        Hide();
    }
}

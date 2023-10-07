using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ButtonUI : MonoBehaviour
{
    [SerializeField] private Color textColorTransparent;
    [SerializeField] private Color textColorOpaque;
    
    public TextMeshProUGUI buttonText;
    private Button button;

    private void Awake()
    {
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        button = GetComponent<Button>();
    }

    public void Disable()
    {
        button.interactable = false;
        buttonText.color = textColorTransparent;
    }

    public void Enable()
    {
        button.interactable = true;
        buttonText.color = textColorOpaque;
    }

    public void AddListener(UnityAction call)
    {
        button.onClick.AddListener(call);
    }
}
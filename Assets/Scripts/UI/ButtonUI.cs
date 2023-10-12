using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
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

    public void Disable(bool setTransparent = true)
    {
        button.interactable = false;
        if(setTransparent)
            buttonText.color = textColorTransparent;
    }

    public void Enable()
    {
        button.interactable = true;
        buttonText.color = textColorOpaque;
    }

    public void AddListener(UnityAction call)
    {
        // TODO: add playsound effects of mouse click
        button.onClick.AddListener(call);
    }

    public void SetText(string text)
    {
        buttonText.text = text;
    }
}
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

public class InteractUI : MonoBehaviour
{
    [SerializeField] private GameObject containerGameObject;
    [SerializeField] private PlayerInteract playerInteract;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private Color cannotInteractTextColor;
    [SerializeField] private Color canInteractTextColor;
    private IInteractable interactable;
    
    private void Update()
    {
        if (!PauseMenuManager.Instance.IsGamePaused && 
            (interactable = playerInteract.GetInteractableObject()) != null)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }
    
    private void Show()
    {
        containerGameObject.SetActive(true);
        
        if (!interactable.CanInteract())
        {
            interactText.text = interactable.GetCannotInteractText();
            interactText.color = cannotInteractTextColor;
        }
        else
        {
            interactText.text = interactable.GetInteractText();
            interactText.color = canInteractTextColor;
        }
    }
    
    private void Hide()
    {
        containerGameObject.SetActive(false); 
    }
    
}

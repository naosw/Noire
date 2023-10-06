using UnityEngine;
using TMPro;

public class InteractUI : MonoBehaviour
{
    [SerializeField] private GameObject containerGameObject;
    [SerializeField] private PlayerInteract playerInteract;
    [SerializeField] private TextMeshProUGUI interactTextMeshProUGUI;

    private IInteractable interactable;
    private void Update()
    {
        interactable = playerInteract.GetInteractableObject();
        if (interactable != null)
            Show();
        else
            Hide(); 
    }
    private void Show()
    {
        containerGameObject.SetActive(true);
        interactTextMeshProUGUI.text = interactable.GetInteractText();
    }
    private void Hide()
    {
        containerGameObject.SetActive(false); 
    }
    
}

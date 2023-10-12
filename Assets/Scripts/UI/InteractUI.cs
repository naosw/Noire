using System;
using UnityEngine;
using TMPro;

public class InteractUI : UI
{
    [SerializeField] private PlayerInteract playerInteract;
    [SerializeField] private TextMeshProUGUI interactText;
    private IInteractable interactable;

    private bool isShowing; 

    private void Awake()
    {
        alternativeGameObject = true;
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        Hide();
        GameEventsManager.Instance.GameStateEvents.OnPauseToggle += ToggleActive;
    }

    private void OnDestroy()
    {
        GameEventsManager.Instance.GameStateEvents.OnPauseToggle -= ToggleActive;
    }
    
    private void ToggleActive(bool paused)
    {
        gameObject.SetActive(!paused);
    }

    private void Update()
    {
        interactable = playerInteract.GetInteractableObject();
        if (interactable != null && !isShowing)
        {
            Show();
        }
        if(interactable == null && isShowing)
        {
            Hide();
        }
    }
    
    protected override void Activate()
    {
        interactText.text = interactable.GetInteractText();
        isShowing = true;
    }
    
    protected override void Deactivate()
    {
        isShowing = false;
    }
}

using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private Color textColorTransparent;
    [SerializeField] private Color textColorOpaque;
    [SerializeField] public TextMeshProUGUI buttonText;
    [SerializeField] private CanvasGroup leftIndicator;
    [SerializeField] private CanvasGroup rightIndicator;
    private float scaleAmount = 1.05f;
    private float animationTime = .2f;
    private Vector3 initialScale;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Start()
    {
        initialScale = transform.localScale;
        SetIndicatorAlphas(0);
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

    private IEnumerator AnimateSelection(bool startAnimation)
    {
        if (!startAnimation)
        {
            transform.localScale = initialScale;
            SetIndicatorAlphas(0);
        }
        else
        {
            SetIndicatorAlphas(1);

            Vector3 endScale = initialScale * scaleAmount;
            float time = 0;
        
            while (time < animationTime)
            {
                time += Time.deltaTime;
                
                float eval = time / animationTime;
                transform.localScale = Vector3.Lerp(transform.localScale, endScale, eval);
                
                yield return null;
            }   
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        eventData.selectedObject = gameObject;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        eventData.selectedObject = null;
    }

    public void OnSelect(BaseEventData eventData)
    {
        StartCoroutine(AnimateSelection(true));
    }

    public void OnDeselect(BaseEventData eventData)
    {
        StartCoroutine(AnimateSelection(false));
    }

    private void SetIndicatorAlphas(float alpha)
    {
        if (leftIndicator)
        {
            leftIndicator.alpha = alpha;
            rightIndicator.alpha = alpha;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    public static OptionsUI Instance { get; private set; }

    [SerializeField] private Button soundEffectsButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button moveUpButton;
    [SerializeField] private Button moveDownButton;
    [SerializeField] private Button moveLeftButton; 
    [SerializeField] private Button moveRightButton;
    [SerializeField] private Button attackButton;
    [SerializeField] private Button cameraLeftButton;
    [SerializeField] private Button cameraRightButton;

    [SerializeField] private TextMeshProUGUI moveUpText;
    [SerializeField] private TextMeshProUGUI moveDownText;
    [SerializeField] private TextMeshProUGUI moveLeftText;
    [SerializeField] private TextMeshProUGUI moveRightText;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI cameraLeftText;
    [SerializeField] private TextMeshProUGUI cameraRightText;

    [SerializeField] private Transform pressToRebindKeyTransform;

    private void Awake()
    {
       Instance = this;

        closeButton.onClick.AddListener(() =>
        {
            Hide();
        });

        moveUpButton.onClick.AddListener(() => {RebindBinding(GameInput.Bindings.Move_Up); });   
        moveDownButton.onClick.AddListener(() => {RebindBinding(GameInput.Bindings.Move_Down); });   
        moveLeftButton.onClick.AddListener(() => {RebindBinding(GameInput.Bindings.Move_Left); });   
        moveRightButton.onClick.AddListener(() => {RebindBinding(GameInput.Bindings.Move_Right); });   
        attackButton.onClick.AddListener(() => {RebindBinding(GameInput.Bindings.Attack); });   
        cameraLeftButton.onClick.AddListener(() => {RebindBinding(GameInput.Bindings.Camera_Left); });   
        cameraRightButton.onClick.AddListener(() => {RebindBinding(GameInput.Bindings.Camera_Right); });   

    }

    private void Start()
    {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        UpdateVisual();
        Hide();
        HidePressToRebindKey();
    }
    private void GameInput_OnPauseAction(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void UpdateVisual()
    {
        moveUpText.text = GameInput.Instance.GetBindingText(GameInput.Bindings.Move_Up);
        moveDownText.text = GameInput.Instance.GetBindingText(GameInput.Bindings.Move_Down);
        moveLeftText.text = GameInput.Instance.GetBindingText(GameInput.Bindings.Move_Left);
        moveRightText.text = GameInput.Instance.GetBindingText(GameInput.Bindings.Move_Right);
        attackText.text = GameInput.Instance.GetBindingText(GameInput.Bindings.Attack);
        cameraLeftText.text = GameInput.Instance.GetBindingText(GameInput.Bindings.Camera_Left);
        cameraRightText.text = GameInput.Instance.GetBindingText(GameInput.Bindings.Camera_Right);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void ShowPressToRebindKey()
    {
        pressToRebindKeyTransform.gameObject.SetActive(true);
    }
    private void HidePressToRebindKey()
    {
        pressToRebindKeyTransform.gameObject.SetActive(false);
    }

    private void RebindBinding(GameInput.Bindings binding)
    {
        ShowPressToRebindKey();
        GameInput.Instance.RebindBinding(binding, () => {
            HidePressToRebindKey();
            UpdateVisual();
        }); 
    }

}


using UnityEngine;

public class ControlsUI : UI
{
    public static ControlsUI Instance { get; private set; }

    [SerializeField] private ButtonUI moveUpButton;
    [SerializeField] private ButtonUI moveDownButton;
    [SerializeField] private ButtonUI moveLeftButton; 
    [SerializeField] private ButtonUI moveRightButton;
    [SerializeField] private ButtonUI cameraLeftButton;
    [SerializeField] private ButtonUI cameraRightButton;
    [SerializeField] private ButtonUI lightAttackButton;
    [SerializeField] private ButtonUI strongAttackButton;
    [SerializeField] private ButtonUI dashButton;
    [SerializeField] private ButtonUI interactButton;
    [SerializeField] private ButtonUI ability1Button;
    [SerializeField] private ButtonUI ability2Button;
    [SerializeField] private ButtonUI ability3Button;
    
    [SerializeField] private Transform pressToRebindKeyTransform;
    
    private void Awake()
    {
       Instance = this; 
    }

    private void Start()
    {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        
        moveUpButton.AddListener(() => {RebindBinding(GameInput.Bindings.MoveUp); });   
        moveDownButton.AddListener(() => {RebindBinding(GameInput.Bindings.MoveDown); });   
        moveLeftButton.AddListener(() => {RebindBinding(GameInput.Bindings.MoveLeft); });   
        moveRightButton.AddListener(() => {RebindBinding(GameInput.Bindings.MoveRight); });
       
        cameraLeftButton.AddListener(() => {RebindBinding(GameInput.Bindings.CameraLeft); });   
        cameraRightButton.AddListener(() => {RebindBinding(GameInput.Bindings.CameraRight); });
       
        lightAttackButton.AddListener(() => {RebindBinding(GameInput.Bindings.LightAttack); });   
        strongAttackButton.AddListener(() => { RebindBinding(GameInput.Bindings.StrongAttack); });
        dashButton.AddListener(() => { RebindBinding(GameInput.Bindings.Dash); });
        interactButton.AddListener(() => { RebindBinding(GameInput.Bindings.Interact); });
       
        ability1Button.AddListener(() => {RebindBinding(GameInput.Bindings.Ability1); });
        ability2Button.AddListener(() => {RebindBinding(GameInput.Bindings.Ability2); });
        ability3Button.AddListener(() => {RebindBinding(GameInput.Bindings.Ability3); });
        
        UpdateVisual();
        Hide();
        HidePressToRebindKey();
    }

    private void GameInput_OnPauseAction()
    {
        Hide();
    }

    private void UpdateVisual()
    {
        moveUpButton.buttonText.text = GameInput.Instance.GetBindingText(GameInput.Bindings.MoveUp);
        moveDownButton.buttonText.text = GameInput.Instance.GetBindingText(GameInput.Bindings.MoveDown);
        moveLeftButton.buttonText.text = GameInput.Instance.GetBindingText(GameInput.Bindings.MoveLeft);
        moveRightButton.buttonText.text = GameInput.Instance.GetBindingText(GameInput.Bindings.MoveRight);
        
        cameraLeftButton.buttonText.text = GameInput.Instance.GetBindingText(GameInput.Bindings.CameraLeft);
        cameraRightButton.buttonText.text = GameInput.Instance.GetBindingText(GameInput.Bindings.CameraRight);
        
        lightAttackButton.buttonText.text = GameInput.Instance.GetBindingText(GameInput.Bindings.LightAttack);
        strongAttackButton.buttonText.text = GameInput.Instance.GetBindingText(GameInput.Bindings.StrongAttack);
        dashButton.buttonText.text = GameInput.Instance.GetBindingText(GameInput.Bindings.Dash);
        interactButton.buttonText.text = GameInput.Instance.GetBindingText(GameInput.Bindings.Interact);
        
        ability1Button.buttonText.text = GameInput.Instance.GetBindingText(GameInput.Bindings.Ability1);
        ability2Button.buttonText.text = GameInput.Instance.GetBindingText(GameInput.Bindings.Ability2);
        ability3Button.buttonText.text = GameInput.Instance.GetBindingText(GameInput.Bindings.Ability3);
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


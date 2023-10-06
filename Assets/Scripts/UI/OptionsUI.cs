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
    
    [SerializeField] private TextMeshProUGUI soundEffectText;
    [SerializeField] private TextMeshProUGUI musicText;
    [SerializeField] private TextMeshProUGUI moveUpText;
    [SerializeField] private TextMeshProUGUI moveDownText;
    [SerializeField] private TextMeshProUGUI moveLeftText;
    [SerializeField] private TextMeshProUGUI moveRightText;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI cameraLeftText;
    [SerializeField] private TextMeshProUGUI cameraRightText;

    [SerializeField] private Transform pressToRebindKeyTransform;
    
    [Header("Audio Manager")] 
    [SerializeField] private AudioManager audioManager;
    private void Awake()
    {
       Instance = this; 
       closeButton.onClick.AddListener(Hide);
       soundEffectsButton.onClick.AddListener(() => VolChange("Sfx"));
       musicButton.onClick.AddListener(() => VolChange("Ost"));
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

    private void VolChange(string vcaType)
    {
        float currentVol = audioManager.getVcaVolume(vcaType);
        // Debug.Log(currentVol);
        float desVol = currentVol + 0.25f;
        if (vcaType == "Sfx")
        {
            if ( desVol <= 1.25)
            {
                audioManager.SetSfxVolume(desVol);
                float covVolume = (desVol * 4);
                soundEffectText.text = "Sound Effects: " + covVolume.ToString();
            }
            else if (desVol == 1.50){
                audioManager.SetSfxVolume(0);
                soundEffectText.text = "Sound Effects: 0";
                
            }
            else
            {
                desVol = desVol - 1.50f;
                audioManager.SetSfxVolume(desVol);
                float covVolume = (desVol * 4);
                soundEffectText.text = "Sound Effects: " + covVolume.ToString();
            }
        }
        else
        {
            if ( desVol <= 1.25)
            {
                audioManager.SetOstVolume(desVol);
                float covVolume = (desVol * 4);
                musicText.text = "Music: " + covVolume.ToString();
            }
            else if (desVol == 1.50){
                audioManager.SetOstVolume(0);
                musicText.text = "Music: 0";
            }
            else
            {
                desVol = desVol - 1.25f;
                audioManager.SetOstVolume(desVol);
                float covVolume = (desVol * 4);
                musicText.text = "Music: " +covVolume.ToString();
            }
        }
        
    }

}


using UnityEngine;

public class OptionsUI : MonoBehaviour
{
    public static OptionsUI Instance { get; private set; }

    [SerializeField] private ButtonUI soundEffectsButton;
    [SerializeField] private ButtonUI musicButton;
    [SerializeField] private ButtonUI controlsButton;
    [SerializeField] private ButtonUI closeButton;
    
    [Header("Audio Manager")] 
    [SerializeField] private AudioManager audioManager;
    
    private void Awake()
    {
       Instance = this; 
       soundEffectsButton.AddListener(() => VolChange("Sfx"));
       musicButton.AddListener(() => VolChange("Ost"));
       controlsButton.AddListener(OnControlsButtonClicked);
       closeButton.AddListener(Hide);
    }

    private void Start()
    {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        Hide();
    }

    private void OnDestroy()
    {
        GameInput.Instance.OnPauseAction -= GameInput_OnPauseAction;
    }

    private void GameInput_OnPauseAction()
    {
        Hide();
    }

    private void OnControlsButtonClicked()
    {
        Hide();
        ControlsUI.Instance.Show();
    }

    private void VolChange(string vcaType)
    {
        float currentVol = audioManager.GetVcaVolume(vcaType);
        // Debug.Log(currentVol);
        float desVol = currentVol + 0.25f;
        if (vcaType == "Sfx")
        {
            if ( desVol <= 1.25)
            {
                audioManager.SetSfxVolume(desVol);
                float covVolume = (desVol * 4);
                soundEffectsButton.buttonText.text = "Sound Effects: " + covVolume + "/5";
            }
            else if (desVol == 1.50){
                audioManager.SetSfxVolume(0);
                soundEffectsButton.buttonText.text = "Sound Effects: 0" + "/5";
            }
            else
            {
                desVol = desVol - 1.50f;
                audioManager.SetSfxVolume(desVol);
                float covVolume = (desVol * 4);
                soundEffectsButton.buttonText.text = "Sound Effects: " + covVolume + "/5";
            }
        }
        else
        {
            if ( desVol <= 1.25)
            {
                audioManager.SetOstVolume(desVol);
                float covVolume = (desVol * 4);
                musicButton.buttonText.text = "Music: " + covVolume + "/5";
            }
            else if (desVol == 1.50){
                audioManager.SetOstVolume(0);
                musicButton.buttonText.text = "Music: 0" + "/5";
            }
            else
            {
                desVol = desVol - 1.25f;
                audioManager.SetOstVolume(desVol);
                float covVolume = (desVol * 4);
                musicButton.buttonText.text = "Music: " + covVolume + "/5";
            }
        }
    }
    
    public void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

}


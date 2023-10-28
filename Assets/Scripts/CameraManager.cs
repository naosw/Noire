using System.Collections;
using UnityEngine;
using Cinemachine;
using UnityEngine.Serialization;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    [Header("Camera zoom/pan")]
    // here orthographic camera is used so FOV is actually m_Lens.OrthographicSize
    [SerializeField] private float FOVmax;
    [SerializeField] private float FOVmin;
    [SerializeField] private float FOVDefault;
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float shiftCameraPerspectiveSpeed = 20f;

    // only allow 3 camera turn angles: -1 for left, 0 middle, 1 right
    private int cameraPosition;

    [Header("Camera effects")] 
    private const float shakeDuration = .5f;
    private const float shakeMagnitude = 10f;
    private Coroutine shakeCoroutine;
    private CinemachineBasicMultiChannelPerlin shakeNoise;
    
    public Transform LookAt;
    
    private const float EPS = .1f;

    private float targetFOV;
    private bool isMoving;
    private Quaternion targetCameraRotation;

    private void Awake()
    {
        Instance = this;
        cameraPosition = 0;

        shakeNoise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        
        shakeNoise.m_AmplitudeGain = 0;
        virtualCamera.m_Lens.OrthographicSize = FOVDefault;
        targetFOV = virtualCamera.m_Lens.OrthographicSize;
        targetCameraRotation = transform.rotation;
        isMoving = false;
    }

    private void Start()
    {
        GameInput.Instance.OnCameraTurn += GameInput_OnCameraTurn;
        virtualCamera.m_Lens.OrthographicSize = FOVDefault;
    }

    private void GameInput_OnCameraTurn(bool isRightTurn)
    {
        if ((isRightTurn && cameraPosition != 1) ||
            (!isRightTurn && cameraPosition != -1))
        {
            isMoving = true;
            Vector3 offset = new Vector3(0, isRightTurn ? 45 : -45, 0);
            targetCameraRotation = Quaternion.Euler(targetCameraRotation.eulerAngles + offset);
            cameraPosition = isRightTurn ? cameraPosition + 1 : cameraPosition - 1;
        }
    }

    private void Update()
    {
        HandleCameraAngle();
        HandleCameraZoom();
    }

    private void HandleCameraAngle()
    {
        if (isMoving)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetCameraRotation, Time.deltaTime * shiftCameraPerspectiveSpeed);
            if (Mathf.Abs(transform.eulerAngles[1] - targetCameraRotation.eulerAngles[1]) < EPS)
            {
                transform.rotation = targetCameraRotation;
                isMoving = false;
            }
        }
    }

    private void HandleCameraZoom() {
        float zoomVal = GameInput.Instance.GetZoomVal();
        if (zoomVal > 0)
            targetFOV -= .5f;
        if (zoomVal < 0)
            targetFOV += .5f;

        targetFOV = Mathf.Clamp(targetFOV, FOVmin, FOVmax);
        virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(virtualCamera.m_Lens.OrthographicSize, targetFOV, Time.deltaTime * zoomSpeed); ;
    }


    public void CameraShake(float duration=shakeDuration, float magnitude=shakeMagnitude)
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);
        
        shakeCoroutine = StartCoroutine(Shake(duration, magnitude));
    }
    
    private IEnumerator Shake(float duration, float magnitude)
    {
        shakeNoise.m_AmplitudeGain = magnitude;

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            shakeNoise.m_AmplitudeGain = Mathf.Lerp(magnitude, 0, time / duration);
            
            yield return null;
        }

        shakeNoise.m_AmplitudeGain = 0;
        shakeCoroutine = null;
    }
}

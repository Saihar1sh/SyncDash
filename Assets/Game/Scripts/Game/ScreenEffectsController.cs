using Arixen.ScriptSmith;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class ScreenEffectsController : MonoBehaviour
{
    public Volume postProcessVolume;

    private ChromaticAberration chromaticAberration;
    private MotionBlur motionBlur;

    [Header("Crash Effects")]
    public float shakeDuration = 0.5f;
    public float shakeStrength = 0.2f;
    public float chromaticIntensity = 1f;

    void OnEnable()
    {
        EventBusService.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
    }

    void OnDisable()
    {
        EventBusService.UnSubscribe<GameStateChangedEvent>(OnGameStateChanged);
    }

    void Start()
    {
        postProcessVolume.profile.TryGet(out chromaticAberration);
        postProcessVolume.profile.TryGet(out motionBlur);
        chromaticAberration.intensity.value = 0;
    }

    private void OnGameStateChanged(GameStateChangedEvent e)
    {
        if (e.NewState == GameState.GameOver)
        {
            TriggerCrashEffects();
        }
    }

    void Update()
    {
        if (GameManager.Instance.currentState == GameState.Playing && motionBlur != null)
        {
            float speedPercent = GameManager.Instance.currentPlatformSpeed / GameManager.Instance.maxPlatformSpeed;
            motionBlur.intensity.value = Mathf.Lerp(0.3f, 1f, speedPercent);
        }
    }

    public void TriggerCrashEffects()
    {
        transform.DOShakePosition(shakeDuration, shakeStrength);

        if (chromaticAberration != null)
        {
            DOTween.To(() => chromaticAberration.intensity.value, x => chromaticAberration.intensity.value = x, chromaticIntensity, 0.1f)
                .OnComplete(() =>
                {
                    DOTween.To(() => chromaticAberration.intensity.value, x => chromaticAberration.intensity.value = x, 0, 0.5f);
                });
        }
    }
}

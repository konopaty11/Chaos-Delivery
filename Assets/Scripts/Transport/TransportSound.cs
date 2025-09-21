using UnityEngine;

public class TransportSound : MonoBehaviour
{

    [Header("Audio Settings")]
    [SerializeField] private AudioSource engineAudioSource;
    [SerializeField] private AudioClip engineSound;
    [SerializeField] private float minPitch = 0.7f;
    [SerializeField] private float maxPitch = 1.5f;
    [SerializeField] private float minVolume = 0.3f;
    [SerializeField] private float maxVolume = 1.0f;

    [Header("Speed Settings")]
    [SerializeField] private float maxSpeed = 100f;
    [SerializeField] private float pitchMultiplier = 2f;

    private Rigidbody carRigidbody;
    private float currentSpeed;

    void Start()
    {
        // Получаем компоненты
        carRigidbody = GetComponent<Rigidbody>();

        // Настраиваем AudioSource
        if (engineAudioSource == null)
        {
            engineAudioSource = gameObject.AddComponent<AudioSource>();
            engineAudioSource.loop = true;
            engineAudioSource.clip = engineSound;
            engineAudioSource.Play();
        }
    }

    void Update()
    {
        UpdateEngineSound();
    }

    void UpdateEngineSound()
    {
        if (carRigidbody == null || engineAudioSource == null) return;

        // Получаем текущую скорость
        currentSpeed = carRigidbody.linearVelocity.magnitude;

        // Рассчитываем pitch based on speed
        float speedRatio = Mathf.Clamp01(currentSpeed / maxSpeed);
        float targetPitch = Mathf.Lerp(minPitch, maxPitch, speedRatio);

        // Плавное изменение pitch
        engineAudioSource.pitch = targetPitch /*Mathf.Lerp(engineAudioSource.pitch, targetPitch, pitchMultiplier * Time.deltaTime)*/;

        // Изменение громкости
        float targetVolume = Mathf.Lerp(minVolume, maxVolume, speedRatio * 0.5f);
        engineAudioSource.volume = targetVolume;
    }

}

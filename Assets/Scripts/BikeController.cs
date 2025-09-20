using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BikeController : TransportController
{    
    float maxStamina = 200f;
    float currentStamina;
    float depletionRateStamina = 5f;
    float recoveryRateStamina = 20f;

    float heightStage1;
    float yPosStage1;
    float heightStage2;
    float yPosStage2;

    new protected SettingsBike settings;

    GameObject stamina;
    RectTransform stage1Stamina;
    RectTransform stage2Stamina;

    protected override void Awake()
    {
        stamina = TransportManager.Instance.Stamina;
        stage1Stamina = TransportManager.Instance.Stage1Stamina;
        stage2Stamina = TransportManager.Instance.Stage2Stamina;

        base.Awake();

        currentStamina = maxStamina;

        heightStage1 = stage1Stamina.rect.height;
        yPosStage1 = stage1Stamina.anchoredPosition.y;
        heightStage2 = stage2Stamina.rect.height;
        yPosStage2 = stage2Stamina.anchoredPosition.y;

        settings = new SettingsBike(depletionRateStamina);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        StaminaControl();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        stamina.SetActive(true);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (stamina != null)
            stamina.SetActive(false);
    }

    void StaminaControl()
    {
        if (Mathf.Abs(motorSlider.value) > 0.5f && currentStamina > 0)
        {
            float depletionFactor = Mathf.Lerp(1f, 3f, motorSlider.value);

            currentStamina -= depletionRateStamina * depletionFactor * Time.fixedDeltaTime;
            currentStamina = currentStamina < 0 ? 0 : currentStamina;

            powerFactor = currentStamina < maxStamina / 2 ? 0.5f : 1;
            powerFactor = currentStamina == 0 ? 0.1f : powerFactor;
        }
        else if (Mathf.Abs(motorSlider.value) < 0.2f)
        {
            currentStamina += recoveryRateStamina * Time.fixedDeltaTime;

            if (currentStamina >= maxStamina)
                currentStamina = maxStamina;
        }
        
        float factor = currentStamina / maxStamina;
        if (currentStamina >= maxStamina / 2)
        {
            float factorStage1 = (factor - 0.5f) * 2;
            stage1Stamina.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, heightStage1 * factorStage1);
            stage1Stamina.anchoredPosition = new Vector2(stage1Stamina.anchoredPosition.x, yPosStage1 * factorStage1);
        }
        else
        {
            float factorStage2 = factor * 2;
            stage2Stamina.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, heightStage2 * factorStage2);
            stage2Stamina.anchoredPosition = new Vector2(stage2Stamina.anchoredPosition.x, yPosStage2 * factorStage2);
        }
    }

    protected override void SetUpgrates()
    {
        base.SetUpgrates();

        if (currentUpgrade is BikeUpgradeData currentBikeUpgrade)
            depletionRateStamina = settings.GetDeplationRate(currentBikeUpgrade.staminaLevel);
    }
}

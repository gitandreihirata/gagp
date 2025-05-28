using UnityEngine;
using UnityEngine.UI;
using GAGP.Models;
using GAGP.Serialization;

public class S_GAGP_VisualAdjustments_UIController : MonoBehaviour
{
    [Header("Referências de Interface")]
    public Slider brightnessSlider;
    public Slider contrastSlider;
    public Slider colorSlider;

    public Button applyButton;
    public Button resetButton;

    [Header("Applier")]
    public S_GAGP_VisualAdjustments_VolumeApplier applier;

    [Header("Caminho de Configuração")]
    public string settingsPath = "Settings/visual.json";

    void Start()
    {
        // Liga sliders ao applier
        brightnessSlider.onValueChanged.AddListener(applier.SetBrightness);
        contrastSlider.onValueChanged.AddListener(applier.SetContrast);
        colorSlider.onValueChanged.AddListener(applier.SetColorFilter);

        // Botões
        applyButton.onClick.AddListener(ApplyAndSave);
        resetButton.onClick.AddListener(ResetToDefaults);

        // Carrega valores do JSON
        LoadSettings();
    }

    void LoadSettings()
    {
        var fullPath = Application.dataPath + "/" + settingsPath;
        var settings = SettingsSaver.Load<VisualAdjustmentSettings>(fullPath)
                      ?? new VisualAdjustmentSettings();

        brightnessSlider.value = settings.Brightness;
        contrastSlider.value = settings.Contrast;
        colorSlider.value = settings.ColorFilterAmount;
    }

    void ApplyAndSave()
    {
        var newSettings = new VisualAdjustmentSettings
        {
            Brightness = brightnessSlider.value,
            Contrast = contrastSlider.value,
            ColorFilterAmount = colorSlider.value
        };

        var fullPath = Application.dataPath + "/" + settingsPath;
        SettingsSaver.Save(fullPath, newSettings);

        applier.SetBrightness(newSettings.Brightness);
        applier.SetContrast(newSettings.Contrast);
        applier.SetColorFilter(newSettings.ColorFilterAmount);
    }

    void ResetToDefaults()
    {
        LoadSettings();
    }
}

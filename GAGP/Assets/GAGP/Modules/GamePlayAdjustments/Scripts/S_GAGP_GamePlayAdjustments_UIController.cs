using UnityEngine;
using UnityEngine.UI;
using GAGP.Models;
using GAGP.Serialization;

public class S_GAGP_GamePlayAdjustments_UIController : MonoBehaviour
{
    public Toggle oneButtonToggle;
    public S_GAGP_GamePlayAdjustments_OneButtonController controller;
    public string settingsPath = "Settings/gameplay.json";

    private GameplaySettings settings;

    void Start()
    {
        var fullPath = Application.dataPath + "/" + settingsPath;
        settings = SettingsSaver.Load<GameplaySettings>(fullPath) ?? new GameplaySettings();

        oneButtonToggle.isOn = settings.OneButtonModeEnabled;
        oneButtonToggle.onValueChanged.AddListener(OnToggleChanged);

        if (controller != null)
        {
            controller.SetOneButtonEnabled(settings.OneButtonModeEnabled); // força estado inicial
        }
    }

    void OnToggleChanged(bool isEnabled)
    {
        settings.OneButtonModeEnabled = isEnabled;

        var fullPath = Application.dataPath + "/" + settingsPath;
        SettingsSaver.Save(fullPath, settings);

        if (controller != null)
        {
            controller.SetOneButtonEnabled(isEnabled);
        }

        Debug.Log("[GAGP] One Button Mode: " + (isEnabled ? "Ativado" : "Desativado"));
    }
}

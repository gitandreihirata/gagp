using UnityEngine;
using GAGP.Modules.VisualAdjustments.Manager;
using GAGP.Models;
using GAGP.Serialization;

public class S_GAGP_VisualAdjustments_Loader : MonoBehaviour
{
    public string settingsPath = "Settings/visual.json";
    private VisualAdjustmentManager manager;

    void Awake()
    {
        manager = new VisualAdjustmentManager();

        var settings = SettingsSaver.Load<VisualAdjustmentSettings>(Application.dataPath + "/" + settingsPath)
                      ?? new VisualAdjustmentSettings();

        manager.LoadSettings(settings);
        manager.ApplyAll();
    }
}

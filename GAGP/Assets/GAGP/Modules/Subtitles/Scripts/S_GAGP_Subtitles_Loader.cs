using UnityEngine;
using GAGP.Modules.Subtitles.Adjustments;
using GAGP.Modules.Subtitles.Manager;
using GAGP.Serialization;

public class S_GAGP_Subtitles_Loader : MonoBehaviour
{
    public string settingsPath = "Settings/subtitle.json"; // Caminho relativo à Application.dataPath
    public SubtitleAdjustmentManager manager;
    // As 'settings' em si não precisam ser públicas aqui, o manager as conterá.
    // private SubtitleSettings settings; // Removido para evitar confusão, o manager é o detentor

    // Usar Awake para garantir que o manager seja inicializado antes de outros scripts tentarem usá-lo no Start.
    public void Awake() // Deixei público caso precise ser chamado externamente, mas geralmente é chamado pela Unity
    {
        if (manager == null) // Evita reinicialização se já foi feito
        {
            manager = new SubtitleAdjustmentManager();
            var fullPath = Application.dataPath + "/" + settingsPath;

            // Tenta carregar as configurações; se falhar ou não existir, usa as padrões da DLL
            SubtitleSettings loadedSettings = SettingsSaver.Load<SubtitleSettings>(fullPath);
            if (loadedSettings == null)
            {
                Debug.LogWarning($"[GAGP] Arquivo '{fullPath}' não encontrado ou inválido. Usando configurações de legenda padrão.");
                loadedSettings = new SubtitleSettings(); // Cria com os defaults da classe
            }
            manager.LoadSettings(loadedSettings);
            // Debug.Log("[GAGP] SubtitleAdjustmentManager inicializado e configurações carregadas.");
        }
    }
}
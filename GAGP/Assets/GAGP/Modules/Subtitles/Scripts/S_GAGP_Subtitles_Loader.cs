using UnityEngine;
using GAGP.Modules.Subtitles.Adjustments;
using GAGP.Modules.Subtitles.Manager;
using GAGP.Serialization;

public class S_GAGP_Subtitles_Loader : MonoBehaviour
{
    public string settingsPath = "Settings/subtitle.json"; // Caminho relativo � Application.dataPath
    public SubtitleAdjustmentManager manager;
    // As 'settings' em si n�o precisam ser p�blicas aqui, o manager as conter�.
    // private SubtitleSettings settings; // Removido para evitar confus�o, o manager � o detentor

    // Usar Awake para garantir que o manager seja inicializado antes de outros scripts tentarem us�-lo no Start.
    public void Awake() // Deixei p�blico caso precise ser chamado externamente, mas geralmente � chamado pela Unity
    {
        if (manager == null) // Evita reinicializa��o se j� foi feito
        {
            manager = new SubtitleAdjustmentManager();
            var fullPath = Application.dataPath + "/" + settingsPath;

            // Tenta carregar as configura��es; se falhar ou n�o existir, usa as padr�es da DLL
            SubtitleSettings loadedSettings = SettingsSaver.Load<SubtitleSettings>(fullPath);
            if (loadedSettings == null)
            {
                Debug.LogWarning($"[GAGP] Arquivo '{fullPath}' n�o encontrado ou inv�lido. Usando configura��es de legenda padr�o.");
                loadedSettings = new SubtitleSettings(); // Cria com os defaults da classe
            }
            manager.LoadSettings(loadedSettings);
            // Debug.Log("[GAGP] SubtitleAdjustmentManager inicializado e configura��es carregadas.");
        }
    }
}
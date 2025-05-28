using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GAGP.Modules.Subtitles.Adjustments;
using GAGP.Serialization;
using GAGP.Modules.Subtitles.Manager; // Necessário para o SubtitleAdjustmentManager
using System.Collections.Generic; // Para a lista de opções do Dropdown

public class S_GAGP_Subtitles_UIController : MonoBehaviour
{
    [Header("Referências de UI")]
    public Toggle enableBackgroundToggle;
    public TMP_InputField fontColorInputField;
    public TMP_InputField fontSizeInputField;
    public TMP_Dropdown alignmentDropdown;
    public Toggle typingEffectToggle;
    public TMP_InputField typingSpeedInputField;
    public Button applyButton;
    public Button resetButton;

    [Header("Referências do Sistema de Legendas")]
    public S_GAGP_Subtitles_Loader subtitleLoader;
    public S_GAGP_Subtitles_Controller subtitleDisplayController; // O controlador que mostra as legendas

    private SubtitleSettings currentSettings;
    private string settingsFilePath;

    void Start()
    {
        if (subtitleLoader == null || subtitleDisplayController == null)
        {
            Debug.LogError("[GAGP] S_GAGP_Subtitles_UIController: Referências do sistema de legendas (Loader ou DisplayController) não configuradas no Inspector!");
            this.enabled = false; // Desabilita o script se as refs não estiverem ok
            return;
        }

        // Garante que o loader inicializou e carregou as settings
        if (subtitleLoader.manager == null)
        {
            // Isso pode acontecer se este script rodar o Start() antes do Loader.
            // Forçar a inicialização do loader pode ser uma opção,
            // mas o ideal é garantir a ordem de execução ou que o loader seja um Singleton/serviço já pronto.
            // Por agora, vamos assumir que o loader já está pronto ou logar um erro.
            Debug.LogWarning("[GAGP] S_GAGP_Subtitles_UIController: SubtitleLoader.manager não inicializado. Tentando inicializar...");
            subtitleLoader.Awake(); // Tenta chamar o Awake do loader
            if (subtitleLoader.manager == null)
            {
                Debug.LogError("[GAGP] Falha ao inicializar SubtitleLoader.manager. A UI de legendas pode não funcionar.");
                this.enabled = false;
                return;
            }
        }

        settingsFilePath = Application.dataPath + "/" + subtitleLoader.settingsPath;
        currentSettings = subtitleLoader.manager.GetSettings(); // Pega as settings carregadas pelo Loader

        PopulateUIFromSettings(currentSettings);

        // Listeners dos botões
        applyButton.onClick.AddListener(ApplyAndSaveSettings);
        resetButton.onClick.AddListener(ResetToDefaultSettings);
    }

    void PopulateUIFromSettings(SubtitleSettings settings)
    {
        enableBackgroundToggle.isOn = settings.EnableBackground;
        fontColorInputField.text = settings.FontColorHex;
        fontSizeInputField.text = settings.FontSize.ToString();

        alignmentDropdown.ClearOptions();
        List<string> alignmentOptions = new List<string> { "TopLeft", "TopCenter", "TopRight", "MiddleLeft", "MiddleCenter", "MiddleRight", "BottomLeft", "BottomCenter", "BottomRight" };
        alignmentDropdown.AddOptions(alignmentOptions);

        int currentIndex = alignmentOptions.FindIndex(option => option.Equals(settings.Alignment, System.StringComparison.OrdinalIgnoreCase));
        if (currentIndex == -1)
        {
            Debug.LogWarning($"[GAGP] Alinhamento '{settings.Alignment}' não encontrado nas opções. Usando MidCenter.");
            currentIndex = alignmentOptions.FindIndex(option => option.Equals("MiddleCenter", System.StringComparison.OrdinalIgnoreCase)); ; // Padrão se não encontrar
        }
        alignmentDropdown.value = currentIndex;
        alignmentDropdown.RefreshShownValue();

        typingEffectToggle.isOn = settings.TypingEffect;
        typingSpeedInputField.text = settings.TypingSpeed.ToString("F2"); // "F2" formata para 2 casas decimais
    }

    public void ApplyAndSaveSettings()
    {
        if (currentSettings == null) return;

        currentSettings.EnableBackground = enableBackgroundToggle.isOn;
        currentSettings.FontColorHex = fontColorInputField.text; // Adicionar validação de Hex se necessário

        if (int.TryParse(fontSizeInputField.text, out int fontSize) && fontSize > 0)
        {
            currentSettings.FontSize = fontSize;
        }
        else
        {
            Debug.LogWarning("[GAGP] Tamanho da fonte inválido. Mantendo valor anterior.");
            fontSizeInputField.text = currentSettings.FontSize.ToString();
        }

        currentSettings.Alignment = alignmentDropdown.options[alignmentDropdown.value].text;
        currentSettings.TypingEffect = typingEffectToggle.isOn;

        if (float.TryParse(typingSpeedInputField.text, out float typingSpeed) && typingSpeed > 0)
        {
            currentSettings.TypingSpeed = typingSpeed;
        }
        else
        {
            Debug.LogWarning("[GAGP] Velocidade de digitação inválida. Mantendo valor anterior.");
            typingSpeedInputField.text = currentSettings.TypingSpeed.ToString("F2");
        }

        subtitleLoader.manager.LoadSettings(currentSettings); // Atualiza as settings no manager da DLL
        SettingsSaver.Save(settingsFilePath, currentSettings); // Salva no arquivo JSON

        subtitleDisplayController.RefreshAppearance(currentSettings); // Manda o painel de legenda se atualizar
        Debug.Log("[GAGP] Configurações de legenda aplicadas e salvas.");
    }

    public void ResetToDefaultSettings()
    {
        SubtitleSettings defaultSettings = new SubtitleSettings(); // Cria uma nova instância com valores padrão do construtor da classe

        PopulateUIFromSettings(defaultSettings); // Atualiza a UI
        currentSettings = defaultSettings; // Atualiza as settings em memória neste controlador

        // Salva e aplica as settings padrão
        subtitleLoader.manager.LoadSettings(currentSettings);
        SettingsSaver.Save(settingsFilePath, currentSettings);
        subtitleDisplayController.RefreshAppearance(currentSettings);
        Debug.Log("[GAGP] Configurações de legenda restauradas para o padrão.");
    }
}
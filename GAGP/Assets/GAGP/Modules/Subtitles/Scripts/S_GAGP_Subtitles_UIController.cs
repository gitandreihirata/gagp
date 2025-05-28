using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GAGP.Modules.Subtitles.Adjustments;
using GAGP.Serialization;
using GAGP.Modules.Subtitles.Manager; // Necess�rio para o SubtitleAdjustmentManager
using System.Collections.Generic; // Para a lista de op��es do Dropdown

public class S_GAGP_Subtitles_UIController : MonoBehaviour
{
    [Header("Refer�ncias de UI")]
    public Toggle enableBackgroundToggle;
    public TMP_InputField fontColorInputField;
    public TMP_InputField fontSizeInputField;
    public TMP_Dropdown alignmentDropdown;
    public Toggle typingEffectToggle;
    public TMP_InputField typingSpeedInputField;
    public Button applyButton;
    public Button resetButton;

    [Header("Refer�ncias do Sistema de Legendas")]
    public S_GAGP_Subtitles_Loader subtitleLoader;
    public S_GAGP_Subtitles_Controller subtitleDisplayController; // O controlador que mostra as legendas

    private SubtitleSettings currentSettings;
    private string settingsFilePath;

    void Start()
    {
        if (subtitleLoader == null || subtitleDisplayController == null)
        {
            Debug.LogError("[GAGP] S_GAGP_Subtitles_UIController: Refer�ncias do sistema de legendas (Loader ou DisplayController) n�o configuradas no Inspector!");
            this.enabled = false; // Desabilita o script se as refs n�o estiverem ok
            return;
        }

        // Garante que o loader inicializou e carregou as settings
        if (subtitleLoader.manager == null)
        {
            // Isso pode acontecer se este script rodar o Start() antes do Loader.
            // For�ar a inicializa��o do loader pode ser uma op��o,
            // mas o ideal � garantir a ordem de execu��o ou que o loader seja um Singleton/servi�o j� pronto.
            // Por agora, vamos assumir que o loader j� est� pronto ou logar um erro.
            Debug.LogWarning("[GAGP] S_GAGP_Subtitles_UIController: SubtitleLoader.manager n�o inicializado. Tentando inicializar...");
            subtitleLoader.Awake(); // Tenta chamar o Awake do loader
            if (subtitleLoader.manager == null)
            {
                Debug.LogError("[GAGP] Falha ao inicializar SubtitleLoader.manager. A UI de legendas pode n�o funcionar.");
                this.enabled = false;
                return;
            }
        }

        settingsFilePath = Application.dataPath + "/" + subtitleLoader.settingsPath;
        currentSettings = subtitleLoader.manager.GetSettings(); // Pega as settings carregadas pelo Loader

        PopulateUIFromSettings(currentSettings);

        // Listeners dos bot�es
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
            Debug.LogWarning($"[GAGP] Alinhamento '{settings.Alignment}' n�o encontrado nas op��es. Usando MidCenter.");
            currentIndex = alignmentOptions.FindIndex(option => option.Equals("MiddleCenter", System.StringComparison.OrdinalIgnoreCase)); ; // Padr�o se n�o encontrar
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
        currentSettings.FontColorHex = fontColorInputField.text; // Adicionar valida��o de Hex se necess�rio

        if (int.TryParse(fontSizeInputField.text, out int fontSize) && fontSize > 0)
        {
            currentSettings.FontSize = fontSize;
        }
        else
        {
            Debug.LogWarning("[GAGP] Tamanho da fonte inv�lido. Mantendo valor anterior.");
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
            Debug.LogWarning("[GAGP] Velocidade de digita��o inv�lida. Mantendo valor anterior.");
            typingSpeedInputField.text = currentSettings.TypingSpeed.ToString("F2");
        }

        subtitleLoader.manager.LoadSettings(currentSettings); // Atualiza as settings no manager da DLL
        SettingsSaver.Save(settingsFilePath, currentSettings); // Salva no arquivo JSON

        subtitleDisplayController.RefreshAppearance(currentSettings); // Manda o painel de legenda se atualizar
        Debug.Log("[GAGP] Configura��es de legenda aplicadas e salvas.");
    }

    public void ResetToDefaultSettings()
    {
        SubtitleSettings defaultSettings = new SubtitleSettings(); // Cria uma nova inst�ncia com valores padr�o do construtor da classe

        PopulateUIFromSettings(defaultSettings); // Atualiza a UI
        currentSettings = defaultSettings; // Atualiza as settings em mem�ria neste controlador

        // Salva e aplica as settings padr�o
        subtitleLoader.manager.LoadSettings(currentSettings);
        SettingsSaver.Save(settingsFilePath, currentSettings);
        subtitleDisplayController.RefreshAppearance(currentSettings);
        Debug.Log("[GAGP] Configura��es de legenda restauradas para o padr�o.");
    }
}
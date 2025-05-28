using UnityEngine;
using UnityEngine.UI; // Necessário para Image
using TMPro;        // Necessário para TextMeshProUGUI e TextAlignmentOptions
using GAGP.Modules.Subtitles.Adjustments; // Para SubtitleSettings
// using GAGP.Modules.Subtitles.Manager; // Não é diretamente usado aqui, mas o Loader usa

public class S_GAGP_Subtitles_Controller : MonoBehaviour
{
    [Header("Referências de UI da Legenda")]
    public TextMeshProUGUI subtitleText; // Deve ser público para arrastar no Inspector
    public Image backgroundPanel;      // Deve ser público para arrastar no Inspector

    [Header("Referências Externas")]
    public S_GAGP_Subtitles_Loader loader; // Referência ao Loader para pegar as settings

    private SubtitleSettings settings; // Configurações atuais carregadas
    private S_GAGP_Subtitles_Typer typer; // Referência ao script de efeito de digitação

    void Awake() // Usar Awake para garantir que o typer é pego antes do Start
    {
        typer = GetComponent<S_GAGP_Subtitles_Typer>();
        if (typer == null)
        {
            Debug.LogError("[GAGP] S_GAGP_Subtitles_Typer não encontrado no mesmo GameObject que S_GAGP_Subtitles_Controller. O efeito de digitação não funcionará.");
        }
    }

    void Start()
    {
        if (loader == null)
        {
            Debug.LogError("[GAGP] S_GAGP_Subtitles_Controller: A referência para 'S_GAGP_Subtitles_Loader' não foi configurada no Inspector!");
            this.enabled = false; // Desabilita o script para evitar mais erros
            return;
        }

        if (loader.manager == null)
        {
            Debug.LogWarning("[GAGP] S_GAGP_Subtitles_Controller: loader.manager não inicializado no Start. Tentando chamar Awake do loader...");
            loader.Awake(); // Garante que o loader tentou carregar as configurações
            if (loader.manager == null)
            {
                Debug.LogError("[GAGP] S_GAGP_Subtitles_Controller: Falha ao inicializar loader.manager. As legendas podem não funcionar corretamente.");
                this.enabled = false;
                return;
            }
        }

        settings = loader.manager.GetSettings();
        if (settings == null)
        {
            Debug.LogError("[GAGP] S_GAGP_Subtitles_Controller: Não foi possível obter as configurações do loader.manager!");
            settings = new SubtitleSettings(); // Usa defaults para evitar NullReferenceException
        }

        RefreshAppearance(settings);

        if (subtitleText != null)
        {
            subtitleText.text = ""; // Limpa o texto inicial
        }
        else
        {
            Debug.LogError("[GAGP] S_GAGP_Subtitles_Controller: 'subtitleText' (TextMeshProUGUI) não configurado no Inspector!");
        }
    }

    public void ShowSubtitle(string message)
    {
        if (subtitleText == null)
        {
            Debug.LogError("[GAGP] Tentativa de mostrar legenda, mas 'subtitleText' não está configurado!");
            return;
        }
        if (settings == null)
        {
            Debug.LogError("[GAGP] Tentativa de mostrar legenda, mas 'settings' são nulas!");
            return;
        }


        if (settings.TypingEffect && typer != null)
        {
            // Garante que o campo de texto do typer está configurado
            if (typer.textField == null) typer.textField = this.subtitleText;
            StartCoroutine(typer.TypeText(message, settings.TypingSpeed));
        }
        else
        {
            subtitleText.text = message;
        }
    }

    public void RefreshAppearance(SubtitleSettings newSettings)
    {
        settings = newSettings;

        if (backgroundPanel != null)
        {
            backgroundPanel.enabled = settings.EnableBackground;
        }
        // Não precisa de 'else' para logar erro aqui, pois o painel de fundo é opcional

        if (subtitleText != null)
        {
            subtitleText.color = HexToColor(settings.FontColorHex);
            subtitleText.fontSize = settings.FontSize;
            subtitleText.alignment = GetAlignmentFromString(settings.Alignment);
        }
        // O erro de subtitleText não configurado já é pego no Start
        // Debug.Log("[GAGP] Aparência do painel de legendas atualizada."); // Log opcional
    }

    // Métodos auxiliares devem ser privados ou protected se não usados fora da classe
    private Color HexToColor(string hex)
    {
        Color color = Color.white; // Cor padrão
        if (!string.IsNullOrEmpty(hex))
        {
            ColorUtility.TryParseHtmlString(hex, out color);
        }
        return color;
    }

    private TextAlignmentOptions GetAlignmentFromString(string alignmentString)
    {
        if (string.IsNullOrEmpty(alignmentString)) return TextAlignmentOptions.Bottom; // Padrão

        switch (alignmentString.ToLowerInvariant())
        {
            case "topleft": return TextAlignmentOptions.TopLeft;
            case "topcenter": return TextAlignmentOptions.Top; // TMPro usa Top para TopCenter
            case "topright": return TextAlignmentOptions.TopRight;
            case "middleleft": return TextAlignmentOptions.Left; // TMPro usa Left para MiddleLeft
            case "middlecenter": return TextAlignmentOptions.Center; // TMPro usa Center para MiddleCenter
            case "middleright": return TextAlignmentOptions.Right; // TMPro usa Right para MiddleRight
            case "bottomleft": return TextAlignmentOptions.BottomLeft;
            case "bottomcenter": return TextAlignmentOptions.Bottom; // TMPro usa Bottom para BottomCenter
            case "bottomright": return TextAlignmentOptions.BottomRight;
            default:
                Debug.LogWarning($"[GAGP] Alinhamento desconhecido: {alignmentString}. Usando BottomCenter como padrão.");
                return TextAlignmentOptions.Bottom;
        }
    }
}
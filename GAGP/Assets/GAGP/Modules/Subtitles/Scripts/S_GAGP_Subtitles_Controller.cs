using UnityEngine;
using UnityEngine.UI; // Necess�rio para Image
using TMPro;        // Necess�rio para TextMeshProUGUI e TextAlignmentOptions
using GAGP.Modules.Subtitles.Adjustments; // Para SubtitleSettings
// using GAGP.Modules.Subtitles.Manager; // N�o � diretamente usado aqui, mas o Loader usa

public class S_GAGP_Subtitles_Controller : MonoBehaviour
{
    [Header("Refer�ncias de UI da Legenda")]
    public TextMeshProUGUI subtitleText; // Deve ser p�blico para arrastar no Inspector
    public Image backgroundPanel;      // Deve ser p�blico para arrastar no Inspector

    [Header("Refer�ncias Externas")]
    public S_GAGP_Subtitles_Loader loader; // Refer�ncia ao Loader para pegar as settings

    private SubtitleSettings settings; // Configura��es atuais carregadas
    private S_GAGP_Subtitles_Typer typer; // Refer�ncia ao script de efeito de digita��o

    void Awake() // Usar Awake para garantir que o typer � pego antes do Start
    {
        typer = GetComponent<S_GAGP_Subtitles_Typer>();
        if (typer == null)
        {
            Debug.LogError("[GAGP] S_GAGP_Subtitles_Typer n�o encontrado no mesmo GameObject que S_GAGP_Subtitles_Controller. O efeito de digita��o n�o funcionar�.");
        }
    }

    void Start()
    {
        if (loader == null)
        {
            Debug.LogError("[GAGP] S_GAGP_Subtitles_Controller: A refer�ncia para 'S_GAGP_Subtitles_Loader' n�o foi configurada no Inspector!");
            this.enabled = false; // Desabilita o script para evitar mais erros
            return;
        }

        if (loader.manager == null)
        {
            Debug.LogWarning("[GAGP] S_GAGP_Subtitles_Controller: loader.manager n�o inicializado no Start. Tentando chamar Awake do loader...");
            loader.Awake(); // Garante que o loader tentou carregar as configura��es
            if (loader.manager == null)
            {
                Debug.LogError("[GAGP] S_GAGP_Subtitles_Controller: Falha ao inicializar loader.manager. As legendas podem n�o funcionar corretamente.");
                this.enabled = false;
                return;
            }
        }

        settings = loader.manager.GetSettings();
        if (settings == null)
        {
            Debug.LogError("[GAGP] S_GAGP_Subtitles_Controller: N�o foi poss�vel obter as configura��es do loader.manager!");
            settings = new SubtitleSettings(); // Usa defaults para evitar NullReferenceException
        }

        RefreshAppearance(settings);

        if (subtitleText != null)
        {
            subtitleText.text = ""; // Limpa o texto inicial
        }
        else
        {
            Debug.LogError("[GAGP] S_GAGP_Subtitles_Controller: 'subtitleText' (TextMeshProUGUI) n�o configurado no Inspector!");
        }
    }

    public void ShowSubtitle(string message)
    {
        if (subtitleText == null)
        {
            Debug.LogError("[GAGP] Tentativa de mostrar legenda, mas 'subtitleText' n�o est� configurado!");
            return;
        }
        if (settings == null)
        {
            Debug.LogError("[GAGP] Tentativa de mostrar legenda, mas 'settings' s�o nulas!");
            return;
        }


        if (settings.TypingEffect && typer != null)
        {
            // Garante que o campo de texto do typer est� configurado
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
        // N�o precisa de 'else' para logar erro aqui, pois o painel de fundo � opcional

        if (subtitleText != null)
        {
            subtitleText.color = HexToColor(settings.FontColorHex);
            subtitleText.fontSize = settings.FontSize;
            subtitleText.alignment = GetAlignmentFromString(settings.Alignment);
        }
        // O erro de subtitleText n�o configurado j� � pego no Start
        // Debug.Log("[GAGP] Apar�ncia do painel de legendas atualizada."); // Log opcional
    }

    // M�todos auxiliares devem ser privados ou protected se n�o usados fora da classe
    private Color HexToColor(string hex)
    {
        Color color = Color.white; // Cor padr�o
        if (!string.IsNullOrEmpty(hex))
        {
            ColorUtility.TryParseHtmlString(hex, out color);
        }
        return color;
    }

    private TextAlignmentOptions GetAlignmentFromString(string alignmentString)
    {
        if (string.IsNullOrEmpty(alignmentString)) return TextAlignmentOptions.Bottom; // Padr�o

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
                Debug.LogWarning($"[GAGP] Alinhamento desconhecido: {alignmentString}. Usando BottomCenter como padr�o.");
                return TextAlignmentOptions.Bottom;
        }
    }
}
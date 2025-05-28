using UnityEngine;
using System.Collections;
using TMPro;

public class S_GAGP_Subtitles_Typer : MonoBehaviour
{
    public TextMeshProUGUI textField; // O TextMeshProUGUI onde o texto ser� "digitado"

    // Este m�todo ser� chamado pelo S_GAGP_Subtitles_Controller
    public IEnumerator TypeText(string fullText, float delayPerChar)
    {
        if (textField == null)
        {
            Debug.LogError("[GAGP] S_GAGP_Subtitles_Typer: textField (TextMeshProUGUI) n�o est� atribu�do!");
            yield break; // Interrompe a coroutine se n�o houver campo de texto
        }

        textField.text = ""; // Limpa o texto anterior
        foreach (char c in fullText)
        {
            textField.text += c;
            yield return new WaitForSeconds(delayPerChar);
        }
    }
}